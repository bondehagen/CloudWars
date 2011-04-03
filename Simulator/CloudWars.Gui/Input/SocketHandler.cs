using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using CloudWars.Core;
using CloudWars.Core.Input;
using CloudWars.Helpers;

namespace CloudWars.Input
{
    public class SocketHandler : IInputHandler
    {
        private const int bufferSize = 256;
        private readonly TcpClient client;
        private readonly Queue<string> commandQueue;
        private readonly byte[] data;
        private readonly NetworkStream netStream;
        private bool disposed;
        private string playerName = null;
        private dynamic state;

        public SocketHandler(TcpClient client)
        {
            commandQueue = new Queue<string>();
            this.client = client;
            data = new byte[bufferSize];
            netStream = this.client.GetStream();
            netStream.BeginRead(data, 0, bufferSize, ReceiveMessage, null);
        }

        public void Dispose()
        {
            if (disposed) return;
            netStream.Close();
            client.Close();
            disposed = true;
        }

        public void Start()
        {
            SendMessage("START\n");
        }

        public void Update(Thunderstorm thunderstorm, World world, int iteration)
        {
            if (thunderstorm.IsDead() || !IsConnected())
            {
                Dispose();
                return;
            }

            thunderstorm.Name = playerName;

            state = new
                        {
                            player = thunderstorm,
                            world,
                            iteration
                        };

            while (commandQueue.Any())
            {
                ParseMessage(commandQueue.Dequeue(), thunderstorm);
            }
        }

        private bool IsConnected()
        {
            try
            {
                return (client.Connected && client.Available != -1 && netStream.CanRead && netStream.CanWrite);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string WaitForName()
        {
            while (string.IsNullOrEmpty(playerName))
                Thread.Sleep(100);

            return playerName;
        }

        public void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                if (!client.Connected && !netStream.CanRead)
                    return;

                int bufferLength = netStream.EndRead(ar);
                string messageReceived = Encoding.ASCII.GetString(data, 0, bufferLength);
                if (!messageReceived.IsNullOrWhiteSpace())
                {
                    commandQueue.Enqueue(messageReceived);
                    CheckForNameMessage(messageReceived);
                    netStream.Flush();
                }
                netStream.BeginRead(data, 0, bufferSize, ReceiveMessage, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void CheckForNameMessage(string messageReceived)
        {
            char[] separators = new[] { '\n', '\r', '\f', '\0', (char) 3 };
            string[] strings = messageReceived.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<KeyValuePair<string, string>> lines = GetLines(strings);
            foreach (KeyValuePair<string, string> line in lines)
            {
                if (line.Key.Equals("NAME"))
                {
                    playerName = line.Value;
                    return;
                }
            }
        }

        private void ParseMessage(string messageReceived, Thunderstorm player)
        {
            char[] separators = new[] { '\n', '\r', '\f', '\0', (char) 3 };
            string[] strings = messageReceived.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<KeyValuePair<string, string>> lines = GetLines(strings);
            foreach (KeyValuePair<string, string> line in lines)
            {
                if (state == null)
                    continue;

                if (line.Key.Equals("WIND"))
                {
                    SendMessage(player.Wind(ParseVector(line.Value)) ? "OK\n" : "IGNORED\n");
                }
                if (line.Key.Equals("GET_STATE"))
                {
                    this.SendMessage(StateProtocol.Create(state.player, state.world, state.iteration));
                }
            }
        }

        private static Vector ParseVector(string value)
        {
            string[] vector = value.Split(' ');
            double x = double.Parse(vector.First(), NumberFormatInfo.InvariantInfo);
            double y = double.Parse(vector.Last(), NumberFormatInfo.InvariantInfo);
            return new Vector(x, y);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetLines(IEnumerable<string> strings)
        {
            return strings.Select(s =>
                                      {
                                          string[] split = s.Split(new[] { ' ' }, 2);
                                          return split.Any()
                                                     ? new KeyValuePair<string, string>(split.First().ToUpper(),
                                                                                        split.Last())
                                                     : new KeyValuePair<string, string>(s, "");
                                      }).ToArray();
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
                client.Client.Send(bytesToSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
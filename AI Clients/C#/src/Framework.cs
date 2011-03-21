using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Windows;
using System.Globalization;

namespace ClientAI
{
    public class Cloud
    {
        public Vector Position;
        public Vector Velocity;
        public float Vapor;
        public float Radius
        {
            get { return (float)Math.Sqrt(Vapor); }
        }
        public Cloud(float x, float y, float vx, float vy, float vapor)
        {
            this.Position = new Vector(x, y);
            this.Velocity = new Vector(vx, vy);
            this.Vapor = vapor;
        }
    }

    public class GameState
    {
        public readonly List<Cloud> Thunderstorms = new List<Cloud>();
        public readonly List<Cloud> Rainclouds = new List<Cloud>();
        public int MeIndex;
        public Cloud Me
        {
            get { return Thunderstorms[MeIndex]; }
        }
    }

    public partial class Client
    {
        private readonly TcpClient client = new TcpClient();
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private readonly byte[] data = new byte[256];
        private readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1986);
        
        public GameState GetState()
        {
            try
            {
                writer.WriteLine("GET_STATE");
                writer.Flush();

                GameState state = new GameState();
                bool end = false;
                while (!end)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        string[] msg = line.Split(' ');
                        switch (msg[0])
                        {
                            case "BEGIN_STATE": break;
                            case "END_STATE": end = true; break;
                            case "THUNDERSTORM": state.Thunderstorms.Add(
                                new Cloud(
                                    float.Parse(msg[1], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[2], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[3], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[4], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[5], NumberFormatInfo.InvariantInfo)));
                                break;
                            case "RAINCLOUD": state.Rainclouds.Add(
                                new Cloud(
                                    float.Parse(msg[1], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[2], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[3], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[4], NumberFormatInfo.InvariantInfo),
                                    float.Parse(msg[5], NumberFormatInfo.InvariantInfo)));
                                break;
                            case "YOU": state.MeIndex = int.Parse(msg[1]); break;
                        }
                    }
                }
                return state;
            }
            catch (Exception) { return null; }
        }

        public void SetName(string name)
        {
            writer.WriteLine("NAME " + name);
            writer.Flush();
        }

        public bool Wind(float x, float y)
        {
            writer.WriteLine("WIND " + x.ToString(NumberFormatInfo.InvariantInfo) + " " + y.ToString(NumberFormatInfo.InvariantInfo));
            writer.Flush();
            string response = reader.ReadLine();
            if (response == "OK") return true;
            else return false;
        }

        public bool Connected
        {
            get { return client.GetStream().CanWrite; }
        }

        public Client()
        {
            // Connect to server
            client.Connect(endPoint);
            reader = new StreamReader(client.GetStream(), Encoding.ASCII);
            writer = new StreamWriter(client.GetStream(), Encoding.ASCII);

            // Runs the implementation in MyAI.cs
            RunAI();
        }

        public static void Main(string[] args)
        {
            new Client();
        }
        
    }
}
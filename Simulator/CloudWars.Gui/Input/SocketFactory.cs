using System.Linq;
using System.Threading;
using System.Windows.Controls;
using CloudWars.Core;
using CloudWars.Core.Settings;
using CloudWars.Graphics;
using CloudWars.Helpers;
using CloudWars.Network;

namespace CloudWars.Input
{
    public class SocketFactory
    {
        private readonly GameSettings settings;
        private readonly int maxClients;
        private SocketManager socketManager;
        public delegate void CancelEvent();
        public event CancelEvent Cancel;

        public SocketFactory(GameSettings settings)
        {
            this.settings = settings;
            maxClients = settings.Players.Count(p => p.Type == CloudType.AI);
        }


        public SocketManager CreateSocketManager()
        {
            if (maxClients <= 0)
                return null;

            socketManager = new SocketManager(maxClients, settings.Port);
            socketManager.Start();

            WindowThread<ProgressWindow> windowThread = new WindowThread<ProgressWindow>();
            windowThread.Update(p => p.Closed += (sender, args) => Cancel());
            windowThread.Start();

            while (socketManager.CountClients < maxClients)
            {
                windowThread.Update(p => p.StatusText.Text = string.Format("Waiting for {0} players..",
                                                                           maxClients -
                                                                           socketManager.CountClients));
                if (windowThread.IsClosed())
                {
                    socketManager.Stop();
                    Cancel();
                    break;
                }

                if (socketManager.HasIncoming)
                {
                    SocketHandler player = socketManager.ConnectPlayer();
                    windowThread.Update(p => p.StatusText.Text = "Waiting for name..");
                    string playerName = player.WaitForName();
                    windowThread.Update(p => p.List.Children.Add(new TextBlock { Text = playerName }));
                }
                Thread.Sleep(10);
            }
            windowThread.Stop();
            return socketManager;
        }
    }
}
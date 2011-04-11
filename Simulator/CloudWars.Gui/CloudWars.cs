using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CloudWars.Core;
using CloudWars.Core.Settings;
using CloudWars.Graphics;
using CloudWars.Helpers;
using CloudWars.Input;
using CloudWars.Network;

namespace CloudWars
{
    public class CloudWars : Application
    {
        private readonly GameSettings settings;
        private GameLoop gameLoop;
        private GameWindow gameWindow;
        public bool hasClosed;
        private SocketManager socketManager;
        private World world;

        public CloudWars()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Startup += (sender, args) =>
                           {
                               if ((settings.Players != null && settings.Players.Any()) || ShowSettingsWindow())
                                   Start();
                               else
                                   Shutdown();
                           };
        }


        public CloudWars(GameSettings settings) : this()
        {
            this.settings = settings;
        }

        public void Start()
        {
            hasClosed = false;
            gameLoop = new DispatcherTimerGameLoop(1000 / 100);
            gameLoop.Update += Update;

            gameWindow = new GameWindow(settings);
            gameWindow.Closed += (sender, args) => Stop();

            SocketFactory socketGame = new SocketFactory(settings);
            socketGame.Cancel += Stop;
            socketManager = socketGame.CreateSocketManager();

            GraphicManager graphicManager = new GraphicManager(gameWindow.Canvas);
            InputFactory inputFactory = new InputFactory(gameWindow.Canvas, socketManager);

            if (hasClosed)
                return;

            world = new World(settings, inputFactory, graphicManager);
            CompositionTarget.Rendering += (sender, args) => world.Draw();
            world.Start();
            gameWindow.Show();
            gameLoop.Start();
        }

        private void Update(TimeSpan elapsed)
        {
            // Update game logic
            world.Update(elapsed);

            if (world.IsFinished) Stop();
        }


        public void Stop()
        {
            hasClosed = true;
            if (socketManager != null)
                socketManager.Stop();

            gameLoop.Stop();

            if (gameWindow.IsEnabled)
                gameWindow.Close();

            if (ShowSettingsWindow())
                Start();
            else
                Shutdown();
        }


        private bool ShowSettingsWindow()
        {
            SettingsWindow settingsWindow = new SettingsWindow(settings);
            bool? result = settingsWindow.ShowDialog();
            return result != null && (bool) result;
        }


        [STAThreadAttribute]
        [DebuggerNonUserCode]
        public static void Main(string[] args)
        {
            CloudWars app = ExtractSettingsFromArgs(args);
            try
            {
                app.Run();
            }
            catch (Exception)
            {
                app.Shutdown();
            }
        }

        private static CloudWars ExtractSettingsFromArgs(IEnumerable<string> args)
        {
            GameSettings settings = new GameSettings
                                        {
                                            Port = 1986
                                        };
            List<NewPlayer> newPlayers = new List<NewPlayer>();
            if (args.Any(a => a.ToLower().Contains("ai")))
            {
                newPlayers.Add(new NewPlayer { Type = CloudType.AI });
            }
            if (args.Any(a => a.ToLower().Contains("human")))
            {
                newPlayers.Add(new NewPlayer
                                   {
                                       Name = "Human player",
                                       Type = CloudType.Human
                                   });
            }
            if (newPlayers.Any())
            {
                settings.Players = newPlayers;
            }
            return new CloudWars(settings);
        }
    }
}
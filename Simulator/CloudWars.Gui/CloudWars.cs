using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CloudWars.Core;
using CloudWars.Core.Settings;
using CloudWars.Graphics;

namespace CloudWars
{
    public class CloudWars : Application
    {
        private readonly GameSettings settings;
        private GameManager gameManager;

        public CloudWars()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Startup += OnStart;
        }


        public CloudWars(GameSettings settings) : this()
        {
            this.settings = settings;
        }

        private void OnStart(object sender, StartupEventArgs e)
        {
            try
            {
                if (settings.Players != null && !settings.Players.Any())
                {
                    bool start = ShowSettingsWindow();
                    if (!start)
                    {
                        Shutdown();
                        return;
                    }
                }
                StartGame();
            }
            catch (Exception)
            {
                Shutdown();
            }
        }

        private void StartGame()
        {
            gameManager = new GameManager(settings);
            gameManager.Close += OnGameManagerClose;
            gameManager.Start();
        }

        private bool ShowSettingsWindow()
        {
            //settings = new GameSettings();
            SettingsWindow settingsWindow = new SettingsWindow(settings);
            bool? result = settingsWindow.ShowDialog();
            return result != null && (bool) result;
        }

        private void OnGameManagerClose()
        {
            bool start = ShowSettingsWindow();
            if (start)
                StartGame();
            else
                Shutdown();
        }

        [STAThreadAttribute]
        [DebuggerNonUserCode]
        public static void Main(string[] args)
        {
            CloudWars app = ExtractSettingsFromArgs(args);
            app.Run();
        }

        private static CloudWars ExtractSettingsFromArgs(string[] args)
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
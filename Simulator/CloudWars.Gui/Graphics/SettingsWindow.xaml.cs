using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CloudWars.Core;
using CloudWars.Core.Settings;
using CloudWars.Helpers;

namespace CloudWars.Graphics
{
    /// <summary>
    ///   Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly GameSettings settings;

        public SettingsWindow(GameSettings settings)
        {
            this.settings = settings;
            KeyDown += SettingsWindow_KeyDown;
            InitializeComponent();
            //IList<Level> levels = GetLevels();
        }

        private IList<Level> GetLevels()
        {
            DirectoryInfo dir = new DirectoryInfo("Levels");
            IList<Level> levels = new List<Level>();
            foreach (FileInfo file in dir.GetFiles("*.lvl"))
            {
                using (StreamReader stream = file.OpenText())
                {
                    Level level = new Level(file.Name);
                    while (stream.Peek() >= 0)
                    {
                        string line = stream.ReadLine();
                        if (line == null) continue;
                        if (line.StartsWith("ITERATIONS"))
                        {
                            level.Iterations = int.Parse(line.Split(' ').Last());
                        }
                        else if (line.StartsWith("THUNDERSTORM"))
                        {
                            NewNpc cloud = new NewNpc();
                            string[] values = line.Remove("THUNDERSTORM").Split(new[] { ' ' },
                                                                                StringSplitOptions.RemoveEmptyEntries);
                            cloud.Position = new Vector(float.Parse(values[0], NumberFormatInfo.InvariantInfo),
                                                        float.Parse(values[1], NumberFormatInfo.InvariantInfo));
                            cloud.Velocity = new Vector(float.Parse(values[2], NumberFormatInfo.InvariantInfo),
                                                        float.Parse(values[3], NumberFormatInfo.InvariantInfo));
                            cloud.Vapor = float.Parse(values[4], NumberFormatInfo.InvariantInfo);
                            level.Thunderstorms.Add(cloud);
                        }
                        else if (line.StartsWith("RAINCLOUD"))
                        {
                            NewNpc cloud = new NewNpc();
                            string[] values = line.Remove("RAINCLOUD").Split(new[] { ' ' },
                                                                             StringSplitOptions.RemoveEmptyEntries);
                            cloud.Position = new Vector(float.Parse(values[0], NumberFormatInfo.InvariantInfo),
                                                        float.Parse(values[1], NumberFormatInfo.InvariantInfo));
                            cloud.Velocity = new Vector(float.Parse(values[2], NumberFormatInfo.InvariantInfo),
                                                        float.Parse(values[3], NumberFormatInfo.InvariantInfo));
                            cloud.Vapor = float.Parse(values[4], NumberFormatInfo.InvariantInfo);
                            level.Rainclouds.Add(cloud);
                        }
                    }
                    levels.Add(level);
                }
            }
            return levels;
        }

        private void SettingsWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            settings.Type = GameType.AiVsAi;
            settings.Players = new List<NewPlayer>
                                   {
                                       CreateAiPlayer(),
                                       CreateAiPlayer(),
                                   };
            Done();
        }

        private void Done()
        {
            settings.Fullscreen = fullscreen.IsChecked ?? true;
            DialogResult = true;
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            settings.Type = GameType.HumanVsAi;
            settings.Players = new List<NewPlayer>
                                   {
                                       CreateAiPlayer(),
                                       CreateHumanPlayer()
                                   };
            Done();
        }

        private static NewPlayer CreateHumanPlayer()
        {
            string name = "";
            return new NewPlayer
                       {
                           Name = name,
                           Type = CloudType.Human
                       };
        }

        private static NewPlayer CreateAiPlayer()
        {
            return new NewPlayer { Type = CloudType.AI };
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            settings.Type = GameType.Human;
            settings.Players = new List<NewPlayer>
                                   {
                                       CreateHumanPlayer()
                                   };
            Done();
        }
    }
}
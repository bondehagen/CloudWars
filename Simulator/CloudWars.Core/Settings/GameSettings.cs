using System.Collections.Generic;
using CloudWars.Core.Settings;

namespace CloudWars.Core
{
    public enum GameMode
    {
        Deathmatch,
        BecomeBig
    }

    public class GameSettings
    {
        public GameSettings()
        {
            GameMode = GameMode.BecomeBig;
            IterationLimit = 10000;
            Width = 1280;
            Height = 720;
            Players = new List<NewPlayer>();
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public GameType Type { get; set; }

        public bool Fullscreen { get; set; }

        public int IterationLimit { get; set; }

        public GameMode GameMode { get; set; }

        public IList<NewPlayer> Players { get; set; }

        public int Port { get; set; }
    }
}
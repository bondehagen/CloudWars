using System.Collections.Generic;

namespace CloudWars.Core.Settings
{
    public class Level
    {
        public Level(string name)
        {
            FileName = name;
            Rainclouds = new List<NewNpc>();
            Thunderstorms = new List<NewNpc>();
        }

        public string FileName { get; set; }

        public IList<NewNpc> Rainclouds { get; set; }

        public IList<NewNpc> Thunderstorms { get; set; }

        public int Iterations { get; set; }
    }
}
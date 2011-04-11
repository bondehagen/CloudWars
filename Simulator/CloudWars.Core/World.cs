using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CloudWars.Core.Graphic;
using CloudWars.Core.Input;
using CloudWars.Core.Settings;

namespace CloudWars.Core
{
    public class World
    {
        private static readonly Random Random = new Random();
        private readonly IGraphicManager graphicManager;
        private readonly IInputFactory inputFactory;
        private int iteration;

        public World(GameSettings settings, IInputFactory inputFactory, IGraphicManager graphicManager)
        {
            iteration = 0;
            this.inputFactory = inputFactory;
            this.graphicManager = graphicManager;
            RainClouds = new List<RainCloud>();
            Thunderstorms = new List<Thunderstorm>();
            Clouds = new List<Cloud>();
            Settings = settings;
        }

        public GameSettings Settings { get; private set; }

        public IList<Cloud> Clouds { get; set; }

        public IList<RainCloud> RainClouds { get; private set; }

        public IList<Thunderstorm> Thunderstorms { get; private set; }

        public bool IsFinished
        {
            get
            {
                switch (Settings.GameMode)
                {
                    case GameMode.BecomeBig:
                        return iteration >= Settings.IterationLimit;
                    case GameMode.Deathmatch:
                        return (iteration >= Settings.IterationLimit) || (Thunderstorms.Count <= 1);
                }
                throw new Exception("Invalid GameMode");
            }
        }

        public void AddRainCloud(Vector position, Vector velocity, float vapor)
        {
            RainCloud rainCloud = new RainCloud(this, vapor,
                                                graphicManager,
                                                ShapeType.RainCloud) { position = position, velocity = velocity };
            RainClouds.Add(rainCloud);
        }

        public void AddRainClouds(int amount)
        {
            const int initialSpeed = 20;
            for (int i = 0; i < amount; i++)
            {
                AddRainCloud(new Vector(Random.Next(Settings.Width), Random.Next(Settings.Height)),
                             new Vector(Random.NextDouble() * initialSpeed, Random.NextDouble() * initialSpeed),
                             Random.Next(600));
            }
        }

        public void AddThunderstorm(string name, CloudType type)
        {
            Thunderstorm thunderstorm = new Thunderstorm(this,
                                                         1000,
                                                         graphicManager,
                                                         ShapeType.ThunderStorm,
                                                         inputFactory.NewInputHandler(type))
                                            {
                                                velocity = new Vector(0, 0),
                                                position = new Vector(Random.Next(Settings.Width),
                                                                      Random.Next(Settings.Height))
                                            };
            Thunderstorms.Add(thunderstorm);
        }

        public void Update(TimeSpan timeSpan)
        {
            // Create cloud list by concatenating thunderstorms and rainclouds
            Clouds = Thunderstorms.Concat<Cloud>(RainClouds).ToList();

            // Update game logic
            foreach (Cloud cloud in Clouds)
                cloud.Update(iteration);

            // Remove dead thunderstorms
            for (int i = 0; i < Thunderstorms.Count; i++)
            {
                if (Thunderstorms[i].IsDead())
                    Thunderstorms.RemoveAt(i--);
            }
            // Remove dead rainclouds
            for (int i = 0; i < RainClouds.Count; i++)
            {
                if (RainClouds[i].IsDead())
                    RainClouds.RemoveAt(i--);
            }
            iteration++;
        }

        public void Start()
        {
            foreach (NewPlayer newPlayer in Settings.Players)
                AddThunderstorm(newPlayer.Name, newPlayer.Type);

            AddRainClouds(10);

            foreach (Thunderstorm thunderstorm in Thunderstorms)
                thunderstorm.Start();
        }

        public void Draw()
        {
            foreach (Cloud cloud in Clouds)
                cloud.Draw();
        }
    }
}
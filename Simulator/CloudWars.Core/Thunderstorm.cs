using System;
using System.Windows;
using CloudWars.Core.Graphic;
using CloudWars.Core.Input;

namespace CloudWars.Core
{
    public class Thunderstorm : Cloud
    {
        private readonly IInputHandler inputHandler;
        private object nameElement;

        public Thunderstorm(World world, float vapor, IGraphicManager graphicsHandler, ShapeType shapeType,
                            IInputHandler inputHandler) : base(world, vapor, graphicsHandler, shapeType)
        {
            this.inputHandler = inputHandler;
        }

        public string Name { get; set; }

        public override void Kill()
        {
            base.Kill();

            if (nameElement != null)
                graphicsHandler.RemoveShape(nameElement);

            nameElement = null;
            inputHandler.Dispose();
        }

        public override void Draw()
        {
            base.Draw();

            if (IsDead()) return;

            // Update textelement for name
            if (nameElement != null)
                graphicsHandler.UpdateShape(nameElement, position.X, position.Y, Radius);
        }

        public override void Update(int iterations)
        {
            inputHandler.Update(this, world, iterations);
            base.Update(iterations);
        }

        /// <summary>
        ///   This method is called by the input handler (socket or mouse) when the event queue
        ///   is processed on update. This method handles the WIND command.
        ///   Returns true if the wind command was OK, or false if IGNORED.
        /// </summary>
        /// <param name = "wind">The wind.</param>
        /// <returns></returns>
        public bool Wind(Vector wind)
        {
            // The strength of the wind is calculated as sqrt(x*x+y*y)
            float strength = (float) Math.Sqrt(wind.X * wind.X + wind.Y * wind.Y);

            // This value is not allowed to be less than 1 or greater than vapor / 2.
            // If this happens, the WIND command is ignored.
            if (strength < 1 || strength > vapor / 2) return false;

            // The vapor property of the thunderstorm will be reduced by strength
            vapor -= strength;

            // If the thunderstorm's amount of vapor goes below 1.0, the player dies and is removed from the
            // player list. The player's client can be immediately disconnected with no prior warning.
            if (vapor < 1.0f)
            {
                Kill();
                return false;
            }

            // The vector [(x / Radius) * 5, (y / Radius) * 5] is added to the velocity of the thunderstorm.
            velocity += (wind / Radius) * 5;

            // The vector [wx, wy] is calculated as [x / strength, y / strength].
            Vector w = wind / strength;

            // A new raincloud is spawned with vapor equal to strength.
            // The distance to spawn the new raincloud at is calculated as: 
            // (int)((storm_radius + raincloud_radius) * 1.1)
            float distance = (Radius + (float) Math.Sqrt(strength)) * 1.1f;

            // The position of the new raincloud is set to 
            // [(int)(px - wx * distance), (int)(py - wy * distance)]
            Vector rainPosition = new Vector(position.X - w.X * distance, position.Y - w.Y * distance);

            // with velocity
            // [-(x / strength) * 20 + vx, -(y / strength) * 20 + vy]
            Vector rainVelocity = new Vector(-(wind.X / strength) * 20 + velocity.X,
                                             -(wind.Y / strength) * 20 + velocity.Y);

            world.AddRainCloud(rainPosition, rainVelocity, strength);

            return true;
        }

        public void Start()
        {
            // Insert textelement
            if (!string.IsNullOrEmpty(Name))
            {
                nameElement = graphicsHandler.InsertText(Name);
                graphicsHandler.UpdateShape(nameElement, position.X, position.Y, Radius);
            }
            inputHandler.Start();
        }
    }
}
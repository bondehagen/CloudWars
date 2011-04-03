using System;
using System.Windows;
using CloudWars.Core.Graphic;

namespace CloudWars.Core
{
    public abstract class Cloud
    {
        protected static Random random;
        protected readonly IGraphicManager graphicsHandler;
        protected readonly World world;
        public Vector position;
        protected object shape;
        public float vapor;
        public Vector velocity;

        protected Cloud(World world, float vapor, IGraphicManager graphicsHandler, ShapeType shapeType)
        {
            this.world = world;
            this.vapor = vapor;
            this.graphicsHandler = graphicsHandler;
            velocity = new Vector();
            position = new Vector();
            shape = graphicsHandler.InsertShape(shapeType);
            random = new Random(DateTime.UtcNow.Millisecond);
        }

        public float Radius
        {
            get { return (float) Math.Sqrt(vapor); }
        }

        public virtual void Kill()
        {
            vapor = 0;
            if (shape != null) graphicsHandler.RemoveShape(shape);
            shape = null;
        }

        public bool Intersects(Cloud b)
        {
            return (b.position - position).Length < b.Radius + Radius;
        }

        public bool IsDead()
        {
            bool dead = vapor < 1;
            if (dead) Kill();
            return dead;
        }

        public virtual void Draw()
        {
            if (IsDead()) return;
            graphicsHandler.UpdateShape(shape, position.X, position.Y, Radius);
        }

        public virtual void Update(int iterations)
        {
            // Processing of input already done by subclass if applicable    

            // If we are dead return
            if (IsDead()) return;

            // Movement
            position += velocity * 0.1;

            // Damping of velocity
            velocity *= 0.999;

            // Absorbing vapor from others
            foreach (Cloud b in world.Clouds)
            {
                if (b == this) continue;

                Cloud smallest = Radius < b.Radius ? this : b;
                Cloud biggest = Radius > b.Radius ? this : b;

                // If the cloud have exactly the same amount of vapor, it is random (but not undefined) which one is considered the largest
                // with 50% probability for both A and B.
                if (Radius == b.Radius)
                {
                    if (random.NextDouble() < 0.5)
                    {
                        smallest = this;
                        biggest = b;
                    }
                    else
                    {
                        smallest = b;
                        biggest = this;
                    }
                }

                // Check for intersection
                while (Intersects(b))
                {
                    if (smallest.vapor < 1.0f)
                    {
                        smallest.Kill();
                        break;
                    }
                    // Transfer vapor from the biggest to the smallest
                    biggest.vapor += 1.0f;
                    smallest.vapor -= 1.0f;
                }
            }

            // Bounce against walls
            if (position.X < Radius)
            {
                position.X = Radius;
                velocity.X = Math.Abs(velocity.X) * 0.6;
            }
            if (position.Y < Radius)
            {
                position.Y = Radius;
                velocity.Y = Math.Abs(velocity.Y) * 0.6;
            }
            if (position.X + Radius > 1280)
            {
                position.X = world.Settings.Width - Radius;
                velocity.X = -Math.Abs(velocity.X) * 0.6;
            }
            if (position.Y + Radius > 720)
            {
                position.Y = world.Settings.Height - Radius;
                velocity.Y = -Math.Abs(velocity.Y) * 0.6;
            }
        }
    }
}
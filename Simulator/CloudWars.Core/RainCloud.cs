using CloudWars.Core.Graphic;

namespace CloudWars.Core
{
    public class RainCloud : Cloud
    {
        public RainCloud(World world, float vapor, IGraphicManager graphicsHandler, ShapeType shapeType)
            : base(world, vapor, graphicsHandler, shapeType) {}
    }
}
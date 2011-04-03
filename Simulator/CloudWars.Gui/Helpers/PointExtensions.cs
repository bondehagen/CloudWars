using System.Windows;

namespace CloudWars.Helpers
{
    public static class PointExtensions
    {
        public static Vector ToVector(this Point point)
        {
            return new Vector(point.X, point.Y);
        }
    }
}
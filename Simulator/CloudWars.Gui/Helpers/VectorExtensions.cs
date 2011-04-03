using System.Windows;

namespace CloudWars.Helpers
{
    public static class VectorExtensions
    {
        public static double Dot(this Vector source, Vector vector)
        {
            return source.X * vector.X + source.Y * vector.Y;
        }

        public static void Zero(this Vector source)
        {
            source.X = 0;
            source.Y = 0;
        }

        public static void Validate(this Vector source)
        {
            if (double.IsNaN(source.X + source.Y))
                source.Zero();
        }
    }
}
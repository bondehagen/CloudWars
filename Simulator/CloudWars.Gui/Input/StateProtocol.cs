using System.Globalization;
using System.Text;
using CloudWars.Core;
using CloudWars.Helpers;

namespace CloudWars.Input
{
    public static class StateProtocol
    {
        public static string Create(Thunderstorm player, World world, int ticks)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(beginStateLine(ticks));
            builder.AppendLine(youIndexLine(world, player));
            foreach (Thunderstorm cloud in world.Thunderstorms)
            {
                builder.AppendLine(cloudLine("THUNDERSTORM", cloud));
            }
            foreach (RainCloud cloud in world.RainClouds)
            {
                builder.AppendLine(cloudLine("RAINCLOUD", cloud));
            }
            builder.AppendLine(endStateLine());
            return builder.ToString();
        }

        private static string beginStateLine(int ticks)
        {
            return "BEGIN_STATE {0}".Format(ticks);
        }

        private static string youIndexLine(World world, Thunderstorm player)
        {
            return "YOU {0}".Format(world.Thunderstorms.IndexOf(player));
        }

        private static string endStateLine()
        {
            return "END_STATE";
        }

        private static string cloudLine(string cloudType, Cloud cloud)
        {
            string x = ToInvariantString(cloud.position.X);
            string y = ToInvariantString(cloud.position.Y);
            string vx = ToInvariantString(cloud.velocity.X);
            string vy = ToInvariantString(cloud.velocity.Y);
            string vapor = ToInvariantString(cloud.vapor);
            return string.Join(" ", cloudType, x, y, vx, vy, vapor);
        }

        private static string ToInvariantString(double d)
        {
            return ((float) d).ToString(NumberFormatInfo.InvariantInfo);
        }

        /*private static void AppendFScriptStreamStyle(this StringBuilder builder, string line)
        {
            builder.AppendFormat("{0}{1}\0\0\0{2}", (char) 3, (char) line.Length, line);
        }*/
    }
}
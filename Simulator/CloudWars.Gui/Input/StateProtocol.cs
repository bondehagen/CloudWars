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
                builder.AppendLine(cloudLine(cloud));
            }
            foreach (RainCloud cloud in world.RainClouds)
            {
                builder.AppendLine(cloudLine(cloud));
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

        private static string cloudLine(RainCloud cloud)
        {
            string x = ((float) cloud.position.X).ToString(NumberFormatInfo.InvariantInfo);
            string y = ((float) cloud.position.Y).ToString(NumberFormatInfo.InvariantInfo);
            string vx = ((float) cloud.velocity.X).ToString(NumberFormatInfo.InvariantInfo);
            string vy = ((float) cloud.velocity.Y).ToString(NumberFormatInfo.InvariantInfo);
            string vapor = cloud.vapor.ToString(NumberFormatInfo.InvariantInfo);
            return string.Format("RAINCLOUD {0} {1} {2} {3} {4}", x, y, vx, vy, vapor);
        }

        private static string cloudLine(Thunderstorm cloud)
        {
            string x = ((float) cloud.position.X).ToString(NumberFormatInfo.InvariantInfo);
            string y = ((float) cloud.position.Y).ToString(NumberFormatInfo.InvariantInfo);
            string vx = ((float) cloud.velocity.X).ToString(NumberFormatInfo.InvariantInfo);
            string vy = ((float) cloud.velocity.Y).ToString(NumberFormatInfo.InvariantInfo);
            string vapor = cloud.vapor.ToString(NumberFormatInfo.InvariantInfo);
            return string.Format("THUNDERSTORM {0} {1} {2} {3} {4}", x, y, vx, vy, vapor);
        }

        private static void AppendFScriptStreamStyle(this StringBuilder builder, string line)
        {
            builder.AppendFormat("{0}{1}\0\0\0{2}", (char) 3, (char) line.Length, line);
        }
    }
}
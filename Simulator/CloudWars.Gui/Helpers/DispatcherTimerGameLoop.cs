using System;
using System.Windows.Threading;

namespace CloudWars.Helpers
{
    public class DispatcherTimerGameLoop : GameLoop
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public DispatcherTimerGameLoop() : this(0) {}

        public DispatcherTimerGameLoop(double milliseconds)
        {
            timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            timer.Tick += Tick;
            timer.Start();
        }

        public override void Start()
        {
            timer.Start();
            base.Start();
        }

        public override void Stop()
        {
            timer.Stop();
            base.Stop();
        }

        private void Tick(object sender, EventArgs e)
        {
            base.Tick();
        }
    }
}
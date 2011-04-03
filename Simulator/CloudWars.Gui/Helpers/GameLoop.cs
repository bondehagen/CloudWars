using System;

namespace CloudWars.Helpers
{
    public abstract class GameLoop
    {
        #region Delegates

        public delegate void UpdateHandler(TimeSpan elapsed);

        #endregion

        protected DateTime lastTick;

        public event UpdateHandler Update;

        public void Tick()
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - lastTick;
            lastTick = now;
            if (Update != null)
                Update(elapsed);
        }

        public virtual void Start()
        {
            lastTick = DateTime.Now;
        }

        public virtual void Stop() {}
    }
}
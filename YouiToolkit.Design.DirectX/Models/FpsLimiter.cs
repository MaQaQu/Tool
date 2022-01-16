using System;

namespace YouiToolkit.Design
{
    public class FpsLimiter
    {
        public const int _15Hz = 1000 / 15;
        public const int _20Hz = 1000 / 20;
        public const int _24Hz = 1000 / 24;
        public const int _30Hz = 1000 / 30;
        public const int _45Hz = 1000 / 45;
        public const int _60Hz = 1000 / 60;
         
        protected int interval = FpsThread._30Hz;
        protected DateTime emittable = DateTime.MinValue;

        public FpsLimiter(int interval)
        {
            this.interval = interval;
        }

        public bool Callabled()
        {
            DateTime curr = DateTime.Now;
            if (curr >= emittable)
            {
                emittable = curr + new TimeSpan(interval * TimeSpan.TicksPerMillisecond);
                return true;
            }
            return false;
        }
    }
}

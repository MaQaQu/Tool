using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace YouiToolkit.Design
{
    internal class FpsThread
    {
        public const int _15Hz = 1000 / 15;
        public const int _20Hz = 1000 / 20;
        public const int _24Hz = 1000 / 24;
        public const int _30Hz = 1000 / 30;
        public const int _60Hz = 1000 / 60;

        protected Thread thread = null;
        protected ThreadStart cyclicMethod = null;
        private Stopwatch stopwatch = null;

        public bool Running { get; set; } = false;
        public bool Paused { get; set; } = false;

        public int interval = default;
        public int Interval
        {
            get => interval;
            set
            {
                if (interval != value)
                {
                    thread.Name = thread.Name.Replace($"FpsThread ({interval}ms)", $"FpsThread ({value}ms)");
                    interval = value;
                }
            }
        }

        public FpsThread(ThreadStart cyclicMethod, int interval = _30Hz, string name = null)
        {
            this.interval = interval;
            this.cyclicMethod = cyclicMethod;
            thread = new Thread(ThreadStart);
            thread.IsBackground = true;
            thread.Name = $"{name}.FpsThread ({interval}ms)";
            stopwatch = new Stopwatch();
        }

        public void Start()
        {
            Running = true;
            thread.Start();
        }

        public void Stop()
        {
            Running = false;
        }

        public void Pause() => Paused = true;
        public void Resume() => Paused = false;

        public void ThreadStart()
        {
            while (Running)
            {
                if (!Paused)
                {
                    stopwatch.Restart();
                    cyclicMethod?.Invoke();
                    stopwatch.Stop();
                }
                Thread.Sleep(Math.Max(default, interval - (int)stopwatch.ElapsedMilliseconds));
            }
        }
    }
}

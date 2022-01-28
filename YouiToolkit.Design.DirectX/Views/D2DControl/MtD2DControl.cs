using SharpDX.Direct2D1;
using System;
using System.Windows.Threading;

namespace YouiToolkit.Design
{
    public class MtD2DControl : MtD2DContentControl
    {
        protected RenderTarget D2DRenderTarget => deviceContext1;
        protected Factory D2DFactory => deviceContext1.Factory;

        protected override void Resized()
        {
            base.Resized();
        }

        public static void DelayInvoke(Action action, double milliseconds = 10)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                action?.Invoke();
            };
            timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            timer.Start();
        }
    }
}

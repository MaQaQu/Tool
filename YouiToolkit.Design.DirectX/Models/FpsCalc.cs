using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace YouiToolkit.Design
{
    public class FpsCalc : DependencyObject
	{
		public int Fps
		{
			get => (int)GetValue(FpsProperty);
			set => SetValue(FpsProperty, value);
		}

		public static readonly DependencyProperty FpsProperty = DependencyProperty.Register("Fps", typeof(int), typeof(FpsCalc), new PropertyMetadata(0));

		public bool Enable { get; set; } = true;

		protected long lastFrameTime = 0;
		protected long lastRenderTime = 0;
		protected int frameCount = 0;
		protected int frameCountHistTotal = 0;
		protected Queue<int> frameCountHist = new Queue<int>();

		protected readonly Stopwatch renderTimer = new Stopwatch();
		public Stopwatch RenderTimer => renderTimer;

		public void Start() => renderTimer.Start();
		public void Stop() => renderTimer.Stop();
		public void UpdateFps() => lastRenderTime = renderTimer.ElapsedMilliseconds;

		public void CalcFps()
		{
			if (!Enable) return;

			frameCount++;
			if (renderTimer.ElapsedMilliseconds - lastFrameTime > 1000)
			{
				frameCountHist.Enqueue(frameCount);
				frameCountHistTotal += frameCount;
				if (frameCountHist.Count > 5)
				{
					frameCountHistTotal -= frameCountHist.Dequeue();
				}

				Fps = frameCountHistTotal / frameCountHist.Count;

				frameCount = 0;
				lastFrameTime = renderTimer.ElapsedMilliseconds;
			}
		}
	}
}

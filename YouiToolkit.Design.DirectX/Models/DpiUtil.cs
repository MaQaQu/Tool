using System;
using System.Drawing;

namespace YouiToolkit.Design
{
    internal static class DpiUtil
	{
		public static double CalcDpi(double src)
		{
			using var graphics = Graphics.FromHwnd(IntPtr.Zero);
			double dpiScale = graphics.DpiX / 96d;
			return Math.Ceiling(src * dpiScale);
		}
	}
}

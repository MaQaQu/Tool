using SharpDX.Direct2D1;
using System;

namespace YouiToolkit.Design.DirectX
{
    public class DirectX2DPaintEventArgs : EventArgs
    {
        public RenderTarget RenderTarget { get; set; } = null;

        public Factory Factory { get; set; } = null;
    }
}

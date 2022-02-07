using System;
using System.Windows;
using System.Windows.Input;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;

namespace YouiToolkit.Views
{
    internal class MtMapImage : D2DBitmap_Mt
    {
        public event EventHandler ContextChanged;

        public MtMapImageRenderContext Context { get; set; }
        public MtMapImageRender Render { get; set; }

        public MtMapImage()
        {
        }

        protected override void Resized()
        {
            base.Resized();
            ResetOrigin();
        }

        protected override void ActualLoaded()
        {
            ResetOrigin();
        }

        public void ResetOrigin()
        {
            Context?.ResetOrigin((float)ActualWidth / 2f, (float)ActualHeight / 2f);
        }

        private void InitEvent()
        {
            LostFocus += (s, e) => Context.OnLostFocus();
            MouseMove += (s, e) => Context.OnMouseMove(Mouse.GetPosition(e.Source as FrameworkElement));
            MouseEnter += (s, e) => Context.OnMouseEnter(default);
            MouseLeave += (s, e) => Context.OnMouseLeave(default);
            MouseDown += (s, e) =>
            {
                Context.OnMouseDown(Mouse.GetPosition(e.Source as FrameworkElement),
                    e.LeftButton == MouseButtonState.Pressed,
                    e.MiddleButton == MouseButtonState.Pressed,
                    e.RightButton == MouseButtonState.Pressed,
                    e.ClickCount);
            };
            MouseUp += (s, e) =>
            {
                Context.OnMouseUp(Mouse.GetPosition(e.Source as FrameworkElement),
                    e.LeftButton == MouseButtonState.Released,
                    e.MiddleButton == MouseButtonState.Released,
                    e.RightButton == MouseButtonState.Released);
            };
            MouseWheel += (s, e) => Context.OnMouseWheel(Mouse.GetPosition(e.Source as FrameworkElement), e.Delta);
            SizeChanged += (s, e) => Context.SetPanelSize((float)ActualWidth, (float)ActualWidth);
            KeyDown += (s, e) => Context.OnKeyDown(e.Key, e.KeyboardDevice.Modifiers);
            KeyUp += (s, e) => Context.OnKeyUp(e.Key, e.KeyboardDevice.Modifiers);
        }

        public double CalcDpi(double src)
        {
            using var graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            double dpiScale = graphics.DpiX / 96d;
            return Math.Ceiling(src * dpiScale);
        }

        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            if (Context == null)
            {
                Context = new MtMapImageRenderContext(this, D2DRenderTarget, D2DFactory);
                Context.FlipOriginScaleY();
                ResetOrigin();
                Render = new MtMapImageRender(this, Context);
                ContextChanged?.Invoke(this, EventArgs.Empty);
                InitEvent();
            }
            else
            {
                Context.InitDirectX(D2DRenderTarget, D2DFactory);
                Render.Context = Context;
            }
        }

        protected override void OnTargetRender()
        {
            Render.Render();
        }
    }
}

using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Windows;
using System.Windows.Input;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;

namespace YouiToolkit.Views
{
    internal class MtMapRenderControl : MtD2DControl
    {
        public event EventHandler ContextChanged;

        public MtMapRenderContext Context { get; set; }
        public MtMapRender Renderer { get; set; }

        public MtMapRenderControl()
        {
        }

        public void ResetOrigin()
        {
            Context?.ResetOrigin((float)ActualWidthDpi / 2f, (float)ActualHeightDpi / 2f);
        }

        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            if (Context == null)
            {
                Context = new MtMapRenderContext(this, D2DRenderTarget, D2DFactory);
                Context.FlipOriginScaleY();
                ResetOrigin();
                Renderer = new MtMapRender(this, Context);
                ContextChanged?.Invoke(this, EventArgs.Empty);
                InitEvent();
            }
            else
            {
                Context.InitDirectX(D2DRenderTarget, D2DFactory);
                Renderer.Context = Context;
            }
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            Context.SetPanelSize((float)ActualWidthDpi, (float)ActualHeightDpi);
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
        }

        private void InitEvent()
        {
            LostFocus += (s, e) => Context?.OnLostFocus();
            MouseMove += (s, e) => Context?.OnMouseMove(MousePosition);
            MouseEnter += (s, e) => Context?.OnMouseEnter(MousePosition);
            MouseLeave += (s, e) => Context?.OnMouseLeave(MousePosition);
            MouseDown += (s, e) =>
            {
                if (e is MouseButtonProcEventArgs ex)
                {
                    Context?.OnMouseDown(MousePosition,
                        ex.LeftButton == MouseButtonState.Pressed,
                        ex.MiddleButton == MouseButtonState.Pressed,
                        ex.RightButton == MouseButtonState.Pressed,
                        ex.ClickCount);
                }
            };
            MouseUp += (s, e) =>
            {
                if (e is MouseButtonProcEventArgs ex)
                {
                    Context?.OnMouseUp(MousePosition,
                        ex.LeftButton == MouseButtonState.Released,
                        ex.MiddleButton == MouseButtonState.Released,
                        ex.RightButton == MouseButtonState.Released);
                }
            };
            MouseWheel += (s, e) => Context?.OnMouseWheel(MousePosition, e.Delta);
            SizeChanged += (s, e) =>
            {
                Context?.SetPanelSize((float)ActualWidthDpi, (float)ActualHeightDpi);
            };
        }

        protected override void OnTargetRender1()
        {
            Renderer.Render();
        }
    }
}

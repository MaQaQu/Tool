using System;
using System.Windows;
using System.Windows.Input;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;

namespace YouiToolkit.Views
{
    internal class AvoidObstacleImage : D2DBitmap
    {
        public event EventHandler ContextChanged;

        public AvoidObstacleRenderContext Context { get; set; }
        public AvoidObstacleRenderer Renderer { get; set; }

        public AvoidObstacleImage()
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
            SizeChanged += (s, e) => Context.SetPanelSize((float)ActualWidth, (float)ActualHeight);
            KeyDown += (s, e) => Context.OnKeyDown(e.Key, e.KeyboardDevice.Modifiers);
            KeyUp += (s, e) => Context.OnKeyUp(e.Key, e.KeyboardDevice.Modifiers);
        }

        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            if (Context == null)
            {
                Context = new AvoidObstacleRenderContext(this, D2DRenderTarget, D2DFactory);
                Context.FlipOriginScaleY();
                ResetOrigin();
                Renderer = new AvoidObstacleRenderer(this, Context);
                ContextChanged?.Invoke(this, EventArgs.Empty);
                InitEvent();
            }
            else
            {
                Context.InitDirectX(D2DRenderTarget, D2DFactory);
                Renderer.Context = Context;
            }
        }

        protected override void OnTargetRender()
        {
            Renderer.Render();
        }
    }
}

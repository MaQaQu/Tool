using System.Windows;
using System.Windows.Input;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;

namespace YouiToolkit.Views
{
    internal class MapRenderImage : D2DImage
    {
        public MapRenderContext Context { get; set; }
        public MapRenderer Renderer { get; set; }

        public MapRenderImage()
        {
            InitEvent();
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
        }

        /// <summary>
        /// 创建DirectX资源
        /// </summary>
        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            if (Context == null)
            {
                Context = new MapRenderContext(this, D2DRenderTarget, D2DFactory);
                Context.FlipOriginScaleY();
                ResetOrigin();
                Renderer = new MapRenderer(this, Context);
            }
            else
            {
                Context.D2DRenderTarget = D2DRenderTarget;
                Context.D2DFactory = D2DFactory;
            }
        }

        /// <summary>
        /// 释放DirectX资源
        /// </summary>
        protected override void InternalUninitialize()
        {
            base.InternalUninitialize();
        }

        protected override void OnTargetRender()
        {
            Renderer.Render();
        }
    }
}

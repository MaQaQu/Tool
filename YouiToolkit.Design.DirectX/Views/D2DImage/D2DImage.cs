using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using Image = System.Windows.Controls.Image;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace YouiToolkit.Design
{
    public abstract class D2DImage : Image
    {
        protected Device device;
        protected Texture2D renderTarget;
        protected D3DImageSource d3DSurface;

        protected Factory d2DFactory;
        protected Factory D2DFactory => d2DFactory;

        protected RenderTarget d2DRenderTarget;
        protected RenderTarget D2DRenderTarget => d2DRenderTarget;

        public FpsCalc FpsCalc { get; private set; } = new FpsCalc();

        private bool isActualLoaded = true;

        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }

        public D2DImage()
        {
            Loaded += Window_Loaded;
            Unloaded += Window_Closing;

            SizeChanged += (s, e) => Resized();

            Stretch = Stretch.Fill;
            Focusable = true;
            FocusVisualStyle = null;
            MouseLeftButtonDown += (s, e) => Focus();
        }

        protected virtual void ActualLoaded()
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
            {
                return;
            }

            StartD3D();
            StartRendering();
        }

        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
            {
                return;
            }

            StopRendering();
            EndD3D();
        }

        /// <summary>
        /// 创建DirectX资源
        /// </summary>
        protected virtual void InternalInitialize()
        {
        }

        /// <summary>
        /// 释放DirectX资源
        /// </summary>
        protected virtual void InternalUninitialize()
        {
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!FpsCalc.RenderTimer.IsRunning)
            {
                return;
            }

            PrepareAndCallRender();
            d3DSurface.InvalidateD3DImage();
            FpsCalc.UpdateFps();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3DSurface.IsFrontBufferAvailable)
            {
                StartRendering();
            }
            else
            {
                StopRendering();
            }
        }

        private void StartD3D()
        {
            device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            d3DSurface = new D3DImageSource();
            d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            base.Source = d3DSurface;
        }

        private void EndD3D()
        {
            d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            base.Source = null;

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d2DFactory);
            Disposer.SafeDispose(ref d3DSurface);
            Disposer.SafeDispose(ref renderTarget);
            Disposer.SafeDispose(ref device);

            InternalUninitialize();
        }

        public void CreateAndBindTargets()
        {
            if (d3DSurface == null)
            {
                return;
            }

            d3DSurface.SetRenderTarget(null);

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d2DFactory);
            Disposer.SafeDispose(ref renderTarget);

            var width = Math.Max((int)ActualWidth, 100);
            var height = Math.Max((int)ActualHeight, 100);

            var renderDesc = new Texture2DDescription()
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            renderTarget = new Texture2D(device, renderDesc);

            var surface = renderTarget.QueryInterface<Surface>();

            d2DFactory = new Factory();
            var rtp = new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied));
            
            d2DRenderTarget = new RenderTarget(d2DFactory, surface, rtp);

            d3DSurface.SetRenderTarget(renderTarget);

            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);

            InternalInitialize();
        }

        protected virtual void Resized()
        {
            if (isActualLoaded
             && !ActualHeight.Equals(double.NaN)
             && !ActualWidth.Equals(double.NaN)
             && ActualHeight != default
             && ActualWidth != default)
            {
                isActualLoaded = false;
                ActualLoaded();
            }

            InternalUninitialize();
            InternalInitialize();
        }

        private void StartRendering()
        {
            if (FpsCalc.RenderTimer.IsRunning)
            {
                return;
            }

            CompositionTarget.Rendering += OnRendering;
            FpsCalc.Start();
        }

        private void StopRendering()
        {
            if (!FpsCalc.RenderTimer.IsRunning)
            {
                return;
            }

            CompositionTarget.Rendering -= OnRendering;
            FpsCalc.Stop();
        }

        public bool IsPaused { get; private set; } = false;
        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;

        private void PrepareAndCallRender()
        {
            if (IsPaused)
            {
                return;
            }
            if (device == null)
            {
                return;
            }

            d2DRenderTarget.BeginDraw();
            OnTargetRender();
            d2DRenderTarget.EndDraw();

            FpsCalc.CalcFps();

            device.ImmediateContext.Flush();
        }

        protected abstract void OnTargetRender();
    }
}

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Windows;
using System.Windows.Media;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace YouiToolkit.Design
{
    /// <summary>
    /// DirectX交换链组件
    /// </summary>
    public abstract class DirectXComponent : Win32HwndControl
	{
		private Device device;
		protected Device Device => device;

		private SwapChain swapChain;
		protected SwapChain SwapChain => swapChain;

		private Texture2D backBuffer;
		protected Texture2D BackBuffer => backBuffer;

		private RenderTargetView renderTargetView;
		protected RenderTargetView RenderTargetView => renderTargetView;

		protected int SurfaceWidth { get; private set; }
		protected int SurfaceHeight { get; private set; }

		public bool Rendering { get; private set; }

		public FpsCalc FpsCalc { get; private set; } = new FpsCalc();

		protected DirectXComponent()
		{
		}

		protected override sealed void Initialize()
		{
			InternalInitialize();

			Rendering = true;
			CompositionTarget.Rendering += OnCompositionTargetRendering;

			FpsCalc.Start();
		}

		protected override sealed void Uninitialize()
		{
			Rendering = false;
			CompositionTarget.Rendering -= OnCompositionTargetRendering;

			InternalUninitialize();
		}

        protected sealed override void Resized()
        {
 	        InternalUninitialize();
            InternalInitialize();
        }

		public bool IsPaused { get; set; } = false;
		public void Pause() => IsPaused = true;
		public void Resume() => IsPaused = false;

		private void OnCompositionTargetRendering(object sender, EventArgs eventArgs)
		{
			if (IsPaused)
			{
				return;
			}
			if (!Rendering)
			{
				return;
			}

			try
			{
				BeginRender();
				OnTargetRender();
				EndRender();
			}
			catch (SharpDXException e)
			{
				if (e.HResult == HResults.D2DERR_RECREATE_TARGET || e.HResult == HResults.DXGI_ERROR_DEVICE_REMOVED)
				{
					Uninitialize();
					Initialize();
				}
				else throw;
			}
		}

		/// <summary>
		/// 创建DirectX资源
		/// </summary>
		protected virtual void InternalInitialize()
		{
			SurfaceWidth = (int)DpiUtil.CalcDpi(ActualWidth);
			SurfaceHeight = (int)DpiUtil.CalcDpi(ActualHeight);

			var swapChainDescription = new SwapChainDescription()
			{
				OutputHandle = Hwnd,
				BufferCount = 1,
				Flags = SwapChainFlags.AllowModeSwitch,
				IsWindowed = true,
				ModeDescription = new ModeDescription(SurfaceWidth, SurfaceHeight, new Rational(60, 1), Format.B8G8R8A8_UNorm),
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput | Usage.Shared,
			};

			Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out device, out swapChain);

			// Ignore all windows events
			using var factory = swapChain.GetParent<Factory>();
			factory.MakeWindowAssociation(Hwnd, WindowAssociationFlags.IgnoreAll);

			// New RenderTargetView from the backbuffer
			backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
			renderTargetView = new RenderTargetView(device, backBuffer);
		}

		/// <summary>
		/// 释放DirectX资源
		/// </summary>
		protected virtual void InternalUninitialize()
		{
			Utilities.Dispose(ref renderTargetView);
			Utilities.Dispose(ref backBuffer);
			Utilities.Dispose(ref swapChain);

			((IUnknown)device).Release();
			Utilities.Dispose(ref device);

			GC.Collect(2, GCCollectionMode.Forced);
		}

		/// <summary>
		/// 开始渲染
		/// </summary>
		protected virtual void BeginRender()
		{
			device.ImmediateContext.Rasterizer.SetViewport(0, 0, (float)ActualWidth, (float)ActualHeight);
			device.ImmediateContext.OutputMerger.SetRenderTargets(renderTargetView);
		}

		/// <summary>
		/// 结束渲染
		/// </summary>
		protected virtual void EndRender()
		{
			FpsCalc.CalcFps();
			swapChain.Present(1, PresentFlags.None);
			FpsCalc.UpdateFps();
		}

		/// <summary>
		/// 渲染
		/// </summary>
		protected abstract void OnTargetRender();
	}
}

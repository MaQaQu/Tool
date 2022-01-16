using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using YouiToolkit.Logging;
using Color = SharpDX.Color;
using D2D1AlphaMode = SharpDX.Direct2D1.AlphaMode;
using D2D1Device = SharpDX.Direct2D1.Device;
using D2D1PixelFormat = SharpDX.Direct2D1.PixelFormat;
using D3D11Device = SharpDX.Direct3D11.Device;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;
using DXGIDevice = SharpDX.DXGI.Device;
using DXGIFactory = SharpDX.DXGI.Factory;

namespace YouiToolkit.Design
{
    public class D2DContentControl : Win32HwndControl
	{
		public Color ClearDirtyColor { get; set; } = Color.White;

		protected int SurfaceWidth { get; private set; }
		protected int SurfaceHeight { get; private set; }

		public bool Rendering { get; private set; }

		public FpsCalc FpsCalc { get; private set; } = new FpsCalc();

		internal FpsThread FpsThread { get; private set; }
		internal FpsLimiter FpsLimiter = new FpsLimiter(FpsLimiter._30Hz);

		/// <summary>
		/// Direct2D 设备上下文
		/// </summary>
		protected DeviceContext deviceContext;
		protected Bitmap1 targetBitmap;
		protected bool dirtyClear = false;

		/// <summary>
		/// Direct2D 多线程设备上下文
		/// </summary>
		protected DeviceContext deviceContext1;
		protected ConcurrentQueue<Bitmap1> bitmapQueue = new ConcurrentQueue<Bitmap1>();

		protected static readonly D2D1PixelFormat PixelFormat = new D2D1PixelFormat(Format.B8G8R8A8_UNorm, D2D1AlphaMode.Premultiplied);
		protected static BitmapProperties1 BitmapProps = new BitmapProperties1(PixelFormat, 96, 96, BitmapOptions.Target);

		protected DeviceContext deviceContext2;

		/// <summary>
		/// DXGI SwapChain
		/// </summary>
		protected SwapChain swapChain;

		/// <summary>
		/// SwapChain 缓冲区
		/// </summary>
		protected Surface backBuffer;

		public D2DContentControl()
        {
		}

        protected override void Initialize()
		{
			InternalInitialize();

			FpsThread = new FpsThread(OnFpsThreadTargetRendering, FpsThread._15Hz, "D2DBitmap1");
			FpsThread.Start();

			Rendering = true;
			CompositionTarget.Rendering += OnCompositionTargetRendering;

			FpsCalc.Start();
		}

        protected override void Uninitialize()
		{
			FpsThread.Stop();

			Rendering = false;
			CompositionTarget.Rendering -= OnCompositionTargetRendering;

			InternalUninitialize();
		}

		protected virtual void ClearDirty(Bitmap1 bitmap = null)
		{
			deviceContext.BeginDraw();
			if (bitmap == null)
			{
				deviceContext.Clear(ClearDirtyColor);
			}
			else
			{
				deviceContext.Clear(ClearDirtyColor);
				deviceContext.DrawBitmap(bitmap, 1f, BitmapInterpolationMode.Linear);
			}
			deviceContext.EndDraw();
			swapChain.Present(1, PresentFlags.None);
		}

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
				if (FpsLimiter.Callabled())
				{
					deviceContext.BeginDraw();
					OnTargetRender();
					deviceContext.EndDraw();
					FpsCalc.CalcFps();
					swapChain.Present(1, PresentFlags.None);
					FpsCalc.UpdateFps();
				}
			}
			catch (SharpDXException e)
			{
				if (e.HResult == HResults.D2DERR_RECREATE_TARGET || e.HResult == HResults.DXGI_ERROR_DEVICE_REMOVED)
				{
					Uninitialize();
					Initialize();
				}
				else
				{
					Logger.Fatal(e.ToString());
					throw;
				}
			}
		}

		protected virtual void OnFpsThreadTargetRendering()
		{
			try
			{
				if (deviceContext1 == null)
					return;

				if (bitmapQueue.IsEmpty)
				{
					Bitmap1 targetBitmap1 = new Bitmap1(deviceContext1, new Size2(SurfaceWidth, SurfaceHeight), BitmapProps);
					deviceContext1.Target = targetBitmap1;

					deviceContext1.BeginDraw();
					OnTargetRender1();
					deviceContext1.EndDraw();

#if false
					Bitmap1 targetBitmapTemp = new Bitmap1(deviceContext1, new Size2(SurfaceWidth, SurfaceHeight), BitmapProps);
					targetBitmapTemp.CopyFromBitmap(targetBitmap1);
					bitmapQueue.Enqueue(targetBitmapTemp);
#endif
                    bitmapQueue.Enqueue(targetBitmap1);
				}
            }
			catch (SharpDXException e)
			{
				if (e.HResult == HResults.D2DERR_RECREATE_TARGET || e.HResult == HResults.DXGI_ERROR_DEVICE_REMOVED)
				{
					Uninitialize();
					Initialize();
				}
				else
				{
					Logger.Fatal(e.ToString());
					throw;
				}
			}
		}

		protected override void Resized()
		{
            if (deviceContext != null)
			{
				SurfaceWidth = (int)DpiUtil.CalcDpi(ActualWidth);
				SurfaceHeight = (int)DpiUtil.CalcDpi(ActualHeight);

				using Bitmap1 targetBitmap2 = new Bitmap1(deviceContext2, new Size2(SurfaceWidth, SurfaceHeight), BitmapProps);
				targetBitmap2.CopyFromBitmap(targetBitmap);

				deviceContext.Target = null;
                backBuffer?.Dispose();
                targetBitmap?.Dispose();

                swapChain?.ResizeBuffers(1, SurfaceWidth, SurfaceHeight, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
                backBuffer = Surface.FromSwapChain(swapChain, 0);
                targetBitmap = new Bitmap1(deviceContext, backBuffer);
                deviceContext.Target = targetBitmap;

				ClearDirty(targetBitmap2);
			}

            GC.Collect(2, GCCollectionMode.Forced);
        }

		public bool isPaused = false;
		public bool IsPaused
		{
			get => isPaused;
			set
			{
				isPaused = value;
				if (FpsThread != null)
					FpsThread.Paused = value;
			}
		}
		public void Pause() => IsPaused = true;
		public void Resume() => IsPaused = false;

		protected virtual void InternalInitialize()
		{
			SurfaceWidth = (int)DpiUtil.CalcDpi(ActualWidth);
			SurfaceHeight = (int)DpiUtil.CalcDpi(ActualHeight);

			// 创建 Dierect3D 设备
			D3D11Device d3DDevice = new D3D11Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
			DXGIDevice dxgiDevice = d3DDevice.QueryInterface<D3D11Device>().QueryInterface<DXGIDevice>();
			
			// 创建 Direct2D 设备和工厂
			D2D1Device d2DDevice = new D2D1Device(dxgiDevice);
			deviceContext = new DeviceContext(d2DDevice, DeviceContextOptions.None);
			deviceContext1 = new DeviceContext(d2DDevice, DeviceContextOptions.None);
			deviceContext2 = new DeviceContext(d2DDevice, DeviceContextOptions.None);

			// 创建 DXGI SwapChain
			var swapChainDesc = new SwapChainDescription()
			{
				OutputHandle = Hwnd,
				BufferCount = 1,
				Flags = SwapChainFlags.AllowModeSwitch,
				IsWindowed = true,
				ModeDescription = new ModeDescription(SurfaceWidth, SurfaceHeight, new Rational(60, 1), Format.B8G8R8A8_UNorm),
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput,
			};
			swapChain = new SwapChain(dxgiDevice.GetParent<Adapter>().GetParent<DXGIFactory>(), d3DDevice, swapChainDesc);
			backBuffer = Surface.FromSwapChain(swapChain, 0);

			// Ignore all windows events
			//using var factory = swapChain.GetParent<DXGIFactory>();
			//factory.MakeWindowAssociation(Hwnd, WindowAssociationFlags.IgnoreAll);

			targetBitmap = new Bitmap1(deviceContext, backBuffer);
			deviceContext.Target = targetBitmap;
		}

		protected virtual void InternalUninitialize()
		{
			if (deviceContext != null)
			{
				deviceContext.Target = null;
				Utilities.Dispose(ref backBuffer);
                Utilities.Dispose(ref targetBitmap);
                Utilities.Dispose(ref swapChain);
                Utilities.Dispose(ref deviceContext);
				Utilities.Dispose(ref deviceContext1);
				Utilities.Dispose(ref deviceContext2);
			}

			GC.Collect(2, GCCollectionMode.Forced);
		}

		/// <summary>
		/// 渲染
		/// </summary>
		protected virtual void OnTargetRender()
		{
			try
			{
				if (bitmapQueue.TryDequeue(out Bitmap1 targetBitmap))
				{
					if (targetBitmap != null)
						deviceContext.DrawBitmap(targetBitmap, 1f, BitmapInterpolationMode.Linear);
					targetBitmap?.Dispose();
				}
			}
			catch (Exception e)
			{
				Logger.Fatal(e.ToString());
			}
		}

		/// <summary>
		/// 渲染
		/// </summary>
		protected virtual void OnTargetRender1()
		{
		}
	}
}

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;
using WriteFactory = SharpDX.DirectWrite.Factory;

namespace YouiToolkit.Design
{
    /// <summary>
    /// Direct2D渲染组件
    /// </summary>
    public abstract class D2DComponent : DirectXComponent
	{
		protected Factory d2DFactory;
		protected Factory D2DFactory => d2DFactory;

		protected RenderTarget d2DRenderTarget;
		protected RenderTarget D2DRenderTarget => d2DRenderTarget;

		protected override void InternalInitialize()
		{
			base.InternalInitialize();

			d2DFactory = new Factory(FactoryType.MultiThreaded);

			using var surface = BackBuffer.QueryInterface<Surface>();
			d2DRenderTarget = new RenderTarget(d2DFactory, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Ignore)));
			d2DRenderTarget.AntialiasMode = AntialiasMode.Aliased;
		}

		protected override void InternalUninitialize()
		{
			Utilities.Dispose(ref d2DRenderTarget);
			Utilities.Dispose(ref d2DFactory);

			base.InternalUninitialize();
		}

		protected override void BeginRender()
		{
			base.BeginRender();

			d2DRenderTarget.BeginDraw();
		}

		protected override void EndRender()
		{
			d2DRenderTarget.EndDraw();

			base.EndRender();
		}
	}
}

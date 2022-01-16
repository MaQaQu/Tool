//using SharpDX;
//using SharpDX.Direct2D1;
//using SharpDX.DXGI;
//using SharpDX.IO;
//using SharpDX.WIC;
//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;
//using AlphaMode = SharpDX.Direct2D1.AlphaMode;
//using Bitmap = System.Drawing.Bitmap;
//using D2DBitmap = SharpDX.Direct2D1.Bitmap;
//using D2DPixelFormat = SharpDX.Direct2D1.PixelFormat;
//using PixelFormat = System.Drawing.Imaging.PixelFormat;
//using Rectangle = System.Drawing.Rectangle;

namespace YouiToolkit.Design
{
    internal static class DirectXUtils
    {
//        public D2DBitmap ConvertFromSystemBitmap(RenderTarget renderTarget, Bitmap source)
//        {
//            Bitmap target;

//            if (source.PixelFormat != PixelFormat.Format32bppPArgb)
//            {
//                target = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppPArgb);
//                using Graphics g = System.Drawing.Graphics.FromImage(target);
//                g.DrawImage(source, 0, 0);
//            }
//            else
//            {
//                target = source;
//            }

//            BitmapData bmpData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.ReadOnly, target.PixelFormat);
//            byte[] byteData = new byte[bmpData.Stride * target.Height];
//            Marshal.Copy(bmpData.Scan0, byteData, 0, byteData.Length);
//            target.UnlockBits(bmpData);

//            D2DPixelFormat pixelFormat = new D2DPixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
//            BitmapProperties bp = new BitmapProperties(pixelFormat, target.HorizontalResolution, target.VerticalResolution);
//            D2DBitmap tempBitmap = new D2DBitmap(renderTarget, new Size2(target.Width, target.Height), bp);
//            tempBitmap.CopyFromMemory(byteData, bmpData.Stride);

//            return tempBitmap;
//         }

//        /// <summary>
//        /// Loads an existing image file into a SharpDX.Direct2D1.Bitmap1.
//        /// </summary>
//        /// <param name="filePath">Relative path to the content file.</param>
//        /// <returns>Loaded bitmap.</returns>
//        public SharpDX.Direct2D1.Bitmap1 LoadBitmapFromContentFile(this RenderTarget d2dContext, string filePath)
//        {
//            SharpDX.Direct2D1.Bitmap1 newBitmap;

//            // Neccessary for creating WIC objects.
//            ImagingFactory imagingFactory = new ImagingFactory();
//            NativeFileStream fileStream = new NativeFileStream(filePath, NativeFileMode.Open, NativeFileAccess.Read);

//            // Used to read the image source file.
//            BitmapDecoder bitmapDecoder = new BitmapDecoder(imagingFactory, fileStream, DecodeOptions.CacheOnDemand);

//            // Get the first frame of the image.
//            BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

//            // Convert it to a compatible pixel format.
//            FormatConverter converter = new FormatConverter(imagingFactory);
//            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);

//            // Create the new Bitmap1 directly from the FormatConverter.
//            newBitmap = SharpDX.Direct2D1.Bitmap1.FromWicBitmap(d2dContext, converter) as Bitmap1;

//            Utilities.Dispose(ref bitmapDecoder);
//            Utilities.Dispose(ref fileStream);
//            Utilities.Dispose(ref imagingFactory);
//            Utilities.Dispose(ref converter);
//            Utilities.Dispose(ref frame);
//            return newBitmap;
//        }
    }
}

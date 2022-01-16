using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace YouiToolkit.Design
{
    public static class ImageUtil
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Bitmap转ImageSource
        /// </summary>
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // 记得要进行内存释放 否则会有内存不足的报错
            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }
            return wpfBitmap;
        }

        public static BitmapSource ToBitmapSource(this InkCanvas canvas)
        {
            // 获取笔画边界
            Rect rect = canvas.Strokes.GetBounds();

            // 获取笔画轮廓几何图形
            double width = canvas.DefaultDrawingAttributes.Width; // 笔画宽度
            double height = canvas.DefaultDrawingAttributes.Height; // 笔画高度
            RectangleGeometry geometry = new RectangleGeometry(new Rect(rect.X - width / 2, rect.Y - height / 2, rect.Width + width, rect.Height + height));
            canvas.Clip = geometry;
            canvas.UpdateLayout();

            // 将笔画转换为图像
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)geometry.Rect.Width, (int)geometry.Rect.Height, 96, 96, PixelFormats.Default);
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(canvas), null, new Rect(0, 0, geometry.Rect.Width, geometry.Rect.Height));
            }
            bitmap.Render(visual);
            canvas.Clip = null;
            return bitmap;
        }

        public static Bitmap ToBitmap(this BitmapSource source)
        {
            using MemoryStream ms = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(ms);
            return new Bitmap(ms);
        }

        /// <summary>
        /// Bitmap转BitmapSource
        /// </summary>
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }

        /// <summary>
        /// Icon转ImageSource
        /// </summary>
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        public static Bitmap GetFromBase64(string imageBase64)
        {
            if (imageBase64 == null) return null;
            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            Stream stream = new MemoryStream(imageBytes);
            Bitmap image = new Bitmap(stream);
            return image;
        }

        public static string ConvertToBase64(Image image, ImageFormat imageFormat = null)
        {
            if (image == null) return null;
            using MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, imageFormat ?? image.RawFormat);
            byte[] imageBytes = memoryStream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Image = System.Windows.Controls.Image;
using MBrushes = System.Windows.Media.Brushes;
using MColor = System.Windows.Media.Color;
using MColors = System.Windows.Media.Colors;
using MDashStyle = System.Windows.Media.DashStyle;
using MPen = System.Windows.Media.Pen;
using MPoint = System.Windows.Point;
using MSize = System.Windows.Size;
using MSolidColorBrush = System.Windows.Media.SolidColorBrush;
using Size = System.Drawing.Size;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using DashStyle = System.Drawing.Drawing2D.DashStyle;

namespace YouiToolkit.Design
{
    public class ThumbnailImage : Image
    {
        public event EventHandler<ThumbnailImageShortCutEventArgs> ShortCut;

        protected Bitmap ThumbnailBitmap;

        public Bitmap GridThumbnailBitmap
        {
            get
            {
                if (ThumbnailBitmap == null) return null;
                Bitmap gridImage = new Bitmap(ThumbnailBitmap);
                using Graphics g = Graphics.FromImage(gridImage);

                g.Clear(Color.White);
                {
                    using Pen pen = new Pen(Color.Gray, 0.1f);
                    pen.DashStyle = DashStyle.Dot;
                    pen.DashPattern = new float[] { 1f, 3f };

                    g.DrawLine(pen, new PointF(0f, gridImage.Height / 2f), new PointF(gridImage.Width, gridImage.Height / 2f));
                    g.DrawLine(pen, new PointF(gridImage.Width / 2f, 0f), new PointF(gridImage.Width / 2f, gridImage.Height));
                }
                g.DrawImage(ThumbnailBitmap, PointF.Empty);
                return gridImage;
            }
        }

        public new ImageSource Source
        {
            get => base.Source;
            protected set => base.Source = value;
        }

        protected bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            internal set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    InvalidateVisual();
                }
            }
        }

        public int Index
        {
            get
            {
                if (Tag is string @string)
                {
                    if (int.TryParse(@string, out int @int))
                    {
                        return @int;
                    }
                }
                else if (Tag is int @int)
                {
                    return @int;
                }
                return -1;
            }
            set => Tag = value;
        }

        public Size ActualSize
        {
            get
            {
                if (Width.Equals(double.NaN) || Height.Equals(double.NaN))
                {
                    return new Size(160, 80);
                }
                return new Size((int)Width, (int)Height);
            }
        }

        public ThumbnailImage()
        {
            InitializeComponent();

            this.SetFocusable();

            SizeChanged += (s, e) => InvalidateVisual();
            MouseLeftButtonDown += (s, e) =>
            {
                IsSelected = true;
                if (Parent is Panel panel)
                {
                    foreach (var control in panel.Children)
                    {
                        if (control is ThumbnailImage image && image != this)
                        {
                            image.IsSelected = false;
                        }
                    }
                }
            };
            LostFocus += (s, e) => InvalidateVisual();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.X:
                        Cut();
                        break;
                    case Key.C:
                        Copy();
                        break;
                    case Key.V:
                        Paste();
                        break;
                }
            }
            else if (e.Key == Key.Delete)
            {
                Remove();
            }
        }

        public void Copy()
        {
            ClipboardUtils.Begin();
            // 剪贴板特性：缩略图没有也得有
            ClipboardUtils.AddBitmap(GridThumbnailBitmap ?? new Bitmap(1, 1));
            ClipboardUtils.AddBitmapBase64(ThumbnailBitmap ?? new Bitmap(1, 1));
            ShortCut?.Invoke(this, ThumbnailImageShortCutEventArgs.Copy(Index));
        }

        public void Paste()
        {
            SetThumbnail(ClipboardUtils.GetBitmapBase64());
            ShortCut?.Invoke(this, ThumbnailImageShortCutEventArgs.Paste(Index));
        }

        public void Cut()
        {
            Copy();
            Remove();
            ShortCut?.Invoke(this, ThumbnailImageShortCutEventArgs.Cut(Index));
        }

        public void Remove()
        {
            SetThumbnail(null);
            ShortCut?.Invoke(this, ThumbnailImageShortCutEventArgs.Remove(Index));
        }

        public void InitializeComponent()
        {
            using Bitmap none = new Bitmap(ActualSize.Width, ActualSize.Height);
            Source = none.ToImageSource();
        }

        public void SetThumbnail(Bitmap bitmap)
        {
            ThumbnailBitmap = bitmap;
            RenderThumbnail();
        }

        public void RenderThumbnail()
        {
            Size size = ActualSize;
            int width = size.Width;
            int height = size.Height;

            using Bitmap fullBitmap = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(fullBitmap);

            if (ThumbnailBitmap != null)
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(ThumbnailBitmap, new RectangleF((fullBitmap.Width - fullBitmap.Height) / 2f, 0, fullBitmap.Height, fullBitmap.Height));
            }
            Source = fullBitmap.ToImageSource();
        }

        protected override void OnRender(DrawingContext dc)
        {
            Size size = ActualSize;
            const double offserBase = 0.3d;
            const double offserRight = 1.5d; // 右侧存在滚动条偏差稍大点
            int index = Index + 1;

            var brushGrid = new MSolidColorBrush(MColors.Gray);

            // 绘制网格
            {
                var pen = new MPen(brushGrid, 0.3d)
                {
                    DashStyle = new MDashStyle()
                    {
                        Offset = 0,
                        Dashes = new DoubleCollection()
                        {
                            7,
                        },
                    }
                };
                dc.DrawLine(pen, new MPoint(0d, size.Height / 2d), new MPoint(size.Width - offserRight, size.Height / 2d));
                dc.DrawLine(pen, new MPoint(size.Width / 2d, 0d), new MPoint(size.Width / 2d, size.Height));
            }

            // 绘制边框
            {
                var pen = new MPen(brushGrid, 0.3d);

                if (index == 1)
                {
                    // 上边
                    dc.DrawLine(pen, new MPoint(offserBase, offserBase), new MPoint(size.Width - offserRight, offserBase));
                }

                // 左边
                dc.DrawLine(pen, new MPoint(offserBase, offserBase), new MPoint(offserBase, size.Height));

                // 右边
                dc.DrawLine(pen, new MPoint(size.Width - offserRight, offserBase), new MPoint(size.Width - offserRight, size.Height));

                // 下边
                dc.DrawLine(pen, new MPoint(offserBase, size.Height - offserBase), new MPoint(size.Width - offserRight, size.Height - offserBase));
            }

            // 绘制选中特效
            if (IsSelected)
            {
                var brushMask = new MSolidColorBrush(MColor.FromArgb(128, 230, 230, 230));

                dc.DrawRectangle(brushMask, null, new Rect(new MPoint(offserBase, offserBase), new MSize(size.Width - offserRight - offserBase * 2d, size.Height - offserBase * 2d)));
            }

            // 绘制文字编号
            {
                var brush = IsSelected ? MBrushes.Blue : MBrushes.Black;
#pragma warning disable CS0618
                var text = new FormattedText(index.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Microsoft Yahei UI"), 15, brush);
#pragma warning restore CS0618

                dc.DrawText(text, new MPoint(2d, 1d));
            }

            // 绘制缩略图
            if (Source != null)
            {
                dc.DrawImage(Source, new Rect((size.Width - Source.Width) / 2d, (size.Height - Source.Height) / 2d, Source.Width, Source.Height));
            }
        }

        public override string ToString() => $"thumbImage (index={Index})";
    }
}

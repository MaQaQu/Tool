using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using Factory = SharpDX.Direct2D1.Factory;
using WriteFactory = SharpDX.DirectWrite.Factory;

namespace YouiToolkit.Design.DirectX
{
    public abstract class DirectXRenderer : IDisposable
    {
        #region [渲染接口]
        protected abstract Factory D2DFactory { get; }
        protected abstract RenderTarget D2DRenderTarget { get; }
        protected WriteFactory WriteFactory { get; private set; }
        #endregion

        #region [渲染资源]
        protected const string FontNameDefault = "Microsoft Yahei UI";
        protected StrokeStyle StrokeStyleDefault => new StrokeStyle(D2DFactory, new StrokeStyleProperties());
        protected StrokeStyle StrokeStyleRound => new StrokeStyle(D2DFactory, new StrokeStyleProperties()
        {
            StartCap = CapStyle.Round,
            EndCap = CapStyle.Round,
        });
        protected StrokeStyle StrokeStyleDashDotDot => new StrokeStyle(D2DFactory, new StrokeStyleProperties()
        {
            DashStyle = DashStyle.DashDotDot,
        });
        #endregion

        protected abstract float OriginScale { get; }
        protected abstract RawVector2 RawVectorMapToPanel(RawVector2 vec);
        protected abstract RawVector2 RawVectorPanelToMap(RawVector2 vec);

        /// <summary>
        /// 构造
        /// </summary>
        public DirectXRenderer()
        {
            WriteFactory = new WriteFactory();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            WriteFactory?.Dispose();
        }

        /// <summary>
        /// 测量文本
        /// </summary>
        public Size2F MeasureString(string text, TextFormat textFormat, float Width = 100f)
        {
            TextLayout layout = new TextLayout(WriteFactory, text, textFormat, Width, textFormat.FontSize);
            return new Size2F(layout.Metrics.Width, layout.Metrics.Height);
        }

        /// <summary>
        /// 绘制文本
        /// </summary>
        public void DrawText(string text, TextFormat textFormat, RawRectangleF layoutRect, Brush brush)
        {
            RawVector2 p1 = RawVectorMapToPanel(new RawVector2(layoutRect.Left, layoutRect.Top));
            RawVector2 p2 = RawVectorMapToPanel(new RawVector2(layoutRect.Right, layoutRect.Bottom));
            D2DRenderTarget.DrawText(text, textFormat, new RawRectangleF(p1.X, p1.Y, p2.X, p2.Y), brush);
        }

        /// <summary>
        /// 绘制直线
        /// </summary>
        public void DrawLine(float x1, float y1, float x2, float y2, Color color, float strokeWidth, StrokeStyle strokeStyle = null)
        {
            using var brush = new SolidColorBrush(D2DRenderTarget, color);
            DrawLine(x1, y1, x2, y2, brush, strokeWidth, strokeStyle);
        }

        /// <summary>
        /// 绘制直线
        /// </summary>
        public void DrawLine(float x1, float y1, float x2, float y2, Brush brush, float strokeWidth, StrokeStyle strokeStyle = null)
        {
            float scale = OriginScale;
            RawVector2 start = RawVectorMapToPanel(new RawVector2(x1, y1));
            RawVector2 stop = RawVectorMapToPanel(new RawVector2(x2, y2));
            using StrokeStyle strokeStyle1 = strokeStyle ?? StrokeStyleRound;

            D2DRenderTarget.DrawLine(start, stop, brush, strokeWidth / scale, strokeStyle1);
        }

        /// <summary>
        /// 填充椭圆形
        /// </summary>
        public void FillEllipse(float cx, float cy, float a, float b, Color color)
        {
            using var brush = new SolidColorBrush(D2DRenderTarget, color);
            FillEllipse(cx, cy, a, b, brush);
        }

        /// <summary>
        /// 填充椭圆形
        /// </summary>
        public void FillEllipse(float cx, float cy, float a, float b, Brush brush)
        {
            float scale = OriginScale;
            RawVector2 center = RawVectorMapToPanel(new RawVector2(cx, cy));
            var ellipse = new Ellipse(new RawVector2(center.X, center.Y), a / scale, b / scale);

            D2DRenderTarget.FillEllipse(ellipse, brush);
        }

        /// <summary>
        /// 绘制椭圆形
        /// </summary>
        public void DrawEllipse(float cx, float cy, float a, float b, Color color, float strokeWidth, StrokeStyle strokeStyle = null)
        {
            using var brush = new SolidColorBrush(D2DRenderTarget, color);
            DrawEllipse(cx, cy, a, b, brush, strokeWidth, strokeStyle);
        }

        /// <summary>
        /// 绘制椭圆形
        /// </summary>
        public void DrawEllipse(float cx, float cy, float a, float b, Brush brush, float strokeWidth, StrokeStyle strokeStyle = null)
        {
            float scale = OriginScale;
            RawVector2 center = RawVectorMapToPanel(new RawVector2(cx, cy));
            var ellipse = new Ellipse(new RawVector2(center.X, center.Y), a / scale, b / scale);
            using StrokeStyle strokeStyle1 = strokeStyle ?? StrokeStyleDefault;

            D2DRenderTarget.DrawEllipse(ellipse, brush, strokeWidth / scale, strokeStyle1);
        }

        /// <summary>
        /// 填充圆形
        /// </summary>
        public void FillCircle(float cx, float cy, float r, Color color)
            => FillEllipse(cx, cy, r, r, color);

        /// <summary>
        /// 填充圆形
        /// </summary>
        public void FillCircle(float cx, float cy, float r, Brush brush)
            => FillEllipse(cx, cy, r, r, brush);

        /// <summary>
        /// 绘制圆形
        /// </summary>
        public void DrawCircle(float cx, float cy, float r, Color color, float strokeWidth, StrokeStyle strokeStyle = null)
            => DrawEllipse(cx, cy, r, r, color, strokeWidth, strokeStyle);

        /// <summary>
        /// 绘制圆形
        /// </summary>
        public void DrawCircle(float cx, float cy, float r, Brush color, float strokeWidth, StrokeStyle strokeStyle = null)
            => DrawEllipse(cx, cy, r, r, color, strokeWidth, strokeStyle);

        /// <summary>
        /// 填充矩形
        /// </summary>
        public void FillRectangle(float leftTopX, float leftTopY, float rightBottomX, float rightBottomY, Color color)
        {
            using var brush = new SolidColorBrush(D2DRenderTarget, color);

            RawVector2 v1 = RawVectorMapToPanel(new RawVector2(leftTopX, leftTopY));
            RawVector2 v2 = RawVectorMapToPanel(new RawVector2(leftTopX, rightBottomY));
            RawVector2 v3 = RawVectorMapToPanel(new RawVector2(rightBottomX, rightBottomY));
            RawVector2 v4 = RawVectorMapToPanel(new RawVector2(rightBottomX, leftTopY));

            using var geometry = new PathGeometry(D2DFactory);
            using var geometrySink = geometry.Open();
            geometrySink.BeginFigure(v1, FigureBegin.Filled);
            geometrySink.AddLine(v2);
            geometrySink.AddLine(v3);
            geometrySink.AddLine(v4);
            geometrySink.EndFigure(FigureEnd.Closed);
            geometrySink.Close();

            D2DRenderTarget.FillGeometry(geometry, brush);
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        public void FillPolygon(Color color, params RawVector2[] points)
        {
            if (points == null || points.Length <= 1)
                return;

            using var brush = new SolidColorBrush(D2DRenderTarget, color);
            using var geometry = new PathGeometry(D2DFactory);
            using var geometrySink = geometry.Open();

            for (int i = default; i < points.Length; i++)
            {
                RawVector2 v = RawVectorMapToPanel(points[i]);

                if (i == default)
                {
                    geometrySink.BeginFigure(v, FigureBegin.Filled);
                }
                else
                {
                    geometrySink.AddLine(v);
                }
            }
            geometrySink.EndFigure(FigureEnd.Closed);
            geometrySink.Close();

            D2DRenderTarget.FillGeometry(geometry, brush);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public void DrawRectangle(float leftTopX, float leftTopY, float rightBottomX, float rightBottomY, Color color, float strokeWidth, StrokeStyle strokeStyle = null)
        {
            using var brush = new SolidColorBrush(D2DRenderTarget, color);
            using StrokeStyle strokeStyle1 = strokeStyle ?? StrokeStyleDefault;
            DrawRectangle(leftTopX, leftTopY, rightBottomX, rightBottomY, brush, strokeWidth, strokeStyle1);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public void DrawRectangle(float leftTopX, float leftTopY, float rightBottomX, float rightBottomY, Brush brush, float strokeWidth, StrokeStyle strokeStyle = null)
        {
            float scale = OriginScale;
            RawVector2 v1 = RawVectorMapToPanel(new RawVector2(leftTopX, leftTopY));
            RawVector2 v2 = RawVectorMapToPanel(new RawVector2(leftTopX, rightBottomY));
            RawVector2 v3 = RawVectorMapToPanel(new RawVector2(rightBottomX, rightBottomY));
            RawVector2 v4 = RawVectorMapToPanel(new RawVector2(rightBottomX, leftTopY));

            using var geometry = new PathGeometry(D2DFactory);
            using var geometrySink = geometry.Open();
            geometrySink.BeginFigure(v1, FigureBegin.Filled);
            geometrySink.AddLine(v2);
            geometrySink.AddLine(v3);
            geometrySink.AddLine(v4);
            geometrySink.EndFigure(FigureEnd.Closed);
            geometrySink.Close();

            using StrokeStyle strokeStyle1 = strokeStyle ?? StrokeStyleDefault;
            D2DRenderTarget.DrawGeometry(geometry, brush, strokeWidth / scale, strokeStyle1);
        }

        /// <summary>
        /// 填充三角形
        /// </summary>
        public void FillTriangle(RawVector2 p1, RawVector2 p2, RawVector2 p3, Color color, float strokeWidth)
        {
            using var brush = new SolidColorBrush(D2DRenderTarget, color);
            FillTriangle(p1, p2, p3, brush, strokeWidth);
        }

        /// <summary>
        /// 填充三角形
        /// </summary>
        public void FillTriangle(RawVector2 p1, RawVector2 p2, RawVector2 p3, Brush brush, float strokeWidth)
        {
            using var geometry = new PathGeometry(D2DFactory);
            using var geometrySink = geometry.Open();

            RawVector2 v1 = RawVectorMapToPanel(p1);
            RawVector2 v2 = RawVectorMapToPanel(p2);
            RawVector2 v3 = RawVectorMapToPanel(p3);

            geometrySink.BeginFigure(v1, FigureBegin.Filled);
            geometrySink.AddLine(v2);
            geometrySink.AddLine(v3);
            geometrySink.EndFigure(FigureEnd.Closed);
            geometrySink.Close();

            D2DRenderTarget.FillGeometry(geometry, brush);
        }

        /// <summary>
        /// 填充弧线
        /// </summary>
        public void FillArc(RawVector2 p1, RawVector2 p2, RawVector2 p3, Color color)
        {
            using Brush brush = new SolidColorBrush(D2DRenderTarget, color);
            FillArc(p1, p2, p3, brush);
        }

        /// <summary>
        /// 填充弧线
        /// </summary>
        public void FillArc(RawVector2 p1, RawVector2 p2, RawVector2 p3, Brush brush)
        {
            using var geometry = new PathGeometry(D2DFactory);
            using var geometrySink = geometry.Open();

            RawVector2 v1 = RawVectorMapToPanel(p1);
            RawVector2 v2 = RawVectorMapToPanel(p2);
            RawVector2 v3 = RawVectorMapToPanel(p3);

            float dx = v2.X - v1.X;
            float dy = v2.Y - v1.Y;
            float radius = (float)Math.Sqrt(dx * dx + dy * dy);

            geometrySink.BeginFigure(v1, FigureBegin.Filled);
            geometrySink.AddLine(v2);
            geometrySink.AddArc(new ArcSegment() { Point = v3, Size = new Size2F(radius, radius), RotationAngle = 0, SweepDirection = SweepDirection.CounterClockwise, ArcSize = ArcSize.Small });
            geometrySink.AddLine(v1);
            geometrySink.EndFigure(FigureEnd.Closed);
            geometrySink.Close();

            D2DRenderTarget.FillGeometry(geometry, brush);
        }
    }
}

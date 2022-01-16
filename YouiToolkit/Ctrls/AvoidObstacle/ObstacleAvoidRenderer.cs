using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Windows;
using YouiToolkit.Assist;
using YouiToolkit.Design.DirectX;
using YouiToolkit.Models;
using Color = SharpDX.Color;
using Factory = SharpDX.Direct2D1.Factory;
using TextFormat = SharpDX.DirectWrite.TextFormat;

namespace YouiToolkit.Ctrls
{
    internal class AvoidObstacleRenderer : DirectXRenderer
    {
        #region [渲染参数]
        public FrameworkElement Parent { get; set; } = null;
        public AvoidObstacleRenderContext Context { get; set; } = null;
        public ToolkitAssist Assist => AssistManager.Assist;
        #endregion

        #region [渲染接口]
        protected override Factory D2DFactory => Context.D2DFactory;
        protected override RenderTarget D2DRenderTarget => Context.D2DRenderTarget;
        #endregion

        protected override float OriginScale => Context.OriginScale;
        protected override RawVector2 RawVectorMapToPanel(RawVector2 vec)
            => Context.RawVectorMapToPanel(vec);
        protected override RawVector2 RawVectorPanelToMap(RawVector2 vec)
            => Context.RawVectorPanelToMap(vec);

        protected TextFormat AxisTextFormat = null;
        protected float AxisHeight = 100f;

        public AvoidObstacleRenderer(FrameworkElement parent, AvoidObstacleRenderContext context)
        {
            Parent = parent;
            Context = context;
            AxisTextFormat = new TextFormat(WriteFactory, FontNameDefault, 13);
            AxisHeight = MeasureString("-999999", AxisTextFormat, float.MaxValue).Height + 12;
        }

        public void DrawScaleAxisY(string text, TextFormat textFormat, RawRectangleF layoutRect, Brush brush)
        {
            RawVector2 p1 = RawVectorMapToPanel(new RawVector2(layoutRect.Left, layoutRect.Top));
            RawVector2 p2 = RawVectorMapToPanel(new RawVector2(layoutRect.Right, layoutRect.Bottom));
            D2DRenderTarget.DrawText(text, textFormat, new RawRectangleF(0, p1.Y, p2.X, p2.Y), brush);
        }

        public void DrawScaleAxisX(string text, TextFormat textFormat, RawRectangleF layoutRect, Brush brush)
        {
            RawVector2 p1 = RawVectorMapToPanel(new RawVector2(layoutRect.Left, layoutRect.Top));
            RawVector2 p2 = RawVectorMapToPanel(new RawVector2(layoutRect.Right, layoutRect.Bottom));
            D2DRenderTarget.DrawText(text, textFormat, new RawRectangleF(p1.X, Context.PanelHeight - AxisHeight, p2.X, p2.Y + (Context.PanelHeight - AxisHeight)), brush);
        }

        internal void Render()
        {
            D2DRenderTarget.Clear(Color.Transparent);

            Vector2 centerMap = Context.CenterMap;
            float regionRadius = Context.RegionRadius;
            float originScale = Context.OriginScale;

            // 绘制网格
            if (Context.ShowGrid)
            {
                float rad = regionRadius;
#if false
                float interval = (float)Math.Pow(10, Math.Round(Math.Log10(rad / 10)));
#else
                float interval = 0;
#endif
                if (interval < 10000)
                {
                    interval = 10000;
                }
                interval -= interval % 10000;

                float minX = centerMap.X - rad;
                float maxX = centerMap.X + rad;
                float minY = centerMap.Y - rad;
                float maxY = centerMap.Y + rad;

                FillRectangle(-Context.LidarRange, -Context.LidarRange, Context.LidarRange, Context.LidarRange, Context.MaskColor);
                FillCircle(0, 0, Context.LidarRange, Context.BackColor);
                
                switch (Context.Layer.Angle)
                {
                    case AvoidObstacleAngle._180:
                        FillArc(new RawVector2(0, 0), new RawVector2(-Context.LidarRange, 0), new RawVector2(Context.LidarRange, 0), Context.MaskColor);
                        break;
                    case AvoidObstacleAngle._270:
                        {
                            float x = Context.LidarRange * (float)Math.Sin(Math.PI / 4d);
                            float y = Context.LidarRange * (float)Math.Cos(Math.PI / 4d);
                            FillArc(new RawVector2(0, 0), new RawVector2(-x, -y), new RawVector2(x, -y), Context.MaskColor);
                        }
                        break;
                    case AvoidObstacleAngle._360:
                        break;
                }

                using var brush = new SolidColorBrush(D2DRenderTarget, Color.Black);

                for (float loopY = maxY - (maxY % interval); loopY > minY; loopY -= interval)
                {
                    DrawLine(minX, loopY, maxX, loopY, Context.GridColor, originScale / 2f, StrokeStyleDefault);
                    DrawScaleAxisY((loopY / 10000).ToString(), AxisTextFormat, new RawRectangleF(0, loopY, 100, loopY + 100), brush);
                }

                for (float loopX = maxX - (maxX % interval); loopX > minX; loopX -= interval)
                {
                    DrawLine(loopX, minY, loopX, maxY, Context.GridColor, originScale / 2f, StrokeStyleDefault);
                    DrawScaleAxisX((loopX / 10000).ToString(), AxisTextFormat, new RawRectangleF(loopX, 0, loopX + 10000, 100), brush);
                }
            }

            // 绘制原点坐标系
            if (Context.ShowOriginAxis)
            {
                DrawLine(-Context.LidarRange, 0, Context.LidarRange, 0, Context.OriginAxisColor, originScale * 1.2f);
                DrawLine(0, -Context.LidarRange, 0, Context.LidarRange, Context.OriginAxisColor, originScale * 1.2f);
            }

            // 绘制激光雷达范围
            if (Context.ShowLidarRange)
            {
                for (int i = 10000; i < 100000; i += 10000)
                {
                    DrawCircle(0, 0, i, Context.GridColor, originScale / 2f);
                }
            }

            if (Context.Layer.PaintMode == AvoidObstacleAreaPaintMode.DragPoint)
            {
                foreach (var set in Context.Layer.DragPointSetBlock.RenderSorted)
                {
                    FillPolygon(set.PolygonColor, set.ToArray());
                    if (set.IsActivted)
                    {
                        foreach (AvoidObstacleDragPoint entity in set.DragPoints)
                        {
                            if (entity != null)
                            {
                                float leftTopX, leftTopY, rightBottomX, rightBottomY;

                                leftTopX = entity.X + AvoidObstacleDragPoint.ClientRectangle.Left;
                                leftTopY = entity.Y + AvoidObstacleDragPoint.ClientRectangle.Top;
                                rightBottomX = entity.X + AvoidObstacleDragPoint.ClientRectangle.Width / 2f;
                                rightBottomY = entity.Y + AvoidObstacleDragPoint.ClientRectangle.Height / 2f;

                                FillRectangle(leftTopX, leftTopY, rightBottomX, rightBottomY, entity.IsMouseHover ? AvoidObstacleDragPoint.HoverColor : AvoidObstacleDragPoint.DefaultColor);
                            }
                        }
                    }
                }
            }
        }
    }
}

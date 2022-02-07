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
    internal class MtMapImageRender : DirectXRenderer
    {
        #region [渲染参数]
        public FrameworkElement Parent { get; set; } = null;
        public MtMapImageRenderContext Context { get; set; } = null;
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
        PageMtMapRenderModel mapModel = PageMtMapRenderModel.CreateInstance();

        public MtMapImageRender(FrameworkElement parent, MtMapImageRenderContext context)
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
            // 绘制背景
            D2DRenderTarget.Clear(Context.BackColor);

            // 绘制原点
            if (Context.ShowCenter)
            {
                FillCircle(centerMap.X, centerMap.Y, 1000, Context.GridColor);
            }

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

                float loopY = maxY - (maxY % interval);
                for (; loopY > minY; loopY -= interval)
                {
                    DrawLine(minX, loopY, maxX, loopY, Context.GridColor, originScale, StrokeStyleDashDotDot);
                }

                float loopX = maxX - (maxX % interval);
                for (; loopX > minX; loopX -= interval)
                {
                    DrawLine(loopX, minY, loopX, maxY, Context.GridColor, originScale, StrokeStyleDashDotDot);
                }
            }

            // 绘制原点坐标系
            if (Context.ShowOriginAxis)
            {
                DrawLine(0, 0, (float)(Context.OriginAxisSize * 10d), 0, Context.OriginAxisXColor, originScale);
                DrawLine(0, 0, 0, (float)(Context.OriginAxisSize * 10d), Context.OriginAxisYColor, originScale);
                FillCircle(0, 0, (float)Context.OriginAxisSize / 3f, Context.OriginPointColor);
            }
            // 绘制实时点云
            if (Context.ShowPointCloud)
            {
                //区分实时和回放
                if (mapModel.ShowType == (int)MtNavDataShowType.PlayBack)
                {
                    if (mapModel.Count > 0)
                    {
                        for (int i = 0; i < mapModel.Count; i++)
                        {
                            FillCircle((float)mapModel.MapPoints[i].X, (float)mapModel.MapPoints[i].Y, originScale * 3, Context.PointCloudColor);
                        }
                    }
                }
                else
                {
                    AssistPointCloud cloud = Assist.Status.LidarPointCloud;
                    if (cloud.Count > 0)
                    {
                        for (int i = 0; i < cloud.Count; i++)
                        {
                            FillCircle((float)cloud.MapPoints[i].X, (float)cloud.MapPoints[i].Y, originScale * 3, Context.PointCloudColor);
                        }
                    }
                }
            }

            // 绘制先验点云图
            //if (Context.ShowPointCloudPrior)
            //{
            //    AssistPointCloud cloud = Assist.Status.LidarPointCloud;
            //    if (cloud.MapPointsPrior != null)
            //    {
            //        Stack<PointGridMap> mapStack = new Stack<PointGridMap>();
            //        mapStack.Push(cloud.GridMapPrior);

            //        while (mapStack.Count > 0)
            //        {
            //            PointGridMap map = mapStack.Pop();
            //            if (/*true && */originScale > (map.Resolution / (map.GetSubMapStep())))
            //            {
            //                DrawGridMapPrior(map);
            //            }
            //            else
            //            {
            //                if (Context.InsidePanel(map))
            //                {
            //                    PointGridMap[] maps = map.SubMap;
            //                    if (maps != null)
            //                    {
            //                        foreach (PointGridMap submap in maps)
            //                        {
            //                            if (submap != null)
            //                            {
            //                                mapStack.Push(submap);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

        }
    }
}

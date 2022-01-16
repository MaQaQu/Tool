using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Windows;
using YouiToolkit.Assist;
using Color = SharpDX.Color;
using Factory = SharpDX.Direct2D1.Factory;
using WriteFactory = SharpDX.DirectWrite.Factory;
using YouiToolkit.Design.DirectX;

namespace YouiToolkit.Ctrls
{
    internal class MapRenderer : DirectXRenderer
    {
        #region [渲染参数]
        public FrameworkElement Parent { get; set; } = null;
        public MapRenderContext Context { get; set; } = null;
        public ToolkitAssist Assist => AssistManager.Assist;
        #endregion

        #region [渲染接口]
        protected override Factory D2DFactory => Context.D2DFactory;
        protected override RenderTarget D2DRenderTarget => Context.D2DRenderTarget;
        #endregion

        public MapRenderer(FrameworkElement parent, MapRenderContext context)
        {
            Parent = parent;
            Context = context;
        }

        protected override float OriginScale => Context.OriginScale;
        protected override RawVector2 RawVectorMapToPanel(RawVector2 vec)
            => Context.RawVectorMapToPanel(vec);
        protected override RawVector2 RawVectorPanelToMap(RawVector2 vec)
            => Context.RawVectorPanelToMap(vec);

        private void DrawGridMap(PointGridMap map)
        {
            for (int i = 0; i < map.GetSubMapMaxCount(); i++)
            {
                //if (map.SubPointCount[i] <= (map.Resolution / (Math.Pow(map.GetSubMapStep(), 2))))
                //if (map.SubPointCount[i] <= 0.05*(dx2d.OriginScale) * (map.Resolution))
                //if (map.SubPointCount[i] <= Math.Ceiling(0.003 * Math.Pow(dx2d.OriginScale, 2)))
                if (map.SubPointCount[i] <= Math.Ceiling(0.0002 * Math.Pow(map.Resolution, 2)))
                {
                    continue;
                }

                int row = i / map.GetSubMapStep();
                int col = i % map.GetSubMapStep();

#if true
                float x = map.MinX + (col + 0.5f) * map.Resolution;
                float y = map.MinY + (row + 0.5f) * map.Resolution;
                FillCircle(x, y, Context.OriginScale, Context.GridMapColor);
#else
                float x = map.MinX + col * map.Resolution;
                float y = map.MinY + row * map.Resolution;
                FillRectangle(x, y + map.Resolution, x + map.Resolution + 20, y + 20, Context.GridMapColor);
#endif
            }
        }

        private void DrawGridMapPrior(PointGridMap map)
        {
            for (int i = 0; i < map.GetSubMapMaxCount(); i++)
            {
                if (map.SubPointCount[i] <= 0)
                {
                    continue;
                }

                int row = i / map.GetSubMapStep();
                int col = i % map.GetSubMapStep();

#if true
                float x = map.MinX + (col + 0.5f) * map.Resolution;
                float y = map.MinY + (row + 0.5f) * map.Resolution;
                FillCircle(x, y, Context.OriginScale, Context.GridMapColor);
#else
                float x = map.MinX + col * map.Resolution;
                float y = map.MinY + row * map.Resolution;
                FillRectangle(x, y + map.Resolution, x + map.Resolution + 20, y + 20, Context.GridMapColor);
#endif
            }
        }

        internal void Render()
        {
            // 地图数据预热
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
#if false
                DrawLine(0, 0, (float)(Context.OriginAxisSize * 10.0), 0, Color.Red, (float)Context.OriginAxisSize);
                DrawLine(0, 0, 0, (float)(Context.OriginAxisSize * 10.0), Color.Blue, (float)Context.OriginAxisSize);
                FillCircle(0, 0, (float)Context.OriginAxisSize, Context.OriginPointColor);
#else
                DrawLine(0, 0, (float)(Context.OriginAxisSize * 10d), 0, Context.OriginAxisXColor, originScale);
                DrawLine(0, 0, 0, (float)(Context.OriginAxisSize * 10d), Context.OriginAxisYColor, originScale);
                FillCircle(0, 0, (float)Context.OriginAxisSize / 3f, Context.OriginPointColor);
#endif
            }

            // 绘制实时位姿
            if (Context.ShowBaselinkAxis)
            {
                DrawLine((float)Assist.Status.Position.X, (float)Assist.Status.Position.Y, (float)Assist.Status.AxisX.X, (float)Assist.Status.AxisX.Y, Context.AGVThetaColor, (float)Context.OriginAxisSize);
                FillCircle((float)Assist.Status.Position.X, (float)Assist.Status.Position.Y, (float)Context.OriginAxisSize, Context.AGVOriginColor);
            }

            // 绘制历史位姿
            if (Context.ShowPoseGraph)
            {
                if (originScale < 200)
                {
                    for (int i = 0; i < Assist.Status.PoseGraphMaxCount; i++)
                    {
                        if (!Assist.Status.PoseGraphValids[i])
                        {
                            continue;
                        }

                        AssistPoseGraph g = Assist.Status.PoseGraphs[i];

                        if (g != null
                         && Math.Abs(g.X - centerMap.X) < regionRadius
                         && Math.Abs(g.Y - centerMap.Y) < regionRadius)
                        {
                            FillCircle((float)g.X, (float)g.Y, 100, Context.PoseGraphColor);
                        }
                    }
                }
            }

            // 绘制历史位姿点云图
            if (Context.ShowPoseMap)
            {
                Stack<PointGridMap> mapStack = new Stack<PointGridMap>();
                mapStack.Push(Assist.Status.GridMap);

                while (mapStack.Count > 0)
                {
                    PointGridMap map = mapStack.Pop();
                    if (true && originScale > (map.Resolution / (map.GetSubMapStep())))
                    {
                        DrawGridMap(map);
                    }
                    else
                    {
                        if (Context.InsidePanel(map))
                        {
                            PointGridMap[] maps = map.SubMap;
                            if (maps != null)
                            {
                                foreach (PointGridMap submap in maps)
                                {
                                    if (submap != null)
                                    {
                                        mapStack.Push(submap);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 绘制实时点云
            if (Context.ShowPointCloud)
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

            // 绘制先验点云图
            if (Context.ShowPointCloudPrior)
            {
                AssistPointCloud cloud = Assist.Status.LidarPointCloud;
                if (cloud.MapPointsPrior != null)
                {
                    Stack<PointGridMap> mapStack = new Stack<PointGridMap>();
                    mapStack.Push(cloud.GridMapPrior);

                    while (mapStack.Count > 0)
                    {
                        PointGridMap map = mapStack.Pop();
                        if (/*true && */originScale > (map.Resolution / (map.GetSubMapStep())))
                        {
                            DrawGridMapPrior(map);
                        }
                        else
                        {
                            if (Context.InsidePanel(map))
                            {
                                PointGridMap[] maps = map.SubMap;
                                if (maps != null)
                                {
                                    foreach (PointGridMap submap in maps)
                                    {
                                        if (submap != null)
                                        {
                                            mapStack.Push(submap);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 绘制手动重定位
            if (Context.ShowPoseEstimate)
            {
                DrawLine(Context.PoseEstimateStart.X, Context.PoseEstimateStart.Y, Context.PoseEstimateEnd.X, Context.PoseEstimateEnd.Y, Context.PoseEstimateColor, (float)Context.OriginAxisSize);
#if false
                FillTriangle(Context.PoseEstimateEnd, Context.PoseEstimateEnd1, Context.PoseEstimateEnd2, Context.PoseEstimateColor, (float)Context.OriginAxisSize);
#else
                DrawLine(Context.PoseEstimateEnd.X, Context.PoseEstimateEnd.Y, Context.PoseEstimateEnd1.X, Context.PoseEstimateEnd1.Y, Context.PoseEstimateColor, (float)Context.OriginAxisSize);
                DrawLine(Context.PoseEstimateEnd1.X, Context.PoseEstimateEnd1.Y, Context.PoseEstimateEnd2.X, Context.PoseEstimateEnd2.Y, Context.PoseEstimateColor, (float)Context.OriginAxisSize);
                DrawLine(Context.PoseEstimateEnd2.X, Context.PoseEstimateEnd2.Y, Context.PoseEstimateEnd.X, Context.PoseEstimateEnd.Y, Context.PoseEstimateColor, (float)Context.OriginAxisSize);
#endif
            }
        }
    }
}

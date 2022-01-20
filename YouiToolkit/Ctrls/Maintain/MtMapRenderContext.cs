using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SuperMath;
using System;
using System.Windows;
using System.Windows.Input;
using YouiToolkit.Assist;
using YouiToolkit.Design.DirectX;
using YouiToolkit.Models;
using YouiToolkit.Utils;
using Color = SharpDX.Color;
using Factory = SharpDX.Direct2D1.Factory;
using Point = System.Windows.Point;

namespace YouiToolkit.Ctrls
{
    public class MtMapRenderContext : DirectXRenderContext
    {
        public FrameworkElement Parent { get; set; } = null;

        public MtMapRenderContext(FrameworkElement parent, RenderTarget renderTarget, Factory factory)
            : this(parent)
        {
            InitDirectX(renderTarget, factory);
        }

        public MtMapRenderContext(FrameworkElement parent)
        {
            Parent = parent;
            InitMatrix();
            ChangeOriginScale(OriginScaleDefault);
        }

        #region [渲染标识]
        public double OriginAxisSize = 500;
        public bool ShowCenter { get; set; } = false;
        public bool ShowGrid { get; set; } = true;
        public bool ShowOriginAxis { get; set; } = true;
        public bool ShowBaselinkAxis { get; set; } = true;
        public bool ShowPointCloud { get; set; } = true;
        public bool ShowPoseGraph { get; set; } = true;
        public bool ShowPoseMap { get; set; } = true;
        public bool ShowPoseEstimate => DriftMode == MapMouseDriftMode.PoseEstimate;
        public bool ShowPointCloudPrior { get; set; } = true;
        #endregion

        #region [交互控制]
        public MapMouseDriftMode DriftMode { get; internal set; } = MapMouseDriftMode.None;
        public bool Drifting { get; internal set; } = false;
        public float DriftStartX { get; internal set; } = 0f;
        public float DriftStartY { get; internal set; } = 0f;
        public float DriftStartA { get; internal set; } = 0f;
        public float DriftMouseX { get; internal set; } = 0f;
        public float DriftMouseY { get; internal set; } = 0f;
        public float DriftCenterX { get; internal set; } = 0f;
        public float DriftCenterY { get; internal set; } = 0f;
        #endregion

        #region [位姿估计]
        public event EventHandler<PoseEstimateEventArgs> PoseEstimateConfirmed;
        public bool PoseEstimateRequested { get; set; } = false;
        public RawVector2 PoseEstimateStart { get; private set; } = default;
        public RawVector2 PoseEstimateEnd { get; private set; } = default;
        public RawVector2 PoseEstimateEnd1 { get; private set; } = default;
        public RawVector2 PoseEstimateEnd2 { get; private set; } = default;
        public const float PoseEstimateLen = 40f;
        public const float PoseEstimateTriangleLen = 6f;
        #endregion

        #region [颜色定义]
        /// <summary>
        /// 背景颜色
        /// </summary>
        [Comment("new Color(15, 15, 15)")]
        public Color BackColor { get; internal set; } = new Color(255, 255, 255);

        /// <summary>
        /// 网格颜色
        /// </summary>
        public Color GridColor { get; internal set; } = Color.Black;

        /// <summary>
        /// 点云颜色
        /// </summary>
        [Comment("Color.White")]
        public Color GridMapColor { get; internal set; } = Color.Black;

        /// <summary>
        /// 位姿估计颜色
        /// </summary>
        [Comment("new Color(0, 240, 0)")]
        public Color PoseEstimateColor { get; internal set; } = Color.Green;

        /// <summary>
        /// 实时点云颜色
        /// </summary>
        [Comment("new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)(byte.MaxValue / 3))")]
        public Color PointCloudColor { get; internal set; } = new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)(byte.MaxValue / 2));

        /// <summary>
        /// 位置点颜色
        /// </summary>
        public Color PoseGraphColor { get; internal set; } = Color.Green;

        /// <summary>
        /// 原点坐标系颜色
        /// </summary>
        public Color OriginAxisXColor { get; internal set; } = new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)(byte.MaxValue / 1.3));
        public Color OriginAxisYColor { get; internal set; } = new Color(Color.Blue.R, Color.Blue.G, Color.Blue.B, (byte)(byte.MaxValue / 1.3));
        [Comment("Color.White")]
        public Color OriginPointColor { get; internal set; } = Color.Blue;

        /// <summary>
        /// 小车颜色
        /// </summary>
        [Comment("Color.White")]
        public Color AGVOriginColor { get; internal set; } = Color.Gray;// new Color(130, 170, 230);
        public Color AGVThetaColor { get; internal set; } = Color.LightCoral;
        #endregion

        internal bool InsidePanel(PointGridMap map)
        {
            if (map == null)
            {
                return false;
            }

            float midX = (map.MaxX + map.MinX) / 2;
            float midY = (map.MaxY + map.MinY) / 2;
            float rad = (float)(map.Resolution * map.GetSubMapStep() / 2 * Math.Sqrt(2));

            if (Math.Abs(midX - CenterMap.X) < (RegionRadius + rad)
             && Math.Abs(midY - CenterMap.Y) < (RegionRadius + rad))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnLostFocus()
        {
            if (Drifting)
            {
                Drifting = false;
                DriftMode = MapMouseDriftMode.None;
            }
        }

        public override void OnMouseMove(Point p)
        {
            if (Drifting)
            {
                switch (DriftMode)
                {
                    case MapMouseDriftMode.Translate:
                        {
                            float newX = DriftStartX + ((DriftMouseX - (float)p.X));
                            float newY = DriftStartY + ((DriftMouseY - (float)p.Y));

                            EditOriginTranslate(newX, newY);
                        }
                        break;
                    case MapMouseDriftMode.Rotate:
                        {
                            double start = GraphMath.SlopeRadian(CenterPanel.X, CenterPanel.Y, DriftMouseX, DriftMouseY);
                            double stop = GraphMath.SlopeRadian(CenterPanel.X, CenterPanel.Y, (float)p.X, (float)p.Y);

                            double change = GraphMath.DifferentRadian(start, stop);
                            EditOriginRotate((float)GraphMath.RadianTo2PI(DriftStartA - change));

                            RawVector2 vcenter = RawVectorMapToPanel(new RawVector2(DriftCenterX, DriftCenterY));

                            ChangeOriginTranslate(CenterPanel.X - vcenter.X, CenterPanel.Y - vcenter.Y);

                            DriftCenterX = CenterMap.X;
                            DriftCenterY = CenterMap.Y;
                        }
                        break;
                    case MapMouseDriftMode.PoseEstimate:
                        {
                            // 计算直线
                            double theta = GraphMath.SlopeRadian(DriftMouseX, DriftMouseY, (float)p.X, (float)p.Y);
                            double l = PoseEstimateLen * OriginScalePercent;
                            float x = DriftMouseX + (float)(l * Math.Cos(theta));
                            float y = DriftMouseY + (float)(l * Math.Sin(theta));
                            PoseEstimateEnd = RawVectorPanelToMap(new RawVector2(x, y));

                            // 计算三角形
                            double l0 = (PoseEstimateLen - PoseEstimateTriangleLen) * OriginScalePercent;
                            float x0 = DriftMouseX + (float)(l0 * Math.Cos(theta));
                            float y0 = DriftMouseY + (float)(l0 * Math.Sin(theta));

                            double theta1 = theta - Math.PI / 2d;
                            double theta2 = theta + Math.PI / 2d;
                            double l1and2 = PoseEstimateTriangleLen * OriginScalePercent;

                            float x1 = x0 + (float)(l1and2 * Math.Cos(theta1));
                            float y1 = y0 + (float)(l1and2 * Math.Sin(theta1));
                            PoseEstimateEnd1 = RawVectorPanelToMap(new RawVector2(x1, y1));

                            float x2 = x0 + (float)(l1and2 * Math.Cos(theta2));
                            float y2 = y0 + (float)(l1and2 * Math.Sin(theta2));
                            PoseEstimateEnd2 = RawVectorPanelToMap(new RawVector2(x2, y2));
                        }
                        break;
                    default:
                        break;
                }
            }
            MouseLoctPanel = new Vector2((float)p.X, (float)p.Y);
            UpdateMouseLoct();
        }

        public override void OnMouseLeave(Point p)
        {
            if (Drifting)
            {
                Drifting = false;
                DriftMode = MapMouseDriftMode.None;
            }
        }

        public override void OnMouseDown(Point p, bool leftPressed, bool middlePressed, bool rightPressed, int clickCount = 1)
        {
            if (!Drifting)
            {
                DriftMouseX = (float)p.X;
                DriftMouseY = (float)p.Y;
                DriftStartX = OriginTranslate.X;
                DriftStartY = OriginTranslate.Y;
                DriftStartA = OriginRotate;
                DriftCenterX = CenterMap.X;
                DriftCenterY = CenterMap.Y;

                if (rightPressed)
                {
                    if (MouseRotateEnable)
                    {
                        DriftMode = MapMouseDriftMode.Rotate;
                        Drifting = true;
                    }
                }
                else if (PoseEstimateRequested && leftPressed)
                {
                    Parent.Cursor = Cursors.Hand;
                    DriftMode = MapMouseDriftMode.PoseEstimate;
                    Drifting = true;
                    PoseEstimateRequested = false;
                    PoseEstimateEnd1 = PoseEstimateEnd2 = PoseEstimateEnd = PoseEstimateStart = RawVectorPanelToMap(new RawVector2(DriftMouseX, DriftMouseY));
                }
                else if (middlePressed || leftPressed)
                {
                    if (MouseTranslateEnable)
                    {
                        DriftMode = MapMouseDriftMode.Translate;
                        Drifting = true;
                    }
                }
            }
            if (clickCount == 2)
            {
                ResetOriginScale();
            }
        }

        public override void OnMouseUp(Point p, bool leftReleased, bool middleReleased, bool rightReleased)
        {
            if (Drifting)
            {
                if (leftReleased || rightReleased || middleReleased)
                {
                    if (DriftMode != MapMouseDriftMode.None)
                    {
                        Drifting = false;
                        if (DriftMode == MapMouseDriftMode.PoseEstimate)
                        {
                            Parent.Cursor = Cursors.None;
                            PoseEstimateConfirmed?.Invoke(this, new PoseEstimateEventArgs()
                            {
                                Start = new RawVector2(PoseEstimateStart.X / 1000f, PoseEstimateStart.Y / 1000f),
                                End = new RawVector2(PoseEstimateEnd.X / 1000f, PoseEstimateEnd.Y / 1000f),
                                Theta = (float)GraphMath.RadianToPI(GraphMath.SlopeRadian(PoseEstimateStart.X, PoseEstimateStart.Y, PoseEstimateEnd.X, PoseEstimateEnd.Y)),
                            });
                        }
                        DriftMode = MapMouseDriftMode.None;
                    }
                }
            }
        }

        public override void OnMouseWheel(Point p, int delta)
        {
            if (!MouseScaleEnable)
            {
                return;
            }
            if (delta == 0)
            {
                return;
            }

            RawVector2 start_p = new RawVector2((float)p.X, (float)p.Y);
            RawVector2 start_m = RawVectorPanelToMap(start_p);

            if (delta > 0)
            {
                ChangeOriginScale(0.9f);
            }
            else if (delta < 0)
            {
                ChangeOriginScale(1.1f);
            }

            RawVector2 stop_p = RawVectorMapToPanel(start_m);
            ChangeOriginTranslate(start_p.X - stop_p.X, start_p.Y - stop_p.Y);
        }

        public void RequestPoseEstimate()
        {
            PoseEstimateRequested = true;
        }
    }
}

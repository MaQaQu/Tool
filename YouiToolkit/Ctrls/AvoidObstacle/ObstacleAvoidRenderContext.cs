using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Windows;
using System.Windows.Input;
using YouiToolkit.Design.DirectX;
using Color = SharpDX.Color;
using Factory = SharpDX.Direct2D1.Factory;
using Point = System.Windows.Point;

namespace YouiToolkit.Ctrls
{
    internal class AvoidObstacleRenderContext : DirectXRenderContext
    {
        public FrameworkElement Parent { get; set; } = null;

        #region [渲染标识]
        public bool ShowCenter { get; set; } = false;
        public bool ShowGrid { get; set; } = true;
        public bool ShowLidarRange { get; set; } = true;
        public bool ShowOriginAxis { get; set; } = true;
        public bool ShowBaselinkAxis { get; set; } = true;
        public bool ShowPointCloud { get; set; } = true;
        #endregion

        #region [交互控制]
        public AvoidObstacleMouseDriftMode DriftMode { get; internal set; } = AvoidObstacleMouseDriftMode.None;
        public bool Drifting { get; internal set; } = false;
        public float DriftStartX { get; internal set; } = 0f;
        public float DriftStartY { get; internal set; } = 0f;
        public float DriftStartA { get; internal set; } = 0f;
        public float DriftMouseX { get; internal set; } = 0f;
        public float DriftMouseY { get; internal set; } = 0f;
        public float DriftCenterX { get; internal set; } = 0f;
        public float DriftCenterY { get; internal set; } = 0f;
        #endregion

        #region [颜色定义]
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackColor { get; internal set; } = Color.White;

        /// <summary>
        /// 遮罩颜色
        /// </summary>
        public Color MaskColor { get; internal set; } = new Color(191, 191, 191);

        /// <summary>
        /// 网格颜色
        /// </summary>
        public Color GridColor { get; internal set; } = new Color(143, 143, 145);

        /// <summary>
        /// 实时点云颜色
        /// </summary>
        public Color PointCloudColor { get; internal set; } = new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)(byte.MaxValue / 2));

        /// <summary>
        /// 原点坐标系颜色
        /// </summary>
        public Color OriginAxisColor { get; internal set; } = Color.Black;
        public Color OriginPointColor { get; internal set; } = Color.Gray;
        #endregion

        public double OriginAxisSize = 500;
        public float LidarRange = 60000f;
        public override float OriginScaleDefault => 150f;
        public override float OriginScaleMax => 150f;
        public override float OriginScaleMin => 50f;

        /// <summary>
        /// TODO
        /// </summary>
        private Rect mouseLoctLimit => new Rect(new Point(-LidarRange, -LidarRange), new Size(LidarRange, LidarRange));
        public override Rect? MouseLoctMapLimit => mouseLoctLimit;

        #region [特定渲染变量]
        public AvoidObstacleLayer Layer { get; set; } = null;
        #endregion

        public AvoidObstacleRenderContext(FrameworkElement parent, RenderTarget renderTarget, Factory factory)
            : this(parent)
        {
            InitDirectX(renderTarget, factory);
        }

        public AvoidObstacleRenderContext(FrameworkElement parent)
        {
            Parent = parent;
            Layer = new AvoidObstacleLayer();
            InitMatrix();
            ChangeOriginScale(OriginScaleDefault);
        }

        public override string ToString(string propertyName)
        {
            if (propertyName == nameof(MouseLoctMap))
            {
                float theta = (MouseLoctMap.X == default && MouseLoctMap.Y == default) ? default : (float)(Math.Atan2(-MouseLoctMap.X, MouseLoctMap.Y) / Math.PI * 180d);

                return $"X: {MouseLoctMap.Y / 10f:0} [mm], Y: {-MouseLoctMap.X / 10f:0} [mm], R: {Math.Sqrt(Math.Pow(MouseLoctMap.X / 10d, 2d) + Math.Pow(MouseLoctMap.Y / 10d, 2d)):0} [mm], Th: {theta:0.0} [deg]";
            }
            return base.ToString(propertyName);
        }

        public override void OnTransMatrixUpdated()
        {
            Layer.UpdateTransMatrix(TransPannelToMap, TransMapToPanel);
        }

        public override void OnLostFocus()
        {
            if (Layer.OnLostFocus())
                return;

            if (Drifting)
            {
                Drifting = false;
                DriftMode = AvoidObstacleMouseDriftMode.None;
            }
        }

        public override void OnMouseMove(Point p)
        {
            if (Layer.OnMouseMove(p))
                return;

            if (Drifting)
            {
                switch (DriftMode)
                {
                    case AvoidObstacleMouseDriftMode.Translate:
                        {
                            float newX = DriftStartX + ((DriftMouseX - (float)p.X));
                            float newY = DriftStartY + ((DriftMouseY - (float)p.Y));

                            //TryEditOriginTranslate(newX, newY);
                            EditOriginTranslate(newX, newY);
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
            if (Layer.OnMouseLeave(p))
                return;

            if (Drifting)
            {
                Drifting = false;
                DriftMode = AvoidObstacleMouseDriftMode.None;
            }
        }

        public override void OnMouseDown(Point p, bool leftPressed, bool middlePressed, bool rightPressed, int clickCount = 1)
        {
            if (Layer.OnMouseDown(p, leftPressed, middlePressed, rightPressed))
                return;

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
                        DriftMode = AvoidObstacleMouseDriftMode.Rotate;
                        Drifting = true;
                    }
                }
                else if (middlePressed || leftPressed)
                {
                    if (MouseTranslateEnable)
                    {
                        DriftMode = AvoidObstacleMouseDriftMode.Translate;
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
            if (Layer.OnMouseUp(p, leftReleased, middleReleased, rightReleased))
                return;

            if (Drifting)
            {
                if (leftReleased || rightReleased || middleReleased)
                {
                    if (DriftMode != AvoidObstacleMouseDriftMode.None)
                    {
                        Drifting = false;
                        DriftMode = AvoidObstacleMouseDriftMode.None;
                    }
                }
            }
        }

        public override void OnMouseWheel(Point p, int delta)
        {
            if (Layer.OnMouseWheel(p, delta))
                return;

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

            Layer?.OnMouseWheel(p, delta);
        }

        public override void OnKeyDown(Key key, ModifierKeys modifiers)
        {
            if (Layer.OnKeyDown(key, modifiers))
                return;
        }

        public override void OnKeyUp(Key key, ModifierKeys modifiers)
        {
            if (Layer.OnKeyUp(key, modifiers))
                return;
        }
    }

    /// <summary>
    /// 鼠标漂移模式
    /// </summary>
    public enum AvoidObstacleMouseDriftMode
    {
        /// <summary>
        /// 默认
        /// </summary>
        None,

        /// <summary>
        /// 平移
        /// </summary>
        Translate,

        /// <summary>
        /// 旋转
        /// </summary>
        Rotate,
    }

    /// <summary>
    /// 激光雷达有效角
    /// </summary>
    public enum AvoidObstacleAngle
    {
        _180 = 180,
        _270 = 270,
        _360 = 360,
    }
}

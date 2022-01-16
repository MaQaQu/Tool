using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SuperMath;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Matrix = SuperMath.Matrix;
using Vector2 = SharpDX.Vector2;

namespace YouiToolkit.Design.DirectX
{
    public abstract class DirectXRenderContext : DirectXPropertyChangedBase
    {
        #region [渲染接口]
        protected RenderTarget d2DRenderTarget;
        public RenderTarget D2DRenderTarget
        {
            get => d2DRenderTarget;
            set => d2DRenderTarget = value;
        }

        protected Factory d2DFactory;
        public Factory D2DFactory
        {
            get => d2DFactory;
            set => d2DFactory = value;
        }
        #endregion

        public Matrix TransPannelToMap = null;
        public Matrix TransMapToPanel = null;
        public bool MouseTranslateEnable { get; internal set; } = true;
        public bool MouseRotateEnable { get; internal set; } = true;
        public bool MouseScaleEnable { get; internal set; } = true;
        public float PanelWidth { get; internal set; } = 800f;
        public float PanelHeight { get; internal set; } = 600f;
        public Vector2 CenterPanel { get; internal set; } = new Vector2(0, 0);
        public Vector2 CenterMap { get; internal set; } = new Vector2(0, 0);
        public float RegionRadius { get; internal set; } = 0f;

        /// <summary>
        /// 鼠标位置（单位：米）
        /// ※状态栏显示
        /// </summary>
        protected Vector2 mouseLoctMap = new Vector2(0, 0);
        public Vector2 MouseLoctMap
        {
            get => mouseLoctMap;
            internal set
            {
                if (OnPropertyChanging(mouseLoctMap, value, nameof(MouseLoctMap)))
                {
                    mouseLoctMap = value;
                    RaisePropertyChanged(nameof(MouseLoctMap));
                }
            }
        }

        /// <summary>
        /// 鼠标位置拖动限制范围（单位：米）
        public virtual Rect? MouseLoctMapLimit => null;

        /// <summary>
        /// 鼠标位置（单位：像素）
        /// </summary>
        public Vector2 MouseLoctPanel { get; protected set; } = new Vector2(0, 0);

        /// <summary>
        /// 原点位移目标点（单位：像素）
        /// </summary>
        protected Vector2 originTranslate = new Vector2(0, 0);
        public Vector2 OriginTranslate
        {
            get => originTranslate;
            internal set
            {
                originTranslate = value;
                RaisePropertyChanged(nameof(OriginTranslate));
            }
        }

        public bool OriginFlipX { get; internal set; } = false;
        public bool OriginFlipY { get; internal set; } = false;

        /// <summary>
        /// 缩放矩阵单位
        /// </summary>
        protected float originScale = 1f;
        [DefaultValue(1f)]
        public float OriginScale
        {
            get => originScale;
            internal set
            {
                originScale = value;
                RaisePropertyChanged(nameof(OriginScale));
            }
        }

        /// <summary>
        /// 缩放比例估算
        /// </summary>
        public float OriginScalePercent => OriginScaleDefault / OriginScale;

        /// <summary>
        /// 缩放矩阵单位初始值
        /// </summary>
        public virtual float OriginScaleDefault => 120f;

        /// <summary>
        /// 缩放矩阵单位最大值
        /// </summary>
        public virtual float OriginScaleMax => 460f;

        /// <summary>
        /// 缩放矩阵单位最小值
        /// </summary>
        public virtual float OriginScaleMin => 0.6f;

        /// <summary>
        /// 旋转角度
        /// </summary>
        public float originRotate = 0f;
        [DefaultValue(0f)]
        public float OriginRotate
        {
            get => originRotate;
            internal set
            {
                originRotate = value;
                RaisePropertyChanged(nameof(OriginRotate));
            }
        }

        protected override bool OnPropertyChanging(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(MouseLoctMap))
            {
            }
            return true;
        }

        /// <summary>
        /// 状态参数可视化
        /// </summary>
        /// <param name="propertyName">参数属性</param>
        /// <returns>可视化字符串</returns>
        public virtual string ToString(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(MouseLoctMap):
                    // 单位：米
                    return $"{MouseLoctMap.X / 1000f:0}, {MouseLoctMap.Y / 1000f:0}";
                case nameof(OriginTranslate):
                    // 单位：像素
                    return $"{-OriginTranslate.X:0}, {-OriginTranslate.Y:0}";
                case nameof(OriginRotate):
                    {
                        // 单位：°
                        double degree = Math.Round(GraphMath.DegreeOfF(OriginRotate));
                        return $"{(degree >= 360d ? 0d : degree)}";
                    }
                case nameof(OriginScale):
                    // 单位：%
                    return $"{Math.Round(OriginScaleDefault / OriginScale * 100f)}%";
            }
            return string.Empty;
        }

        /// <summary>
        /// 初始化DirectX
        /// </summary>
        public void InitDirectX(RenderTarget renderTarget, Factory factory)
        {
            D2DRenderTarget = renderTarget;
            D2DFactory = factory;
        }

        public void InitMatrix()
        {
            OriginTranslate = new Vector2(0, 0);
            OriginScale = 1;
            OriginRotate = 0;
            OriginFlipX = false;
            OriginFlipY = false;

            TransPannelToMap = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
            TransMapToPanel = Matrix.Inverse(TransPannelToMap);
            OnTransMatrixUpdated();
        }

        public virtual void OnTransMatrixUpdated()
        {
        }

        public void SetPanelSize(float width, float height)
        {
            if (width > 0)
            {
                PanelWidth = width;
            }
            if (height > 0)
            {
                PanelHeight = height;
            }
            UpdateCenter();
        }

        public void ChangeOriginTranslate(float deltaX, float deltaY)
        {
            if (deltaX != 0 || deltaY != 0)
            {
                OriginTranslate = new Vector2(OriginTranslate.X - deltaX, OriginTranslate.Y - deltaY);
                UpdateTransMatrix();
            }
        }

        public void ChangeOriginScale(float deltaScale)
        {
            if (deltaScale > 0)
            {
                if (OriginScale * deltaScale >= OriginScaleMax)
                {
                    OriginScale = OriginScaleMax;
                }
                else if (OriginScale * deltaScale <= OriginScaleMin)
                {
                    OriginScale = OriginScaleMin;
                }
                else
                {
                    OriginScale *= deltaScale;
                }
                UpdateTransMatrix();
            }
        }

        public void UpdateOriginScale(float originScale)
        {
            if (originScale > 0)
            {
                OriginScale = originScale;
                UpdateTransMatrix();
            }
        }

        public void ChangeOriginRotate(float deltaA)
        {
            if (deltaA != 0)
            {
                OriginRotate += deltaA;
                UpdateTransMatrix();
            }
        }

        public void ResetOriginTranslate(float x, float y)
        {
            OriginTranslate = new Vector2(-x, -y);
            UpdateTransMatrix();
        }

        protected void ResetOriginScale() => UpdateOriginScale(OriginScaleDefault);

        public void ResetOriginRotate()
        {
            OriginRotate = default;
            UpdateTransMatrix();
        }

        public void ResetOrigin(float x, float y)
        {
            OriginTranslate = new Vector2(-x, -y);
            OriginRotate = default;
            OriginScale = OriginScaleDefault;
            UpdateTransMatrix();
        }

        public void FlipOriginScaleX()
        {
            OriginFlipX = !OriginFlipX;
            UpdateTransMatrix();
        }

        public void FlipOriginScaleY()
        {
            OriginFlipY = !OriginFlipY;
            UpdateTransMatrix();
        }

        public void EditOriginTranslate(float x, float y)
        {
            if (OriginTranslate.X != x || OriginTranslate.Y != y)
            {
                OriginTranslate = new Vector2(x, y);
                UpdateTransMatrix();
            }
        }

        public void TryEditOriginTranslate(float x, float y)
        {
            if (OriginTranslate.X != x || OriginTranslate.Y != y)
            {
                Vector2 tryOriginTranslate = new Vector2(x, y);

                // UpdateTransMatrix
                double sin = Math.Sin(OriginRotate);
                double cos = Math.Cos(OriginRotate);
                Matrix rotate = new Matrix(new double[,] { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } });
                Matrix translate = new Matrix(new double[,] { { 1, 0, OriginTranslate.X }, { 0, 1, OriginTranslate.Y }, { 0, 0, 1 } });
                Matrix origin = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });

                if (OriginFlipX)
                {
                    origin[0, 0] = -1;
                }

                if (OriginFlipY)
                {
                    origin[1, 1] = -1;
                }

                if (OriginScale != 1)
                {
                    origin[0, 0] = origin[0, 0] * OriginScale;
                    origin[1, 1] = origin[1, 1] * OriginScale;
                }

                Matrix tryTransPannelToMap = origin * rotate * translate;
                Matrix tryTransMapToPanel = Matrix.Inverse(TransPannelToMap);

                // UpdateCenter
                Vector2 tryCenterPanel = new Vector2(PanelWidth / 2.0f, PanelHeight / 2.0f);
                RawVector2 cmap = RawVectorPanelToMap(new RawVector2(CenterPanel.X, CenterPanel.Y));
                Vector2 tryCenterMap = new Vector2(cmap.X, cmap.Y);

                RawVector2 zero = RawVectorPanelToMap(new RawVector2(0, 0));
                float tryRegionRadius = (float)GraphMath.DistancePointToPoint(CenterMap.X, CenterMap.Y, zero.X, zero.Y);

                // UpdateMouseLoct
                Matrix input = new Matrix(new double[,] { { MouseLoctPanel.X }, { MouseLoctPanel.Y }, { 1 } });
                Matrix output = TransPannelToMap * input;
                Vector2 tryMouseLoctMap = new Vector2((float)output[0, 0], (float)output[1, 0]);

                if (MouseLoctMapLimit != null)
                {
                    Rect limit = (Rect)MouseLoctMapLimit;

                    if (tryMouseLoctMap.X >= limit.X
                        && tryMouseLoctMap.X <= limit.Width
                        && tryMouseLoctMap.Y >= limit.Y
                        && tryMouseLoctMap.Y <= limit.Height
                        || true)
                    {
                        // 可以更新
                        OriginTranslate = tryOriginTranslate;
                        TransPannelToMap = tryTransPannelToMap;
                        TransMapToPanel = tryTransMapToPanel;
                        OnTransMatrixUpdated();
                        CenterPanel = tryCenterPanel;
                        CenterMap = tryCenterMap;
                        RegionRadius = tryRegionRadius;
                        MouseLoctMap = tryMouseLoctMap;
                    }
                }
            }
        }

        public void EditOriginScale(float scale)
        {
            if (OriginScale != scale && scale > 0)
            {
                OriginScale = scale;
                UpdateTransMatrix();
            }
        }

        public void EditOriginRotate(float rad)
        {
            if (OriginRotate != rad)
            {
                OriginRotate = (float)GraphMath.RadianTo2PI(rad);
                UpdateTransMatrix();
            }
        }

        public RawVector2 RawVectorPanelToMap(RawVector2 vec)
        {
            Matrix input = new Matrix(new double[,] { { vec.X }, { vec.Y }, { 1 } });
            Matrix output = TransPannelToMap * input;
            return new RawVector2((float)output[0, 0], (float)output[1, 0]);
        }

        public RawVector2 RawVectorMapToPanel(RawVector2 vec)
        {
            Matrix input = new Matrix(new double[,] { { vec.X }, { vec.Y }, { 1 } });
            Matrix output = TransMapToPanel * input;
            return new RawVector2((float)output[0, 0], (float)output[1, 0]);
        }

        public void UpdateCenter()
        {
            CenterPanel = new Vector2(PanelWidth / 2.0f, PanelHeight / 2.0f);
            RawVector2 cmap = RawVectorPanelToMap(new RawVector2(CenterPanel.X, CenterPanel.Y));
            CenterMap = new Vector2(cmap.X, cmap.Y);

            RawVector2 zero = RawVectorPanelToMap(new RawVector2(0, 0));
            RegionRadius = (float)GraphMath.DistancePointToPoint(CenterMap.X, CenterMap.Y, zero.X, zero.Y);
        }

        public void UpdateTransMatrix()
        {
            double sin = Math.Sin(OriginRotate);
            double cos = Math.Cos(OriginRotate);
            Matrix rotate = new Matrix(new double[,] { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } });
            Matrix translate = new Matrix(new double[,] { { 1, 0, OriginTranslate.X }, { 0, 1, OriginTranslate.Y }, { 0, 0, 1 } });
            Matrix origin = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });

            if (OriginFlipX)
            {
                origin[0, 0] = -1;
            }

            if (OriginFlipY)
            {
                origin[1, 1] = -1;
            }

            if (OriginScale != 1)
            {
                origin[0, 0] = origin[0, 0] * OriginScale;
                origin[1, 1] = origin[1, 1] * OriginScale;
            }

            TransPannelToMap = origin * rotate * translate;
            TransMapToPanel = Matrix.Inverse(TransPannelToMap);
            OnTransMatrixUpdated();

            UpdateCenter();
            UpdateMouseLoct();
        }

        public void UpdateMouseLoct()
        {
            Matrix input = new Matrix(new double[,] { { MouseLoctPanel.X }, { MouseLoctPanel.Y }, { 1 } });
            Matrix output = TransPannelToMap * input;
            MouseLoctMap = new Vector2((float)output[0, 0], (float)output[1, 0]);
        }

        public virtual void OnGotFocus()
        {
        }

        public virtual void OnLostFocus()
        {
        }

        public virtual void OnMouseMove(Point p)
        {
        }

        public virtual void OnMouseEnter(Point p)
        {
        }

        public virtual void OnMouseLeave(Point p)
        {
        }

        public virtual void OnMouseDown(Point p, bool leftPressed, bool middlePressed, bool rightPressed, int clickCount = 1)
        {
        }

        public virtual void OnMouseUp(Point p, bool leftReleased, bool middleReleased, bool rightReleased)
        {
        }

        public virtual void OnMouseWheel(Point p, int delta)
        {
        }

        public virtual void OnKeyDown(Key key, ModifierKeys modifiers)
        {
        }

        public virtual void OnKeyUp(Key key, ModifierKeys modifiers)
        {
        }
    }
}

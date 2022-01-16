using SharpDX;
using System;
using YouiToolkit.Design;
using YouiToolkit.Design.DirectX;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;

namespace YouiToolkit.Models
{
    /// <summary>
    /// 避障实体：拖动点
    /// </summary>
    internal class AvoidObstacleDragPoint : DirectXEntity, IComparable
    {
        public static readonly RectangleF ClientRectangle = new RectangleF(-1000, -1000, 2000, 2000);
        public static Color DefaultColor { get; set; } = new Color(0, 0, 240);
        public static Color HoverColor { get; set; } = new Color(255, 64, 64);
        public bool IsMouseHover { get; set; } = false;
        public float Theta { get; private set; } = 0f;
        public bool IsOriginal { get; set; } = false;
        public float Radius => (float)Math.Sqrt(Math.Pow(Math.Abs(X), 2d) + Math.Pow(Math.Abs(Y), 2d));

        public AvoidObstacleDragPoint(float x, float y, bool isOriginal = false)
        {
            IsOriginal = isOriginal;
            SetPos(x, y);
        }

        public void SetPos(float x, float y)
        {
            X = x;
            Y = y;

            if (IsOriginal)
            {
                Theta = 0;
            }
            else
            {
#if DEBUG
                Theta = (float)(Math.Atan2(x, -y) / Math.PI * 180d);
#else
                Theta = (float)Math.Atan2(x, -y);
#endif
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is AvoidObstacleDragPoint right)
            {
                if (Theta > right.Theta)
                {
                    return 1;
                }
                else if (Theta == right.Theta)
                {
                    return 0;
                }
            }
            return -1;
        }

        public void CopyTo(AvoidObstacleDragPoint entity)
        {
            entity.X = X;
            entity.Y = Y;
            entity.Theta = Theta;
            entity.IsOriginal = IsOriginal;
        }

        public bool Contains(PointF p)
        {
            RectangleF rect = this;
            return rect.Contains(p);
        }

        public virtual RectangleF ToRect() => new RectangleF(X + ClientRectangle.X, Y + ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        public static implicit operator RectangleF(AvoidObstacleDragPoint self) => self.ToRect();

        public PointF ToPointF() => new PointF(X, Y);
        public static implicit operator PointF(AvoidObstacleDragPoint self) => self.ToPointF();

        public PolarF ToPolarF() => PolarF.FromPoint(X, Y);
        public static implicit operator PolarF(AvoidObstacleDragPoint self) => self.ToPolarF();

        public override string ToString() => $"X:{X}, Y:{Y}, Theta:{Theta}{(IsOriginal ? " (Origin)" : string.Empty)}";
    }
}

using System;

namespace YouiToolkit.Design
{
    /// <summary>
    /// 极坐标系点
    /// </summary>
    public struct PolarF
    {
        /// <summary>
        /// 空值
        /// </summary>
        public static readonly PolarF Empty = new();

        /// <summary>
        /// 空值判断
        /// </summary>
        public bool IsEmpty => p == 0f && theta == 0f;

        /// <summary>
        /// 极径ρ
        /// </summary>
        private float p;
        public float P
        {
            get => p;
            set => p = value;
        }

        /// <summary>
        /// 极角θ
        /// </summary>
        private float theta;
        public float Theta
        {
            get => theta;
            set => theta = value;
        }

        /// <summary>
        /// 构造
        /// </summary>
        public PolarF(float p, float theta)
        {
            this.p = p;
            this.theta = theta;
        }

        /// <summary>
        /// 构造
        /// </summary>
        public PolarF() : this(default, default)
        {
        }

        /// <summary>
        /// 转数组结构
        /// </summary>
        public float[] ToFloatArray()
            => new float[] { p, theta };

        /// <summary>
        /// 构造
        /// </summary>
        public static PolarF FromPoint(float x, float y, float scale = 1f)
        {
            x *= scale;
            y *= scale;

            float p = (float)Math.Sqrt(Math.Pow(x, 2d) + Math.Pow(y, 2d));
            float theta = (float)Math.Atan2(x, -y);

            return new PolarF(p, theta);
        }
    }
}

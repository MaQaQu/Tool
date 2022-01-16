using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace YouiToolkit.Models
{
    /// <summary>
    /// 避障实体：拖动点集合块
    /// <remark>（包含减速/停止/急停区点）</remark>
    /// </summary>
    internal class AvoidObstacleDragPointSetBlock
    {
        /// <summary>
        /// 拖点绘制控制器们
        /// </summary>
        public AvoidObstacleDragPointSet[] DragPointSets { get; private set; } = null;

        /// <summary>
        /// 交互操作生效中：拖点绘制控制器
        /// </summary>
        public AvoidObstacleDragPointSet Actived => Array.Find(DragPointSets, set => set.IsActivted);

        /// <summary>
        /// 是否为选中集合块
        /// </summary>
        public bool IsSelected { get; set; } = false;

        /// <summary>
        /// 索引编号
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// 渲染序：拖点绘制控制器
        /// </summary>
        public AvoidObstacleDragPointSet[] RenderSorted
        {
            get
            {
                var activted = Actived;

                if (activted == null)
                {
                    return DragPointSets;
                }

                var sorted = new List<AvoidObstacleDragPointSet>();

                foreach (var set in DragPointSets)
                {
                    if (set != activted)
                    {
                        sorted.Add(set);
                    }
                }
                sorted.Add(activted);
                return sorted.ToArray();
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public AvoidObstacleDragPointSetBlock()
        {
            DragPointSets = new AvoidObstacleDragPointSet[3]
            {
                new AvoidObstacleDragPointSet(AvoidObstacleAreaTypeAs.Decelerate),
                new AvoidObstacleDragPointSet(AvoidObstacleAreaTypeAs.Stop),
                new AvoidObstacleDragPointSet(AvoidObstacleAreaTypeAs.EmergencyStop),
            };
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            foreach (var set in DragPointSets)
            {
                set?.Clear();
            }
        }

        /// <summary>
        /// 激活编辑类型
        /// </summary>
        public void Active(AvoidObstacleAreaTypeAs typeAs)
            => DragPointSets.Any(set => (set.IsActivted = set.TypeAs == typeAs) && false);

        /// <summary>
        /// 数据拷贝
        /// </summary>
        public void CopyTo(AvoidObstacleDragPointSetBlock setBlock)
        {
            DragPointSets.CopyTo(setBlock.DragPointSets, default);
        }

        /// <summary>
        /// 取得真实渲染区域
        /// </summary>
        public SizeF RealRegion()
        {
            float maxX = default;
            float maxY = default;

            float curX;
            float curY;
            foreach (var set in DragPointSets)
            {
                foreach (var point in set.DragPoints)
                {
                    if (point == null) continue;
                    curX = Math.Abs(point.X);
                    curY = Math.Abs(point.Y);
                    if (curX > maxX)
                        maxX = curX;
                    if (curY > maxY)
                        maxY = curY;
                }
            }
            return new SizeF(maxX * 2f, maxY * 2f);
        }

        /// <summary>
        /// 创建GDI+缩略图
        /// </summary>
        public Bitmap CreateThumbnail(int width = 200, int height = 200, bool debug = false)
        {
            var bitmap = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(bitmap);
            SizeF realRegion = RealRegion();
            float realRadius = Math.Max(realRegion.Width, realRegion.Height);
            realRegion = new SizeF(realRadius, realRadius);
            float scale = Math.Min(width / realRegion.Width, height / realRegion.Height);

            if (debug)
            {
                g.Clear(Color.White);
            }
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            foreach (var set in DragPointSets)
            {
                using Brush brush = new SolidBrush(Color.FromArgb(Math.Min(byte.MaxValue, set.PolygonColor.A + 10), set.PolygonColor.R, set.PolygonColor.G, set.PolygonColor.B));
                g.FillPolygon(brush, set.ToPoints(new PointF(realRegion.Width / 2f, realRegion.Height / 2f), new SizeF(scale, scale)));
            }
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            if (debug)
            {
                using Pen pen = new Pen(Color.FromArgb(191, 191, 191));
                pen.DashStyle = DashStyle.Dot;
                pen.Width = 0.5f;
                g.DrawLine(pen, new PointF(0f, height / 2f), new PointF(width, height / 2));
                g.DrawLine(pen, new PointF(width / 2f, 0f), new PointF(width / 2f, height));
            }
            return bitmap;
        }

        public object[][] ToCtorParamsArrayArray()
        {
            var pointArrays = new object[DragPointSets.Length][];

            for (int i = 0; i < pointArrays.Length; i++)
            {
                pointArrays[i] = DragPointSets[i].ToCtorParamsArray();
            }
            return pointArrays;
        }

        public void FromCtorParamsArrayArray(object[][] pointArrays)
        {
            for (int i = 0; i < pointArrays.Length; i++)
            {
                DragPointSets[i].FromCtorParamsArray(pointArrays[i]);
            }
        }
    }
}

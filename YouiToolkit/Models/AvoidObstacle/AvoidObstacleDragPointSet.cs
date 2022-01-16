using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YouiToolkit.Design;
using YouiToolkit.Logging;
using YouiToolkit.Utils;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;

namespace YouiToolkit.Models
{
    /// <summary>
    /// 避障实体：拖动点集合
    /// </summary>
    internal class AvoidObstacleDragPointSet
    {
        /// <summary>
        /// 拖点绘制最大个数
        /// </summary>
        public const int DragPointsMax = 32;

        /// <summary>
        /// 交互操作是否生效中
        /// </summary>
        public bool IsActivted { get; set; } = false;

        /// <summary>
        /// 多边形颜色
        /// </summary>
        public Color PolygonColor { get; set; } = default;

        /// <summary>
        /// 类型
        /// </summary>
        public AvoidObstacleAreaTypeAs TypeAs { get; set; } = default;

        /// <summary>
        /// 拖点绘制元素
        /// </summary>
        public AvoidObstacleDragPoint[] DragPoints { get; private set; } = null;

        /// <summary>
        /// 拖中点元素
        /// </summary>
        public AvoidObstacleDragPoint Selected { get; internal set; } = null;

        /// <summary>
        /// 构造
        /// </summary>
        public AvoidObstacleDragPointSet(AvoidObstacleAreaTypeAs typeAs = default)
        {
            TypeAs = typeAs;
            PolygonColor = typeAs switch
            {
                AvoidObstacleAreaTypeAs.Decelerate => new Color(255, 255, 75, 180),
                AvoidObstacleAreaTypeAs.Stop => new Color(255, 181, 22, 180),
                AvoidObstacleAreaTypeAs.EmergencyStop => new Color(255, 56, 6, 180),
                _ => new Color(0, 0, 0, 128),
            };
            IsActivted = typeAs switch
            {
                AvoidObstacleAreaTypeAs.Decelerate => true,
                _ => false,
            };
            DragPoints = new AvoidObstacleDragPoint[DragPointsMax];
            DragPoints[0] = new AvoidObstacleDragPoint(default, default, true);
            DebugAppend();
        }

        /// <summary>
        /// 追加拖点绘制元素
        /// </summary>
        public void Append(AvoidObstacleDragPoint entity)
        {
            int count = Count();

            if (count < DragPointsMax)
            {
                DragPoints[count - 1] = entity;
                Sort();
            }
        }

        /// <summary>
        /// 删除拖点绘制元素
        /// </summary>
        public void Remove(AvoidObstacleDragPoint entity)
        {
            if (entity != null)
            {
                int index = Array.IndexOf(DragPoints, entity);

                if (index >= 0)
                {
                    DragPoints[index] = null;
                    Arrange(index);
                    Sort();
                }
            }
        }

        /// <summary>
        /// 整理拖点绘制元素
        /// </summary>
        private void Arrange(int index)
        {
            for (int i = index; i + 1 < DragPoints.Length; i++)
            {
                DragPoints[i] = DragPoints[i + 1];
                DragPoints[i + 1] = null;
            }
        }

        /// <summary>
        /// 快排拖点绘制元素
        /// </summary>
        public void Sort()
        {
            SortAlgorithm.QuickSort(DragPoints, 0, IndexOfLast());
            DebugPrint();
        }

        /// <summary>
        /// 最后一点下标
        /// </summary>
        public int IndexOfLast()
        {
            int indexOfNull = Array.IndexOf(DragPoints, null);
            int right = indexOfNull < 0 ? DragPoints.Length - 1 : indexOfNull;
            return right;
        }

        /// <summary>
        /// 计算总数
        /// </summary>
        public int Count()
        {
            int indexOfNull = Array.IndexOf(DragPoints, null);
            int right = indexOfNull < 0 ? DragPoints.Length : indexOfNull + 1;
            return Math.Max(right, default);
        }

        /// <summary>
        /// 判断点是否在拖点绘制元素中
        /// </summary>
        public bool Inside(PointF p, out AvoidObstacleDragPoint entity)
        {
            for (int i = default; i < DragPoints.Length; i++)
            {
                var polygon = DragPoints[i];

                if (polygon?.Contains(p) ?? false)
                {
                    entity = polygon;
                    return true;
                }
            }
            entity = null;
            return false;
        }

        /// <summary>
        /// 清除拖点绘制元素
        /// </summary>
        public void Clear()
        {
            for (int i = default; i < DragPoints.Length; i++)
            {
                var polygon = DragPoints[i];

                if (!(polygon?.IsOriginal ?? false))
                {
                    DragPoints[i] = null;
                }
            }
            Sort();
        }

        /// <summary>
        /// 清除拖点绘制元素特效
        /// </summary>
        public void ClearFx()
        {
            DragPoints.Any((polygon) =>
            {
                if (polygon != null)
                    polygon.IsMouseHover = false;
                return false;
            });
        }

        /// <summary>
        /// 数据拷贝
        /// </summary>
        public void CopyTo(AvoidObstacleDragPointSet set)
        {
            DragPoints.CopyTo(set.DragPoints, default);
        }

        /// <summary>
        /// 获取拖点
        /// </summary>
        public RawVector2[] ToArray()
        {
            var points = new List<RawVector2>(DragPointsMax);

            foreach (AvoidObstacleDragPoint entity in DragPoints)
            {
                if (entity != null)
                    points.Add(entity);
            }
            return points.ToArray();
        }

        public object[] ToCtorParamsArray()
        {
            var ctorParamsArray = new List<object>(DragPointsMax);

            foreach (AvoidObstacleDragPoint entity in DragPoints)
            {
                if (entity != null)
                {
                    ctorParamsArray.Add(new { x = entity.X, y = entity.Y, o = entity.IsOriginal });
                }
            }
            return ctorParamsArray.ToArray();
        }

        public void FromCtorParamsArray(object[] ctorParamsArray)
        {
            Clear();
            for (int i = default; i < ctorParamsArray.Length; i++)
            {
                dynamic @dynamic = ctorParamsArray[i];

                DragPoints[i] = new AvoidObstacleDragPoint(x: (float)@dynamic.x.Value, y: (float)@dynamic.y.Value, isOriginal: @dynamic.o.Value);
            }
        }

        /// <summary>
        /// 获取拖点
        /// </summary>
        public PointF[] ToPoints(PointF offset, SizeF scale)
        {
            var points = new List<PointF>(DragPointsMax);

            foreach (AvoidObstacleDragPoint entity in DragPoints)
            {
                if (entity != null)
                {
                    points.Add(new PointF((entity.X + offset.X) * scale.Width, (entity.Y + offset.Y) * scale.Height));
                }
            }
            return points.ToArray();
        }

        /// <summary>
        /// 获取拖点（极坐标系）
        /// </summary>
        public PolarF[] ToPolars()
        {
            var polars = new List<PolarF>(DragPointsMax);

            foreach (AvoidObstacleDragPoint entity in DragPoints)
            {
                if (entity != null && !entity.IsOriginal)
                {
                    PolarF polar = PolarF.FromPoint(entity.X, entity.Y, 1 / 10000f);

                    polars.Add(polar);
                }
            }
            return polars.ToArray();
        }

        /// <summary>
        /// 测试快排拖点绘制元素
        /// </summary>
        [Conditional("DEBUG")]
        public void DebugSort()
        {
            Sort();
            DebugPrint();
        }

        /// <summary>
        /// 打印测试
        /// </summary>
        [Conditional("DEBUG_PRINT")]
        public void DebugPrint()
        {
            Logger.Info("--- DebugSortDragPoints --- | started");
            foreach (var dragPoint in DragPoints)
            {
                if (dragPoint != null)
                {
                    Logger.Info(dragPoint.ToString());
                }
            }
            Logger.Info("--- DebugSortDragPoints --- | ended");
        }

        /// <summary>
        /// 测试添加拖点绘制元素
        /// </summary>
        [Conditional("DEBUG_PRINT")]
        public void DebugAppend()
        {
            var random = new Random();
            for (int i = default; i < DragPointsMax; i++)
            {
                if (i == default)
                {
                    DragPoints[i] = new AvoidObstacleDragPoint(0, 0);
                }
                else
                {
                    DragPoints[i] = new AvoidObstacleDragPoint(random.Next(-60000, 60000), random.Next(-60000, 60000));
                }
            }
            DebugSort();
        }
    }
}

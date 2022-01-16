using Newtonsoft.Json;
using System;
using System.Windows.Input;
using YouiToolkit.Design;
using YouiToolkit.Design.DirectX;
using YouiToolkit.Logging;
using YouiToolkit.Models;
using Point = System.Windows.Point;
using PointF = System.Drawing.PointF;

namespace YouiToolkit.Ctrls
{
    /// <summary>
    /// 避障配置用户交互层
    /// </summary>
    internal class AvoidObstacleLayer : DirectXLayer
    {
        public event Action<AvoidObstacleDragPointSetBlock> Edited;

        /// <summary>
        /// 当前绘制模式
        /// </summary>
        public AvoidObstacleAreaPaintMode PaintMode { get; set; } = AvoidObstacleAreaPaintMode.DragPoint;

        public AvoidObstacleLayerMouseDriftMode DriftMode { get; internal set; } = AvoidObstacleLayerMouseDriftMode.None;
        public bool Drifting { get; protected set; } = false;

        public AvoidObstacleStock Stock { get; set; } = null;

        public AvoidObstacleAngle Angle = AvoidObstacleAngle._270;

        public AvoidObstacleDragPointSetBlock DragPointSetBlock = new AvoidObstacleDragPointSetBlock();

        /// <summary>
        /// 构造
        /// </summary>
        public AvoidObstacleLayer()
        {
            Stock = new AvoidObstacleStock();
        }

        public override bool OnLostFocus()
        {
            if (Drifting)
            {
                Drifting = false;
                DriftMode = AvoidObstacleLayerMouseDriftMode.None;
            }
            return base.OnLostFocus();
        }

        public override bool OnMouseMove(Point p)
        {
            PointF pl = ToLayerPoint(p);

            if (PaintMode == AvoidObstacleAreaPaintMode.DragPoint)
            {
                if (Drifting)
                {
                    if (DriftMode == AvoidObstacleLayerMouseDriftMode.Translate)
                    {
                        if (DragPointSetBlock.Actived.Selected != null)
                        {
                            DragPointSetBlock.Actived.Selected.SetPos(pl.X, pl.Y);
                            DragPointSetBlock.Actived.Sort();
                            return true;
                        }
                        else
                        {
                            if (DragPointSetBlock.Actived.Count() < AvoidObstacleDragPointSet.DragPointsMax)
                            {
                                DragPointSetBlock.Actived.Selected = new AvoidObstacleDragPoint(pl.X, pl.Y);
                                DragPointSetBlock.Actived.Selected.IsMouseHover = true;
                                DragPointSetBlock.Actived.Append(DragPointSetBlock.Actived.Selected);
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (DragPointSetBlock.Actived.Inside(pl, out AvoidObstacleDragPoint entity))
                    {
                        entity.IsMouseHover = true;
                        Logger.Debug($"[AvoidObstacle|DragPointEntity|IsMouseHover] {entity}");
                    }
                    else
                    {
                        DragPointSetBlock.Actived.ClearFx();
                    }
                }
            }
            return base.OnMouseMove(p);
        }

        public override bool OnMouseEnter(Point p)
        {
            return base.OnMouseEnter(p);
        }

        public override bool OnMouseLeave(Point p)
        {
            if (Drifting)
            {
                Drifting = false;
                DriftMode = AvoidObstacleLayerMouseDriftMode.None;
            }
            return base.OnMouseLeave(p);
        }

        public override bool OnMouseDown(Point p, bool leftPressed, bool middlePressed, bool rightPressed, int clickCount = 1)
        {
            PointF pl = ToLayerPoint(p);

            if (PaintMode == AvoidObstacleAreaPaintMode.DragPoint)
            {
                if (DragPointSetBlock.Actived.Inside(pl, out AvoidObstacleDragPoint entity))
                {
                    if (entity.IsOriginal)
                    {
                        DragPointSetBlock.Actived.Selected = null;
                        DragPointSetBlock.Actived.ClearFx();
                    }
                    else
                    {
                        DragPointSetBlock.Actived.Selected = entity;
                    }

                    Logger.Ignore($"[AvoidObstacle|DragPointEntity|SelectedPolygon] {entity}");

                    if (!Drifting)
                    {
                        if (leftPressed)
                        {
                            DriftMode = AvoidObstacleLayerMouseDriftMode.Translate;
                            Drifting = true;
                        }
                    }
                }
            }
            return base.OnMouseDown(p, leftPressed, middlePressed, rightPressed);
        }

        public override bool OnMouseUp(Point p, bool leftReleased, bool middleReleased, bool rightReleased)
        {
            if (Drifting)
            {
                if (leftReleased || rightReleased || middleReleased)
                {
                    if (DriftMode == AvoidObstacleLayerMouseDriftMode.Translate)
                    {
                        if (PaintMode == AvoidObstacleAreaPaintMode.DragPoint)
                        {
                            OnEdited();
                        }
                    }
                    if (DriftMode != AvoidObstacleLayerMouseDriftMode.None)
                    {
                        Drifting = false;
                        DriftMode = AvoidObstacleLayerMouseDriftMode.None;
                    }
                }
            }
            return base.OnMouseUp(p, leftReleased, middleReleased, rightReleased);
        }

        public override bool OnMouseWheel(Point p, int delta)
        {
            return base.OnMouseWheel(p, delta);
        }

        public void SwitchTo(int index)
        {
            // 从储存层拷贝到渲染层
            Stock.SwitchTo(DragPointSetBlock, index);
        }

        public void SwitchFrom(int? index = null)
        {
            // 从渲染层拷贝到储存层
            Stock.SwitchFrom(DragPointSetBlock, index);
        }

        /// <summary>
        /// <seealso cref="ThumbnailImage.ShortCut"/>
        /// </summary>
        public void ShortCutTo(ThumbnailImageShortCutEventArgs e)
        {
            // 修改操作可直接对渲染层对象操作(※因为他引用自存储层对象)
            switch (e.Type)
            {
                // 复制操作处理
                case ShortCutType.Copy:
                    {
                        try
                        {
                            var text = new AvoidObstacleShortCutText()
                            {
                                Index = e.Index,
                                DragPoints = DragPointSetBlock.ToCtorParamsArrayArray(),
                            };
                            ClipboardUtils.AddData(ClipboardForamt.Text, JsonConvert.SerializeObject(text, Formatting.Indented));
                            ClipboardUtils.End();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString());
                        }
                    }
                    break;
                // 粘贴操作处理
                case ShortCutType.Paste:
                    {
                        try
                        {
                            string clipboardText = ClipboardUtils.GetData(ClipboardForamt.Text) as string;
                            AvoidObstacleShortCutText text = JsonConvert.DeserializeObject<AvoidObstacleShortCutText>(clipboardText);

                            DragPointSetBlock.FromCtorParamsArrayArray(text.DragPoints);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString());
                        }
                    }
                    break;
                case ShortCutType.Cut:
                    // 剪切操作处理
                    // **************************************
                    // 此处无需处理：
                    // 因为触发端已经会调用
                    // ShortCutTo(Copy) & ShortCutTo(Remove)
                    // **************************************
                    break;
                case ShortCutType.Remove:
                    // 清空当前索引数据
                    DragPointSetBlock.Clear();
                    break;
                case ShortCutType.Clear:
                    // 清空存储层所有数据
                    Stock.Clear();
                    break;
                default:
                    break;
            }
        }

        public void OnEdited(int? index = null)
        {
            SwitchFrom(index);

            if (index == null)
            {
                DragPointSetBlock.Index = Stock.CurrentIndex;
            }
            Edited?.Invoke(DragPointSetBlock);
        }

        public override bool OnKeyDown(Key key, ModifierKeys modifiers)
        {
            if (PaintMode == AvoidObstacleAreaPaintMode.DragPoint)
            {
                if (DragPointSetBlock.Actived.Selected != null)
                {
                    if (modifiers == ModifierKeys.Shift && key == Key.Delete)
                    {
                        DragPointSetBlock.Actived.Clear();
                        return true;
                    }
                    else if(key == Key.Delete)
                    {
                        DragPointSetBlock.Actived.Remove(DragPointSetBlock.Actived.Selected);
                        return true;
                    }
                }
            }
            return base.OnKeyDown(key, modifiers);
        }

        public override bool OnKeyUp(Key key, ModifierKeys modifiers)
        {
            return base.OnKeyUp(key, modifiers);
        }
    }

    /// <summary>
    /// 鼠标漂移模式
    /// </summary>
    public enum AvoidObstacleLayerMouseDriftMode
    {
        /// <summary>
        /// 默认
        /// </summary>
        None,

        /// <summary>
        /// 平移
        /// </summary>
        Translate,
    }
}

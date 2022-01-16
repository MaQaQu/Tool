using YouiToolkit.Utils;

namespace YouiToolkit.Models
{
    /// <summary>
    /// 绘制图形方式
    /// </summary>
    internal enum AvoidObstacleAreaPaintMode
    {
        /// <summary>
        /// 独立模式：拖点绘制
        /// </summary>
        DragPoint,

        /// <summary>
        /// 独立模式：扇形绘制
        /// </summary>
        [Comment("未实现")]
        Fan,

        /// <summary>
        /// 独立模式：矩形绘制
        /// </summary>
        [Comment("未实现")]
        Rect,

        /// <summary>
        /// 依靠模式：全部填充绘制
        /// </summary>
        [Comment("未实现")]
        FillAll,

        /// <summary>
        /// 依靠模式：比例填充绘制
        /// </summary>
        [Comment("未实现")]
        FillRatio,
    }
}

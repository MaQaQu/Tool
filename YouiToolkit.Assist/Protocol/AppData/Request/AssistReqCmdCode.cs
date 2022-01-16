namespace YouiToolkit.Assist
{
    /// <summary>
    /// 建图控制命令值
    /// </summary>
    public enum AssistCmdReqMappingCtrl : byte
    {
        /// <summary>
        /// 开始建图
        /// </summary>
        StartMapping = 0x01,

        /// <summary>
        /// 结束并保存建图
        /// </summary>
        StopMapping = 0x02,

        /// <summary>
        /// 取消建图（不保存）
        /// </summary>
        CancelMapping = 0x03,

        /// <summary>
        /// 加载地图
        /// </summary>
        LoadMapping = 0x11,

        /// <summary>
        /// 删除地图
        /// </summary>
        RemoveMapping = 0x12,

        /// <summary>
        /// 开始扩建
        /// </summary>
        StartRemapping = 0x21,

        /// <summary>
        /// 停止并保存扩建
        /// </summary>
        StopRemapping = 0x22,

        /// <summary>
        /// 取消扩建（不保存）
        /// </summary>
        CancelRemapping = 0x23,
    }

    /// <summary>
    /// 重定位控制命令值
    /// </summary>
    public enum AssistCmdReqRelocalizationCtrl : byte
    {
        /// <summary>
        /// 自动重定位
        /// </summary>
        Auto,

        /// <summary>
        /// 指定初始位姿
        /// </summary>
        Manual,
    }
}

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 建图状态雷达状态
    /// </summary>
    public enum AssistCmdRspMappingStatusLidar : byte
    {
        /// <summary>
        /// 异常
        /// </summary>
        Error = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,
    }

    /// <summary>
    /// 建图状态
    /// </summary>
    public enum AssistCmdRspMappingStatus : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0,

        /// <summary>
        /// 待命
        /// </summary>
        Ready = 1,

        /// <summary>
        /// 建图中
        /// </summary>
        Mapping = 2,

        /// <summary>
        /// 保存地图中
        /// </summary>
        Saving = 3,

        /// <summary>
        /// 未加载地图
        /// </summary>
        MapUnloaded = 4,

        /// <summary>
        /// 已加载地图
        /// </summary>
        MapLoaded = 5,

        /// <summary>
        /// 补建中
        /// </summary>
        Remapping = 6,

        /// <summary>
        /// 取消中
        /// </summary>
        Canceling = 7,
    }

    /// <summary>
    /// 建图状态重定位状态
    /// </summary>
    public enum AssistCmdRspMappingStatusRelocalization : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0,

        /// <summary>
        /// 已重定位
        /// </summary>
        Relocalized = 1,

        /// <summary>
        /// 未重定位
        /// </summary>
        Unrelocalized = 2,

        /// <summary>
        /// 异常
        /// </summary>
        Error = 3,
    }

    /// <summary>
    /// 建图状态异常码
    /// </summary>
    public enum AssistCmdRspMappingStatusErrorCode : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0,
    }

    /// <summary>
    /// 建图控制结果
    /// </summary>
    public enum AssistCmdRspMappingCtrlResult : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 0x01,

        /// <summary>
        /// 失败
        /// </summary>
        Error = 0x02,
    }

    /// <summary>
    /// 建图控制错误码
    /// </summary>
    public enum AssistCmdRspMappingCtrlErrorCode : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 状态错误
        /// </summary>
        StatusError = 0x01,

        /// <summary>
        /// 地图名称不存在
        /// </summary>
        MapNameNotExist = 0x02,

        /// <summary>
        /// 未加载地图
        /// </summary>
        MapUnload = 0x03,

        /// <summary>
        /// 无定位数据
        /// </summary>
        NoLocalizationData = 0x04,
    }

    /// <summary>
    /// 重定位控制结果
    /// </summary>
    public enum AssistCmdRspRelocalizationCtrlResult : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 0x01,

        /// <summary>
        /// 失败
        /// </summary>
        Error = 0x02,
    }

    /// <summary>
    /// 重定位控制错误码
    /// </summary>
    public enum AssistCmdRspRelocalizationCtrlErrorCode : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0x00,
    }
}

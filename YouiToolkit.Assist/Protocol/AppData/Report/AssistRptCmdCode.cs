namespace YouiToolkit.Assist
{
    /// <summary>
    /// 报告数据错误
    /// </summary>
    public enum AssistCmdRptErrorCode
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 命令码错误
        /// </summary>
        CmdError = 0x01,

        /// <summary>
        /// 长度错误
        /// </summary>
        LengthError = 0x02,

        /// <summary>
        /// 命令值错误
        /// </summary>
        CmdValueError = 0x03,

        /// <summary>
        /// 权限不足
        /// </summary>
        PermissionDenied = 0x04,

        /// <summary>
        /// 内存不足
        /// </summary>
        OutOfMemory = 0x05,

        /// <summary>
        /// CRC校验失败
        /// </summary>
        CRCVerifyFailed = 0x06,

        /// <summary>
        /// 回环成功
        /// </summary>
        LoopbackSucceeded = 0x07,
    }

    /// <summary>
    /// 报告数据错误值
    /// </summary>
    public enum AssistCmdRptErrorValue : uint
    {
        None = 0,
    }
}

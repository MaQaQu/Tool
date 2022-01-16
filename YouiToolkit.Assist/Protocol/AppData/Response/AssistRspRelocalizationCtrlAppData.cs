namespace YouiToolkit.Assist
{
    /// <summary>
    /// 响应重定位控制结果
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspRelocalizationCtrl)]
    internal class AssistRspRelocalizationCtrlAppData : AssistAppData
    {
        /// <summary>
        /// 结果
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspRelocalizationCtrlResult))]
        public byte Result { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspRelocalizationCtrlErrorCode))]
        public byte ErrorCode { get; set; }
    }
}

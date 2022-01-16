namespace YouiToolkit.Assist
{
    /// <summary>
    /// 建图控制
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspMappingCtrl)]
    internal class AssistRspMappingCtrlAppData : AssistAppData
    {
        /// <summary>
        /// 结果
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspMappingCtrlResult))]
        public byte Result { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspMappingCtrlErrorCode))]
        public byte ErrorCode { get; set; }
    }
}

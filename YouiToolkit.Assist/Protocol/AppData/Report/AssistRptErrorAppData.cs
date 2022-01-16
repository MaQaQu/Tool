namespace YouiToolkit.Assist
{
    /// <summary>
    /// 错误报告数据
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RptError)]
    internal class AssistRptErrorAppData : AssistAppData
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRptErrorCode))]
        public byte ErrorCode { get; set; }

        /// <summary>
        /// 错误值
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRptErrorValue))]
        public int ErrorValue { get; set; }
    }
}

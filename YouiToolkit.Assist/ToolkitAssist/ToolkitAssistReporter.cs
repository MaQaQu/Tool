namespace YouiToolkit.Assist
{
    public class ToolkitAssistReporter : ObservableObjectBase, IToolkitAssistSubCtrl
    {
        /// <summary>
        /// 父控制器
        /// </summary>
        public ToolkitAssist Assist { get; set; } = null;

        /// <summary>
        /// 错误码
        /// </summary>
        private AssistCmdRptErrorCode errorCode = AssistCmdRptErrorCode.None;
        public AssistCmdRptErrorCode ErrorCode
        {
            get => errorCode;
            internal set => Set(ref errorCode, value, nameof(ErrorCode), true);
        }

        /// <summary>
        /// 错误值
        /// </summary>
        private AssistCmdRptErrorValue errorValue = AssistCmdRptErrorValue.None;
        public AssistCmdRptErrorValue ErrorValue
        {
            get => errorValue;
            internal set => Set(ref errorValue, value, nameof(ErrorValue), false);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="assist">父控制器</param>
        public ToolkitAssistReporter(ToolkitAssist assist)
        {
            Assist = assist;
        }
    }
}

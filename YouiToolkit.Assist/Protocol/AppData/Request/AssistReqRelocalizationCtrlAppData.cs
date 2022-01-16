namespace YouiToolkit.Assist
{
    [AssistCmdCode(AssistCmdCode.ReqRelocalizationCtrl)]
    internal class AssistReqRelocalizationCtrlAppData : AssistAppData
    {
        /// <summary>
        /// 重定位控制指令
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdReqRelocalizationCtrl))]
        public byte CmdVal { get; set; }

        /// <summary>
        /// 初始位置X
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// 初始位置Y
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 初始角度A
        /// </summary>
        public float A { get; set; }
    }
}

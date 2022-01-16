namespace YouiToolkit.Assist
{
    [AssistCmdCode(AssistCmdCode.ReqMappingCtrl)]
    internal class AssistReqMappingCtrlAppData : AssistAppData
    {
        [AssistCmdCode(typeof(AssistCmdReqMappingCtrl))]
        public byte CmdVal { get; set; }

        public string MapName { get; set; }
    }
}

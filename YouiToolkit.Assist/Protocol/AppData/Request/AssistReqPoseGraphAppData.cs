namespace YouiToolkit.Assist
{
    /// <summary>
    /// 位姿图请求数据
    /// </summary>
    [AssistCmdCode(AssistCmdCode.ReqPoseGraph)]
    internal class AssistReqPoseGraphAppData : AssistAppData
    {
        public int Index { get; set; }
    }
}

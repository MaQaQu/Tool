namespace YouiToolkit.Assist
{
    [AssistCmdCode(AssistCmdCode.RspMappingName)]
    internal class AssistRspMappingNameAppData : AssistAppData
    {
        /// <summary>
        /// 地图名称长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 地图名称
        /// </summary>
        public byte[] Name { get; set; }
    }
}

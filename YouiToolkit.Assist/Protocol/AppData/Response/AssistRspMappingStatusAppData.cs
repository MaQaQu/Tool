namespace YouiToolkit.Assist
{
    /// <summary>
    /// 建图状态
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspMappingStatus)]
    internal class AssistRspMappingStatusAppData : AssistAppData
    {
        /// <summary>
        /// 雷达数据
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspMappingStatusLidar))]
        public bool Valid { get; set; }

        /// <summary>
        /// 建图状态
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspMappingStatus))]
        public byte MappingStatus { get; set; }

        /// <summary>
        /// 定位状态
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspMappingStatusRelocalization))]
        public byte LocalizationStatus { get; set; }

        /// <summary>
        /// 异常码
        /// </summary>
        [AssistCmdCode(typeof(AssistCmdRspMappingStatusErrorCode))]
        public int ErrorCode { get; set; }

        /// <summary>
        /// 位置X（相对于世界坐标X）
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 位置X（相对于世界坐标Y）
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 角度A
        /// </summary>
        public double A { get; set; }
    }
}

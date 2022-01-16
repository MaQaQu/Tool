namespace YouiToolkit.Assist
{
    /// <summary>
    /// 位姿图响应数据
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspPoseGraph)]
    internal class AssistRspPoseGraphAppData : AssistAppData
    {
        public int Index { get; set; }
        public int Version { get; set; }
        public int TotalPack { get; set; }
        public int CurrPack { get; set; }
        public int Count { get; set; }

        /// <summary>
        /// 点云已生成的地图X（历史数据）
        /// </summary>
        public float[] X { get; set; }

        /// <summary>
        /// 点云已生成的地图Y（历史数据）
        /// </summary>
        public float[] Y { get; set; }
    }
}

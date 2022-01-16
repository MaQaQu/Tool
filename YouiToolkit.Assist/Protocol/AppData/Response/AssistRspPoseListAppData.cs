namespace YouiToolkit.Assist
{
    /// <summary>
    /// 位姿图列表响应数据
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspPoseList)]
    internal class AssistRspPoseListAppData : AssistAppData
    {
        /// <summary>
        /// 列表长度
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 位姿图列表（历史数据）
        /// </summary>
        public AssistPoseNode[] PoseNodes { get; set; }
    }

    /// <summary>
    /// 位姿节点数据
    /// </summary>
    internal struct AssistPoseNode
    {
        /// <summary>
        /// 所在节点索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 数据版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 相对于世界坐标系X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 相对于世界坐标系Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 相对于世界坐标系方向角Θ
        /// </summary>
        public double A { get; set; }
    }
}

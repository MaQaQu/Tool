namespace YouiToolkit.Assist
{
    /// <summary>
    /// 点云响应数据
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspPointCloud)]
    internal class AssistRspPointCloudAppData : AssistAppData
    {
        /// <summary>
        /// 总包数
        /// </summary>
        public int TotalPack { get; set; }

        /// <summary>
        /// 当前包
        /// </summary>
        public int CurrPack { get; set; }

        public int StartIndex { get; set; }
        public int TotalCount { get; set; }
        public int CurrCount { get; set; }

        /// <summary>
        /// 相对于PX
        /// </summary>
        public float[] X { get; set; }

        /// <summary>
        /// 相对于PY
        /// </summary>
        public float[] Y { get; set; }

        /// <summary>
        /// 相对于世界坐标X
        /// </summary>
        public double PX { get; set; }

        /// <summary>
        /// 相对于世界坐标Y
        /// </summary>
        public double PY { get; set; }

        /// <summary>
        /// 角度A
        /// </summary>
        public double PA { get; set; }
    }
}

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 先验点云响应数据
    /// </summary>
    [AssistCmdCode(AssistCmdCode.RspPointCloud)]
    internal class AssistRspPointCloudPriorAppData : AssistAppData
    {
        /// <summary>
        /// 总包数
        /// </summary>
        public int TotalPack { get; set; }

        /// <summary>
        /// 当前包
        /// </summary>
        public int CurrPack { get; set; }

        /// <summary>
        /// 相对于世界坐标X
        /// </summary>
        public float PX { get; set; }

        /// <summary>
        /// 相对于世界坐标Y
        /// </summary>
        public float PY { get; set; }

        /// <summary>
        /// 角度A
        /// </summary>
        public float PA { get; set; }

        /// <summary>
        /// 地图分辨率
        /// </summary>
        public float Resolution { get; set; }

        /// <summary>
        /// PNG数据总长度
        /// </summary>
        public uint PngMaxLength { get; set; }

        /// <summary>
        /// PNG图片数据
        /// </summary>
        public byte[] PngData { get; set; }
    }
}

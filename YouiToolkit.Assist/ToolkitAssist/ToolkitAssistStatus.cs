using SuperMath;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 反馈状态集
    /// </summary>
    public class ToolkitAssistStatus : ObservableObjectBase
    {
        public override bool Async => false;

        /// <summary>
        /// 雷达数据是否正常
        /// </summary>
        public bool Valid { get; internal set; }

        /// <summary>
        /// 位置（RAW）
        /// </summary>
        public GraphPointA Position { get; internal set; }
        public GraphPoint AxisX { get; internal set; }
        public GraphPoint AxisY { get; internal set; }

        /// <summary>
        /// 雷达点云
        /// </summary>
        public AssistPointCloud LidarPointCloud { get; private set; }
        public bool[] PoseGraphValids { get; private set; }
        public AssistPoseGraph[] PoseGraphs { get; private set; }
        public readonly int PoseGraphMaxCount = ushort.MaxValue;

        /// <summary>
        /// 位姿图
        /// </summary>
        public PointGridMap GridMap { get; private set; } = null;
        internal object GridMapMonitorLock = new object();

        /// <summary>
        /// 建图状态
        /// </summary>
        private AssistCmdRspMappingStatus mappingStatus = AssistCmdRspMappingStatus.None;
        public AssistCmdRspMappingStatus MappingStatus
        {
            get => mappingStatus;
            internal set => Set(ref mappingStatus, value, nameof(MappingStatus));
        }

        /// <summary>
        /// 重定位状态
        /// </summary>
        private AssistCmdRspMappingStatusRelocalization relocalizationStatus = AssistCmdRspMappingStatusRelocalization.None;
        public AssistCmdRspMappingStatusRelocalization RelocalizationStatus
        {
            get => relocalizationStatus;
            internal set => Set(ref relocalizationStatus, value, nameof(RelocalizationStatus));
        }

        /// <summary>
        /// 构造
        /// </summary>
        public ToolkitAssistStatus()
        {
            LidarPointCloud = new AssistPointCloud(PoseGraphMaxCount);
            PoseGraphValids = new bool[PoseGraphMaxCount];
            PoseGraphs = new AssistPoseGraph[PoseGraphMaxCount];
            for (int i = 0; i < PoseGraphMaxCount; i++)
            {
                PoseGraphValids[i] = false;
                PoseGraphs[i] = null;
            }
            GridMap = new PointGridMap(-1310720, -1310720, 1310720);
        }

        /// <summary>
        /// 清除缓存区
        /// </summary>
        public void ClearBuffers()
        {
            lock (GridMapMonitorLock)
            {
                GridMap = new PointGridMap(-1310720, -1310720, 1310720);
            }
            for (int i = 0; i < PoseGraphMaxCount; i++)
            {
                PoseGraphValids[i] = false;
                PoseGraphs[i] = null;
            }
            LidarPointCloud.ClearMapPrior();
        }
    }
}

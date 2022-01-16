using System;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 通讯命令码
    /// </summary>
    internal enum AssistCmdCode : byte
    {
        /// <summary>
        /// 无命令
        /// </summary>
        None = 0x00,

        #region [请求命令]
        /// <summary>
        /// 请求心跳
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqHeartbeat = 0x01,

        /// <summary>
        /// 请求姿态列表
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqPoseList = 0x12,

        /// <summary>
        /// 请求姿态图
        /// </summary>
        [AssistCmdCode(typeof(AssistReqPoseGraphAppData), Tag = AssistCmdType.Request)]
        ReqPoseGraph = 0x13,

        /// <summary>
        /// 请求点云图
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqPointCloud = 0x24,

        /// <summary>
        /// 请求先验点云图
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqPointCloudPrior = 0x25,

        /// <summary>
        /// 请求建图状态
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqMappingStatus = 0x31,

        /// <summary>
        /// 请求地图名称
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqMappingName = 0x32,

        /// <summary>
        /// 请求地图列表
        /// </summary>
        [AssistCmdCode(typeof(AssistAppData), Tag = AssistCmdType.Request)]
        ReqMappingList = 0x33,

        /// <summary>
        /// 请求建图控制
        /// </summary>
        [AssistCmdCode(typeof(AssistReqMappingCtrlAppData), Tag = AssistCmdType.Request)]
        [AssistCmdCode(typeof(AssistCmdReqMappingCtrl))]
        ReqMappingCtrl = 0x91,

        /// <summary>
        /// 请求重定位控制
        /// </summary>
        [AssistCmdCode(typeof(AssistReqRelocalizationCtrlAppData), Tag = AssistCmdType.Request)]
        [AssistCmdCode(typeof(AssistCmdReqRelocalizationCtrl))]
        ReqRelocalizationCtrl = 0x92,
        #endregion

        #region [响应命令]
        /// <summary>
        /// 响应心跳
        /// </summary>
        [AssistCmdCode(typeof(Nullable), Tag = AssistCmdType.Response)]
        RspHeartbeat = 0xA1,

        /// <summary>
        /// 响应姿态列表
        /// </summary>
        [AssistCmdCode(typeof(AssistRspPoseListAppData), Tag = AssistCmdType.Response)]
        [AssistCmdCode(typeof(AssistPoseNode), Tag = AssistCmdType.Response)]
        RspPoseList = 0xB2,

        /// <summary>
        /// 响应姿态图
        /// </summary>
        [AssistCmdCode(typeof(AssistRspPoseGraphAppData), Tag = AssistCmdType.Response)]
        RspPoseGraph = 0xB3,

        /// <summary>
        /// 响应点云图
        /// </summary>
        [AssistCmdCode(typeof(AssistRspPointCloudAppData), Tag = AssistCmdType.Response)]
        RspPointCloud = 0xC4,

        /// <summary>
        /// 响应先验点云图
        /// </summary>
        [AssistCmdCode(typeof(AssistRspPointCloudPriorAppData), Tag = AssistCmdType.Response)]
        RspPointCloudPrior = 0xC5,

        /// <summary>
        /// 响应位置
        /// </summary>
        [AssistCmdCode(typeof(Nullable), Tag = AssistCmdType.Response)]
        RspPosition = 0xD1,

        /// <summary>
        /// 响应建图状态
        /// </summary>
        [AssistCmdCode(typeof(AssistRspMappingStatusAppData), Tag = AssistCmdType.Response)]
        [AssistCmdCode(typeof(AssistCmdRspMappingStatusLidar))]
        [AssistCmdCode(typeof(AssistCmdRspMappingStatus))]
        [AssistCmdCode(typeof(AssistCmdRspMappingStatusRelocalization))]
        RspMappingStatus = 0xD1,

        /// <summary>
        /// 响应地图名称
        /// </summary>
        [AssistCmdCode(typeof(AssistRspMappingNameAppData), Tag = AssistCmdType.Response)]
        RspMappingName = 0xD2,

        /// <summary>
        /// 响应地图列表
        /// </summary>
        [AssistCmdCode(typeof(AssistRspMappingListAppData), Tag = AssistCmdType.Response)]
        RspMappingList = 0xD3,

        /// <summary>
        /// 响应建图控制
        /// </summary>
        [AssistCmdCode(typeof(AssistRspMappingCtrlAppData), Tag = AssistCmdType.Response)]
        [AssistCmdCode(typeof(AssistCmdRspMappingCtrlResult))]
        [AssistCmdCode(typeof(AssistCmdRspMappingCtrlErrorCode))]
        RspMappingCtrl = 0xE1,

        /// <summary>
        /// 响应重定位控制
        /// </summary>
        [AssistCmdCode(typeof(AssistRspRelocalizationCtrlAppData), Tag = AssistCmdType.Response)]
        [AssistCmdCode(typeof(AssistCmdRspRelocalizationCtrlResult))]
        [AssistCmdCode(typeof(AssistCmdRspRelocalizationCtrlErrorCode))]
        RspRelocalizationCtrl = 0xE2,
        #endregion

        #region [报告命令]
        /// <summary>
        /// 报告数据错误
        /// </summary>
        [AssistCmdCode(typeof(AssistRptErrorAppData), Tag = AssistCmdType.Report)]
        [AssistCmdCode(typeof(AssistCmdRptErrorCode))]
        [AssistCmdCode(typeof(AssistCmdRptErrorValue))]
        RptError = 0xF0,
        #endregion
    }

    public enum AssistCmdType
    {
        Request,
        Response,
        Report,
    }
}

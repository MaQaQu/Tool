using System;
using YouiToolkit.Utils;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 建图非周期会话接口
    /// </summary>
    public class ToolkitAssistDialog : ToolkitAssistDialogBase
    {
        /// <summary>
        /// 地图名称
        /// </summary>
        private string mapName = string.Empty;
        public string MapName
        {
            get => mapName;
            internal set => Set(ref mapName, value, nameof(MapName), true);
        }

        /// <summary>
        /// 地图列表
        /// </summary>
        private string[] mapList = Array.Empty<string>();
        public string[] MapList
        {
            get => mapList;
            internal set => Set(ref mapList, value, nameof(MapList), true);
        }

        /// <summary>
        /// 建图控制结果
        /// </summary>
        private AssistCmdRspMappingCtrlResult mappingCtrlResult = AssistCmdRspMappingCtrlResult.None;
        public AssistCmdRspMappingCtrlResult MappingCtrlResult
        {
            get => mappingCtrlResult;
            internal set => Set(ref mappingCtrlResult, value, nameof(MappingCtrlResult), true);
        }

        /// <summary>
        /// 建图控制错误码
        /// </summary>
        private AssistCmdRspMappingCtrlErrorCode mappingCtrlErrorCode = AssistCmdRspMappingCtrlErrorCode.None;
        public AssistCmdRspMappingCtrlErrorCode MappingCtrlErrorCode
        {
            get => mappingCtrlErrorCode;
            internal set => Set(ref mappingCtrlErrorCode, value, nameof(MappingCtrlErrorCode));
        }

        /// <summary>
        /// 重定位控制结果
        /// </summary>
        private AssistCmdRspRelocalizationCtrlResult relocalizationCtrlResult = AssistCmdRspRelocalizationCtrlResult.None;
        public AssistCmdRspRelocalizationCtrlResult RelocalizationCtrlResult
        {
            get => relocalizationCtrlResult;
            internal set => Set(ref relocalizationCtrlResult, value, nameof(RelocalizationCtrlResult), true);
        }

        /// <summary>
        /// 重定位控制错误码
        /// </summary>
        private AssistCmdRspRelocalizationCtrlErrorCode relocalizationCtrlErrorCode = AssistCmdRspRelocalizationCtrlErrorCode.None;
        public AssistCmdRspRelocalizationCtrlErrorCode RelocalizationCtrlErrorCode
        {
            get => relocalizationCtrlErrorCode;
            internal set => Set(ref relocalizationCtrlErrorCode, value, nameof(RelocalizationCtrlErrorCode));
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="assist">父控制器</param>
        public ToolkitAssistDialog(ToolkitAssist assist)
        {
            Assist = assist;
        }

        /// <summary>
        /// 发送请求建图控制命令
        /// </summary>
        /// <param name="cmdVal">建图控制命令</param>
        /// <param name="mapName">地图名称</param>
        private void TransReqMappingCtrl(AssistCmdReqMappingCtrl cmdVal, string mapName)
        {
            var app = new AssistReqMappingCtrlAppData()
            {
                Session = Assist.GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqMappingCtrl,
                CmdVal = (byte)cmdVal,
                MapName = mapName,
            };
            Assist.TcpPort.DataTransmit(app);
        }

        /// <summary>
        /// 发送请求地图名称命令
        /// </summary>
        private void TransReqMappingName()
        {
            var app = new AssistAppData()
            {
                Session = Assist.GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqMappingName,
            };
            Assist.TcpPort.DataTransmit(app);
        }

        /// <summary>
        /// 发送请求地图列表命令
        /// </summary>
        private void TransReqMappingList()
        {
            var app = new AssistAppData()
            {
                Session = Assist.GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqMappingList,
            };
            Assist.TcpPort.DataTransmit(app);
        }

        private void TransReqPointCloudPrior(string mapName = null)
        {
            var app = new AssistReqPointCloudPriorAppData()
            {
                Session = Assist.GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqPointCloudPrior,
                MapName = mapName,
            };

            Assist.TcpPort.DataTransmit(app);
        }

        /// <summary>
        /// 发送请求重定位控制命令
        /// </summary>
        /// <param name="cmdVal">建图控制命令</param>
        /// <param name="mapName">地图名称</param>
        private void TransReqRelocalizationCtrl(AssistCmdReqRelocalizationCtrl cmdVal, float x, float y, float a)
        {
            var app = new AssistReqRelocalizationCtrlAppData()
            {
                Session = Assist.GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqRelocalizationCtrl,
                CmdVal = (byte)cmdVal,
                X = x,
                Y = y,
                A = a,
            };
            Assist.TcpPort.DataTransmit(app);
        }

        public bool CallMappingCtrl(AssistCmdReqMappingCtrl cmdVal,
            string mapName,
            out AssistCmdRspMappingCtrlResult result,
            out AssistCmdRspMappingCtrlErrorCode errorCode,
            int? millisecondsTimeout = 4000)
        {
            bool callResult = CallCore(nameof(MappingCtrlResult), () => TransReqMappingCtrl(cmdVal, mapName), millisecondsTimeout);

            if (callResult)
            {
                result = mappingCtrlResult;
                errorCode = mappingCtrlErrorCode;
            }
            else
            {
                result = AssistCmdRspMappingCtrlResult.None;
                errorCode = AssistCmdRspMappingCtrlErrorCode.None;
            }
            return callResult;
        }

        public bool CallRelocalizationCtrl(AssistCmdReqRelocalizationCtrl cmdVal,
            float x, float y, float a,
            out AssistCmdRspRelocalizationCtrlResult result,
            out AssistCmdRspRelocalizationCtrlErrorCode errorCode)
        {
            bool callResult = CallCore(nameof(RelocalizationCtrlResult), () => TransReqRelocalizationCtrl(cmdVal, x, y, a));

            if (callResult)
            {
                result = relocalizationCtrlResult;
                errorCode = relocalizationCtrlErrorCode;
            }
            else
            {
                result = AssistCmdRspRelocalizationCtrlResult.None;
                errorCode = AssistCmdRspRelocalizationCtrlErrorCode.None;
            }
            return callResult;
        }

        public bool CallMappingName(out string mapName)
        {
            bool callResult = CallCore(nameof(MapName), () => TransReqMappingName());

            if (callResult)
            {
                mapName = this.mapName;
            }
            else
            {
                mapName = null;
            }
            return callResult;
        }

        public bool CallMappingList(out string[] mapList)
        {
            bool callResult = CallCore(nameof(MapList), () => TransReqMappingList());

            if (callResult)
            {
                mapList = this.mapList;
            }
            else
            {
                mapList = Array.Empty<string>();
            }
            return callResult;
        }

        private bool isPointCloudPriorRequested = false;
        public bool RequestPointCloudPrior()
        {
            if (!isPointCloudPriorRequested && Assist.Status.LidarPointCloud.IsMapPriorEmpty())
            {
                isPointCloudPriorRequested = true;
                TransReqPointCloudPrior();
                return true;
            }
            return false;
        }

        public void RequestExportPointCloudPrior(string mapName)
        {
            TransReqPointCloudPrior(mapName);
        }

        public bool ClearBuffers()
        {
            isPointCloudPriorRequested = false;
            return true;
        }
    }
}

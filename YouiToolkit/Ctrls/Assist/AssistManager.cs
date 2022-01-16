using System.Threading.Tasks;
using System.Windows;
using YouiToolkit.Assist;
using YouiToolkit.Design;
using YouiToolkit.Logging;

namespace YouiToolkit.Ctrls
{
    internal static class AssistManager
    {
        public static ToolkitAssist Assist { get; private set; } = ToolkitAssist.GetInstance();
        public static bool IsConnected => Assist.IsConnected;
        public static ToolkitAssistDialog Dialog => Assist.Dialog;
        public static ToolkitAssistReporter Reporter => Assist.Reporter;
        public static ToolkitAssistStatus Status => Assist.Status;

        static AssistManager()
        {
            Assist.ConnectStatusChanged += (s, e) =>
            {
                if (IsConnected)
                {
                    Task.Run(() =>
                    {
                        Config.Instance.IPAddr = AssistManager.Assist.IP.ToString();
                        _ = Dialog.CallMappingName(out _);
                        _ = Dialog.CallMappingList(out _);
                        ConfigCtrl.Save();
                    });
                }
            };

            Status.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ToolkitAssistStatus.MappingStatus):
                        {
                            AssistCmdRspMappingStatus statusPrev = (AssistCmdRspMappingStatus?)e.OldValue ?? AssistCmdRspMappingStatus.None;
                            AssistCmdRspMappingStatus statusNext = (AssistCmdRspMappingStatus?)e.NewValue ?? AssistCmdRspMappingStatus.None;

                            Logger.Info($"[MappingStatus] Status changed `{e.TimeStamp.ToString("HH:mm:ss.fff")}` from {statusPrev} to {statusNext}.");

                            if ((statusPrev == AssistCmdRspMappingStatus.Mapping || statusPrev == AssistCmdRspMappingStatus.Saving || statusPrev == AssistCmdRspMappingStatus.MapUnloaded || statusPrev == AssistCmdRspMappingStatus.MapLoaded || statusPrev == AssistCmdRspMappingStatus.Remapping || statusPrev == AssistCmdRspMappingStatus.Canceling)
                            && (statusNext == AssistCmdRspMappingStatus.Ready || statusNext == AssistCmdRspMappingStatus.None))
                            {
                                // 清空建图缓存
                                Status.ClearBuffers();
                                Dialog.ClearBuffers();
                            }

                            if (statusNext == AssistCmdRspMappingStatus.MapUnloaded || statusNext == AssistCmdRspMappingStatus.MapLoaded || statusNext == AssistCmdRspMappingStatus.Remapping)
                            {
                                Dialog.RequestPointCloudPrior();
                            }
                        }
                        break;
                    case nameof(ToolkitAssistStatus.RelocalizationStatus):
                        Logger.Info($"[RelocalizationStatus] Status changed `{e.TimeStamp.ToString("HH:mm:ss.fff")}` from {e.OldValue} to {e.NewValue}.");
                        break;
                }
            };
            
            Reporter.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ToolkitAssistReporter.ErrorCode):
                        Logger.Info($"[Reporter] Error received ErrorCode={AssistManager.Reporter.ErrorCode}({(int)AssistManager.Reporter.ErrorCode}), ErrorValue={AssistManager.Reporter.ErrorValue}({(int)AssistManager.Reporter.ErrorValue})");
                        break;
                }
            };
        }

        public static bool CheckConnetced(FrameworkElement frameworkElement)
        {
            if (!IsConnected)
            {
                MessageBoxX.Warning(frameworkElement, "请连接机器人后重复操作！", "未连接机器人");
                return false;
            }
            return true;
        }
    }
}

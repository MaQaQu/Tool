using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YouiToolkit.Assist;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;
using YouiToolkit.Logging;
using YouiToolkit.Models;
using YouiToolkit.Utils;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMtMapRender.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtMapRender : UserControl
    {
        public MapRenderReloadTarget ReloadTarget { get; set; } = MapRenderReloadTarget.None;

        public void Reload(MapRenderReloadTarget reloadTarget)
        {
            ReloadTarget = reloadTarget;
            ResetOrigin();
        }

        public PageMtMapRender()
        {
            InitializeComponent();

            // 地图渲染控件上下文构造完成事件
            mapRender.ContextChanged += (s, e) =>
            {
                // 地图渲染状态信息变化事件
                mapRender.Context.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(mapRender.Context.MouseLoctMap):
                            runMouseLoctMap.Text = mapRender.Context.ToString(e.PropertyName);
                            break;
                        case nameof(mapRender.Context.OriginRotate):
                            runOriginRotate.Text = mapRender.Context.ToString(e.PropertyName);
                            break;
                        case nameof(mapRender.Context.OriginScale):
                            runOriginScale.Text = mapRender.Context.ToString(e.PropertyName);
                            break;
                    }
                };

                // 手动重定位结果反馈事件
                mapRender.Context.PoseEstimateConfirmed += (s, e) =>
                {
                    Task.Run(() =>
                    {
                        Logger.Info($"[PoseEstimateConfirmed] {e.ToString()}");
                        bool ret = AssistManager.Dialog.CallRelocalizationCtrl(AssistCmdReqRelocalizationCtrl.Manual, e.Start.X, e.Start.Y, e.Theta, out AssistCmdRspRelocalizationCtrlResult result, out _);

                        if (result == AssistCmdRspRelocalizationCtrlResult.Success)
                        {
                            DispatcherHelper.BeginInvoke(() => Toast.Success(gridRender, "重定位成功", ToastLocation.TopCenter));
                        }
                        else
                        {
                            DispatcherHelper.BeginInvoke(() => Toast.Warning(gridRender, "重定位失败", ToastLocation.TopCenter));
                        }
                    });
                };
            };

            // 请求暂停渲染事件
            MessageCenter.Subscribe(MessageToken.PageMapRenderPause, (e) =>
            {
                if (e.Param != null)
                {
                    mapRender.IsPaused = (bool?)e.Param ?? false;
                    Logger.Info($"[MapRender] IsPaused requested {mapRender.IsPaused}.");
                }
            });
            mapRender.IsVisibleChanged += (s, e) =>
            {
                mapRender.IsPaused = !mapRender.IsVisible;
                Logger.Info($"[MapRender] IsPaused requested {mapRender.IsPaused}.");
                HandleMappingStatusChanged(AssistManager.Status.MappingStatus);
            };

            // 更新建图状态&重定位状态
            AssistManager.Status.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ToolkitAssistStatus.MappingStatus):
                        HandleMappingStatusChanged(AssistManager.Status.MappingStatus);
                        break;
                }
            };

            AssistManager.Reporter.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ToolkitAssistReporter.ErrorCode):
                        if (AssistManager.Reporter.ErrorCode == AssistCmdRptErrorCode.LoopbackSucceeded)
                        {
                            DispatcherHelper.BeginInvoke(() => Toast.Success(gridRender, "回环成功"));
                        }
                        break;
                }
            };
        }

        public void HandleMappingStatusChanged(AssistCmdRspMappingStatus status)
        {
            DispatcherHelper.BeginInvoke(() =>
            {
                switch (status)
                {
                    case AssistCmdRspMappingStatus.None:
                    case AssistCmdRspMappingStatus.Ready:
                        break;
                    case AssistCmdRspMappingStatus.Mapping:
                        break;
                    case AssistCmdRspMappingStatus.Saving:
                    case AssistCmdRspMappingStatus.Canceling:
                        break;
                    case AssistCmdRspMappingStatus.MapUnloaded:
                        break;
                    case AssistCmdRspMappingStatus.MapLoaded:
                        break;
                    case AssistCmdRspMappingStatus.Remapping:
                        break;
                }
            });
        }

        public void HandleRelocalizationStatusChanged(AssistCmdRspMappingStatusRelocalization status)
        {
            DispatcherHelper.BeginInvoke(() =>
            {
                switch (status)
                {
                    case AssistCmdRspMappingStatusRelocalization.None:
                        break;
                    case AssistCmdRspMappingStatusRelocalization.Relocalized:
                        break;
                    case AssistCmdRspMappingStatusRelocalization.Unrelocalized:
                        break;
                    case AssistCmdRspMappingStatusRelocalization.Error:
                        break;
                }
            });
        }

        public void ResetOrigin() => mapRender.ResetOrigin();
    }
}

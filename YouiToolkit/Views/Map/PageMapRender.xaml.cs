using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using YouiToolkit.Assist;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;
using YouiToolkit.Logging;
using YouiToolkit.Models;
using YouiToolkit.Utils;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    public partial class PageMapRender : UserControl
    {
        public MapRenderReloadTarget ReloadTarget { get; set; } = MapRenderReloadTarget.None;

        public void Reload(MapRenderReloadTarget reloadTarget)
        {
            ReloadTarget = reloadTarget;
            ResetOrigin();
        }

        public PageMapRender()
        {
            InitializeComponent();

            // 开始补建
            buttonStart.Click += (s, e) =>
            {
                if (!AssistManager.CheckConnetced(this)) return;

                // 可以开始补建的条件：已重定位 + 已加载地图
                if (AssistManager.Status.RelocalizationStatus != AssistCmdRspMappingStatusRelocalization.Relocalized)
                {
                    MessageBoxX.Warning(this, "需要在重定位成功后才能开始建图！", "提示");
                    return;
                }

                Task.Run(() =>
                {
                    AssistManager.Dialog.CallMappingCtrl(AssistCmdReqMappingCtrl.StartRemapping, string.Empty, out AssistCmdRspMappingCtrlResult result, out _);

                    if (result == AssistCmdRspMappingCtrlResult.Success)
                    {
                        Logger.Info("[MappingCtrl] Start mapping success.");
                    }
                    else
                    {
                        DispatcherHelper.BeginInvoke(() => Toast.Error(gridRender, "开始失败", ToastLocation.TopCenter));
                    }
                });
            };

            // 取消地图
            buttonCancel.Click += (s, e) =>
            {
                if (!AssistManager.CheckConnetced(this)) return;
                if (MessageBoxX.Question(this, "确定要取消本次地图录制吗？", "取消录制") == MessageBoxResult.Yes)
                {
                    Task.Run(() =>
                    {
                        AssistManager.Dialog.CallMappingCtrl(AssistCmdReqMappingCtrl.CancelMapping, string.Empty, out AssistCmdRspMappingCtrlResult result, out _, 10000);

                        if (result == AssistCmdRspMappingCtrlResult.Success)
                        {
                            IPendingHandler handler = DispatcherHelper.Invoke(() => PendingBox.Show(this, "正在取消地图", null, false, new PendingBoxConfig()
                            {
                                FontSize = 20,
                                CancelButton = "后台取消",
                                CloseOnCanceled = true,
                            }));

                            if (SpinWaiter.SpinUntil(() => AssistManager.Status.MappingStatus == AssistCmdRspMappingStatus.None || AssistManager.Status.MappingStatus == AssistCmdRspMappingStatus.Ready
                                                        || !AssistManager.IsConnected, 3000))
                            {
                                DispatcherHelper.Invoke(() =>
                                {
                                    handler.Close();

                                    if (AssistManager.IsConnected)
                                    {
                                        Toast.Success(this, "取消成功");
                                        MessageCenter.Publish(MessageToken.PageMapListOpList, string.Empty);
                                    }
                                });
                            }
                            else
                            {
                                handler.Cancelable = true;
                                if (SpinWaiter.SpinUntil(() => AssistManager.Status.MappingStatus == AssistCmdRspMappingStatus.None || AssistManager.Status.MappingStatus == AssistCmdRspMappingStatus.Ready
                                                            || handler.Canceled
                                                            || !AssistManager.IsConnected))
                                {
                                    DispatcherHelper.Invoke(() =>
                                    {
                                        handler.Close();
                                        if (!handler.Canceled && AssistManager.IsConnected)
                                        {
                                            Toast.Success(this, "取消成功");
                                        }
                                        MessageCenter.Publish(MessageToken.PageMapListOpList, string.Empty);
                                    });
                                }
                            }
                        }
                        else
                        {
                            DispatcherHelper.BeginInvoke(() => MessageBoxX.Error(this, "取消录制失败，请稍后再试！"));
                        }
                    });
                }
            };
             
            // 保存地图
            buttonSave.Click += (s, e) =>
            {
                if (!AssistManager.CheckConnetced(this)) return;
                Task.Run(() =>
                {
                    AssistCmdRspMappingCtrlResult result = default;

                    if (ReloadTarget == MapRenderReloadTarget.MapCapture)
                    {
                        AssistManager.Dialog.CallMappingCtrl(AssistCmdReqMappingCtrl.StopMapping, string.Empty, out result, out _);
                    }
                    else if (ReloadTarget == MapRenderReloadTarget.MapEdit)
                    {
                        AssistManager.Dialog.CallMappingCtrl(AssistCmdReqMappingCtrl.StopRemapping, string.Empty, out result, out _);
                    }

                    if (result == AssistCmdRspMappingCtrlResult.Success)
                    {
                        IPendingHandler handler = DispatcherHelper.Invoke(() => PendingBox.Show(this, "正在保存地图", null, false, new PendingBoxConfig()
                        {
                            FontSize = 20,
                            CancelButton = "后台保存",
                            CloseOnCanceled = true,
                        }));

                        Task.Run(() =>
                        {
                            if (SpinWaiter.SpinUntil(() => AssistManager.Status.MappingStatus != AssistCmdRspMappingStatus.Saving
                                                        || !AssistManager.IsConnected, 3000))
                            {
                                DispatcherHelper.Invoke(() =>
                                {
                                    handler.Close();

                                    if (AssistManager.IsConnected)
                                    {
                                        Toast.Success(this, "保存成功");
                                    }
                                    MessageCenter.Publish(MessageToken.PageMapListOpList, string.Empty);
                                });
                            }
                            else
                            {
                                handler.Cancelable = true;
                                if (SpinWaiter.SpinUntil(() => AssistManager.Status.MappingStatus != AssistCmdRspMappingStatus.Saving
                                                            || handler.Canceled
                                                            || !AssistManager.IsConnected))
                                {
                                    DispatcherHelper.Invoke(() =>
                                    {
                                        handler.Close();
                                        if (!handler.Canceled && AssistManager.IsConnected)
                                        {
                                            Toast.Success(this, "保存成功");
                                        }
                                        MessageCenter.Publish(MessageToken.PageMapListOpList, string.Empty);
                                    });
                                }
                            }
                        });
                    }
                    else
                    {
                        DispatcherHelper.BeginInvoke(() => MessageBoxX.Error(this, "保存录制失败，请稍后再试！"));
                    }
                });
            };

            // 自动重定位
            menuItemRelocalizationAuto.Click += (s, e) =>
            {
                if (!AssistManager.CheckConnetced(this)) return;
            };

            // 手动重定位
            menuItemRelocalizationManual.Click += (s, e) =>
            {
                if (!AssistManager.CheckConnetced(this)) return;
                mapRender.Context.PoseEstimateRequested = true;
            };

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

            // 返回到列表界面
            buttonBack.Click += (s, e) =>
            {
                MessageCenter.Publish(MessageToken.PageMapListOpList, string.Empty);
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

            // 注册修改按钮右键菜单为左键菜单
            LeftContextMenuHelper.Register(buttonRelocalization, new Point(-10, -6));

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
                        buttonStart.IsEnabled = false;
                        buttonStart.Visibility = Visibility.Collapsed;
                        buttonRelocalization.IsEnabled = false;
                        buttonRelocalization.Visibility = Visibility.Collapsed;
                        buttonSave.IsEnabled = false;
                        buttonSave.Visibility = Visibility.Collapsed;
                        buttonCancel.IsEnabled = false;
                        buttonCancel.Visibility = Visibility.Collapsed;
                        buttonBack.IsEnabled = true;
                        buttonBack.Visibility = Visibility.Visible;
                        break;
                    case AssistCmdRspMappingStatus.Mapping:
                        buttonStart.IsEnabled = false;
                        buttonStart.Visibility = Visibility.Collapsed;
                        buttonRelocalization.IsEnabled = true;
                        buttonRelocalization.Visibility = Visibility.Visible;
                        buttonSave.IsEnabled = true;
                        buttonSave.Visibility = Visibility.Visible;
                        buttonCancel.IsEnabled = true;
                        buttonCancel.Visibility = Visibility.Visible;
                        buttonBack.IsEnabled = false;
                        buttonBack.Visibility = Visibility.Collapsed;
                        break;
                    case AssistCmdRspMappingStatus.Saving:
                    case AssistCmdRspMappingStatus.Canceling:
                        buttonStart.IsEnabled = false;
                        buttonStart.Visibility = Visibility.Collapsed;
                        buttonRelocalization.IsEnabled = false;
                        buttonRelocalization.Visibility = Visibility.Visible;
                        buttonSave.IsEnabled = false;
                        buttonSave.Visibility = Visibility.Visible;
                        buttonCancel.IsEnabled = false;
                        buttonCancel.Visibility = Visibility.Visible;
                        buttonBack.IsEnabled = false;
                        buttonBack.Visibility = Visibility.Collapsed;
                        break;
                    case AssistCmdRspMappingStatus.MapUnloaded:
                        buttonStart.IsEnabled = false;
                        buttonStart.Visibility = Visibility.Visible;
                        buttonRelocalization.IsEnabled = true;
                        buttonRelocalization.Visibility = Visibility.Visible;
                        buttonSave.IsEnabled = false;
                        buttonSave.Visibility = Visibility.Visible;
                        buttonCancel.IsEnabled = false;
                        buttonCancel.Visibility = Visibility.Visible;
                        buttonBack.IsEnabled = false;
                        buttonBack.Visibility = Visibility.Collapsed;
                        break;
                    case AssistCmdRspMappingStatus.MapLoaded:
                        buttonStart.IsEnabled = true;
                        buttonStart.Visibility = Visibility.Visible;
                        buttonRelocalization.IsEnabled = true;
                        buttonRelocalization.Visibility = Visibility.Visible;
                        buttonSave.IsEnabled = true;
                        buttonSave.Visibility = Visibility.Visible;
                        buttonCancel.IsEnabled = true;
                        buttonCancel.Visibility = Visibility.Visible;
                        buttonBack.IsEnabled = false;
                        buttonBack.Visibility = Visibility.Collapsed;
                        break;
                    case AssistCmdRspMappingStatus.Remapping:
                        buttonStart.IsEnabled = false;
                        buttonStart.Visibility = Visibility.Visible;
                        buttonRelocalization.IsEnabled = false;
                        buttonRelocalization.Visibility = Visibility.Visible;
                        buttonSave.IsEnabled = true;
                        buttonSave.Visibility = Visibility.Visible;
                        buttonCancel.IsEnabled = true;
                        buttonCancel.Visibility = Visibility.Visible;
                        buttonBack.IsEnabled = false;
                        buttonBack.Visibility = Visibility.Collapsed;
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

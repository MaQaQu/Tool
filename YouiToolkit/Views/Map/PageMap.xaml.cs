using System.Threading.Tasks;
using System.Windows.Controls;
using YouiToolkit.Assist;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;
using YouiToolkit.Models;
using YouiToolkit.Utils;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    public partial class PageMap : UserControl
    {
        private MapReloadTarget Target { get; set; } = MapReloadTarget.None;
        internal PageMapRender PageMapRender { get; private set; }
        internal PageMapList PageMapList { get; private set; }

        public PageMap()
        {
            InitializeComponent();

            PageMapRender = new PageMapRender();
            PageMapList = new PageMapList();

            MessageCenter.Subscribe(MessageToken.PageMapListOpCapture, (e) =>
            {
                Reload(MapReloadTarget.MapCapture);
            });
            MessageCenter.Subscribe(MessageToken.PageMapListOpEdit, (e) =>
            {
                Task.Run(() =>
                {
                    AssistManager.Dialog.CallMappingCtrl(AssistCmdReqMappingCtrl.LoadMapping, e.Message as string, out AssistCmdRspMappingCtrlResult result, out _);
                });
                Reload(MapReloadTarget.MapEdit);
            });
            MessageCenter.Subscribe(MessageToken.PageMapListOpList, (e) =>
            {
                Reload(MapReloadTarget.MapList);
            });

            AssistManager.Status.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ToolkitAssistStatus.MappingStatus))
                {
                    DispatcherHelper.BeginInvoke(Reload);
                }
            };

            IsVisibleChanged += (s, e) =>
            {
                Reload();
            };

            Reload();
        }

        private void Reload(MapReloadTarget target)
        {
            if (Target == target)
            {
                return;
            }
            switch (target)
            {
                case MapReloadTarget.MapList:
                    gridRoot.ShowSubPage(PageMapList);
                    PageMapList.Reload();
                    break;
                case MapReloadTarget.MapCapture:
                    gridRoot.ShowSubPage(PageMapRender);
                    PageMapRender.Reload(MapRenderReloadTarget.MapCapture);
                    break;
                case MapReloadTarget.MapEdit:
                    gridRoot.ShowSubPage(PageMapRender);
                    PageMapRender.Reload(MapRenderReloadTarget.MapEdit);
                    break;
            }
            Target = target;
        }

        public void Reload()
        {
#if DEBUG_MAP_CAPTURE
            Reload(MapRenderReloadTarget.MapCapture);
#elif DEBUG_MAP_EDIT
            Reload(MapRenderReloadTarget.MapEdit);
#else
            if (AssistManager.IsConnected)
            {
                switch (AssistManager.Status.MappingStatus)
                {
                    case AssistCmdRspMappingStatus.None:
                    case AssistCmdRspMappingStatus.Ready:
                    default:
                        Reload(MapReloadTarget.MapList);
                        break;
                    case AssistCmdRspMappingStatus.Mapping:
                    case AssistCmdRspMappingStatus.Saving:
                    case AssistCmdRspMappingStatus.Canceling:
                        Reload(MapReloadTarget.MapCapture);
                        break;
                    case AssistCmdRspMappingStatus.MapLoaded:
                    case AssistCmdRspMappingStatus.MapUnloaded:
                    case AssistCmdRspMappingStatus.Remapping:
                        Reload(MapReloadTarget.MapEdit);
                        break;
                }
            }
            else
            {
                Reload(MapReloadTarget.MapList);
            }
#endif
        }
    }
}

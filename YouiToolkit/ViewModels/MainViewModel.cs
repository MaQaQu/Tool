using GalaSoft.MvvmLight;
using System.Windows.Media;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;

namespace YouiToolkit.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 构造
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
            }
            else
            {
            }
            AssistManager.Assist.ConnectStatusChanged += (s, e) => ConnectStatus = e ? EConnectStatus.Connected : EConnectStatus.Unconnected;
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        protected private EConnectStatus connectStatus = default;
        public EConnectStatus ConnectStatus
        {
            get => connectStatus;
            set
            {
                Set(ref connectStatus, value);
                RaisePropertyChanged(nameof(ConnectStatusBackground));
                RaisePropertyChanged(nameof(ConnectStatusText));
            }
        }

        public Brush ConnectStatusBackground => connectStatus switch
        {
            EConnectStatus.Unconnected => BrushUtils.GetBrush("Gray"),
            _ => BrushUtils.GetBrush("#00ae9d"),
        };

        public string ConnectStatusText => connectStatus switch
        {
            EConnectStatus.Unconnected => "未连接",
            _ => "已连接",
        };

        public override string ToString() => $"{ConnectStatus.ToString()}";
    }

    public enum EConnectStatus : byte
    {
        Unconnected,
        Connected,
    }
}

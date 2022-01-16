using GalaSoft.MvvmLight;

namespace YouiToolkit.ViewModels
{
    public class PageRobotViewModel : ViewModelBase
    {
        public string IPAddr
        {
            get => Config.Instance.IPAddr;
            set
            {
                Config.Instance.IPAddr = value;
                RaisePropertyChanged(nameof(IPAddr));
            }
        }
    }
}

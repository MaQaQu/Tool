using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using YouiToolkit.Models;

namespace YouiToolkit.ViewModels
{
    public class PageMapListViewModel : ViewModelBase
    {
        public ObservableCollection<MapListModel> DataList { get; set; } = new ObservableCollection<MapListModel>();
    }
}

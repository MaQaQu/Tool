using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Shapes;
using YouiToolkit.Design;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMaintain.xaml 的交互逻辑
    /// </summary>
    public partial class PageMaintain : UserControl
    {
        internal PageMtMapRender PageMapRender { get; private set; }
        internal PageMtChartRender pageMtChartRender { get; set; }
        public PageMaintain()
        {
            InitializeComponent();
            PageMapRender = new PageMtMapRender();
            pageMtChartRender = new PageMtChartRender();
            Reload();
        }
        private void Reload()
        {
            gridMap.ShowSubPage(PageMapRender);
            gridChart.ShowSubPage(pageMtChartRender);
            PageMapRender.Reload(MapRenderReloadTarget.MapCapture);
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModelLocator.PageMaintain.maintainModel.videoPlayingFlag)
            {
                case false:
                    tbPlay.Style = (Style)FindResource("iconStop");
                    ViewModelLocator.PageMaintain.maintainModel.videoPlayingFlag = true;
                    break;
                case true:
                    tbPlay.Style = (Style)FindResource("iconStart");
                    ViewModelLocator.PageMaintain.maintainModel.videoPlayingFlag = false;
                    break;
            }
        }
    }
}

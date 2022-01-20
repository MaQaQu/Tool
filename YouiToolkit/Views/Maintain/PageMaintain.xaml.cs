using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
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
        public PageMaintain()
        {
            InitializeComponent();
            PageMapRender = new PageMtMapRender();
            Reload();
            MoniSpeedChart();
        }
        private void Reload()
        {
            gridMap.ShowSubPage(PageMapRender);
            PageMapRender.Reload(MapRenderReloadTarget.MapCapture);
        }

        private void MoniSpeedChart()
        {
            ChartValues<double> Values = new ChartValues<double> { 1, 5, 2, 3, 4, 6, 1 };
            SeriesCollection series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Wheel speed",
                    Values = Values
                }
            };
            s1.Series = series;
        }
    }
}

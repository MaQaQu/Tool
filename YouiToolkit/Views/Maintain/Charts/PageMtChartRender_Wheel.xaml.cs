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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YouiToolkit.ViewModels.Maintain.Charts;

namespace YouiToolkit.Views.Maintain.Charts
{
    /// <summary>
    /// PageMtChartRender_Wheel.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtChartRender_Wheel : UserControl
    {
        private PageMtChartRenderViewModel_Wheel _viewModel;
        public PageMtChartRender_Wheel()
        {
            InitializeComponent();
            MoniSpeedChart();
        }
        private void MoniSpeedChart()
        {
            _viewModel = new PageMtChartRenderViewModel_Wheel();
            //画直线
            Chart_Wheel.DataContext = _viewModel;
        }
    }
}

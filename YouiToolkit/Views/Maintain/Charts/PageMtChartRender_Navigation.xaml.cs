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
    /// PageMtChartRender_Navigation.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtChartRender_Navigation : UserControl
    {
        private PageMtChartRenderViewModel_Navigation _viewModel;
        public PageMtChartRender_Navigation()
        {
            InitializeComponent();
            MoniSpeedChart();
        }
        private void MoniSpeedChart()
        {
            _viewModel = new PageMtChartRenderViewModel_Navigation();
            //画直线
            Chart_Navigation.DataContext = _viewModel;
        }
    }
}

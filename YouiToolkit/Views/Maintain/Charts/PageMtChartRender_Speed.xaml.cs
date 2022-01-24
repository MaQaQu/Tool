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
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views.Maintain.Charts
{
    /// <summary>
    /// PageMtChartRender_Speed.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtChartRender_Speed : UserControl
    {
        private PageMtChartRenderViewModel_Speed _viewModel;
        public PageMtChartRender_Speed()
        {
            InitializeComponent();
            MoniSpeedChart();
        }
        private void MoniSpeedChart()
        {
            _viewModel = new PageMtChartRenderViewModel_Speed();
            //画直线
            Chart_Speed.DataContext = _viewModel;
        }
    }
}

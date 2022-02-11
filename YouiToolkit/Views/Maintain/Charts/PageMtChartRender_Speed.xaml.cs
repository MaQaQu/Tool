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
using System.Windows.Threading;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views.Maintain.Charts
{
    /// <summary>
    /// PageMtChartRender_Speed.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtChartRender_Speed : UserControl
    {
        private PageMtChartRenderViewModel_Speed _viewModel;
        private Models.PageMtMapRenderModel mapModel = Models.PageMtMapRenderModel.CreateInstance();
        DispatcherTimer timer;
        public PageMtChartRender_Speed()
        {
            InitializeComponent();
            MoniSpeedChart();
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);//设置的间隔为100ms
                timer.Tick += timer_Tick;
                timer.IsEnabled = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (mapModel.ShowType == (int)Models.MtNavDataShowType.RealTime)
                {
                    _viewModel.UpdateSpeedData_Realtime();
                }
                else
                {
                    _viewModel.UpdateSpeedData_Playback();
                }
            }
            catch { }
        }

        private void MoniSpeedChart()
        {
            _viewModel = new PageMtChartRenderViewModel_Speed();
            //画直线
            Chart_Speed.DataContext = _viewModel;
        }
    }
}

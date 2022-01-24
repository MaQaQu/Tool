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
using YouiToolkit.Design;
using YouiToolkit.Views.Maintain.Charts;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMtChartRender.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtChartRender : UserControl
    {
        internal PageMtChartRender_Angle pageMtChartRender_Angle { get; set; }
        internal PageMtChartRender_Navigation pageMtChartRender_Navigation { get; set; }
        internal PageMtChartRender_Speed pageMtChartRender_Speed { get; set; }
        internal PageMtChartRender_Wheel pageMtChartRender_Wheel { get; set; }
        internal PageMtChartRender_Wlan pageMtChartRender_Wlan { get; set; }
        public PageMtChartRender()
        {
            InitializeComponent();
            pageMtChartRender_Angle = new PageMtChartRender_Angle();
            pageMtChartRender_Navigation = new PageMtChartRender_Navigation();
            pageMtChartRender_Speed = new PageMtChartRender_Speed();
            pageMtChartRender_Wheel = new PageMtChartRender_Wheel();
            pageMtChartRender_Wlan = new PageMtChartRender_Wlan();
            Reload();
        }
        private void Reload()
        {
            Chart_Angle.ShowSubPage(pageMtChartRender_Angle);
            Chart_Navigation.ShowSubPage(pageMtChartRender_Navigation);
            Chart_Speed.ShowSubPage(pageMtChartRender_Speed);
            Chart_Wheel.ShowSubPage(pageMtChartRender_Wheel);
            Chart_Wlan.ShowSubPage(pageMtChartRender_Wlan);
        }

    }
}

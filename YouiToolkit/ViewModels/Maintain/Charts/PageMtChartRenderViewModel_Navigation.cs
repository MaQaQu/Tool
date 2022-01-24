using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.ViewModels
{
    public class PageMtChartRenderViewModel_Navigation
    {
        /// <summary>
        /// 画直线
        /// </summary>
        public PlotModel PlotNavigation{ get; set; }
        public PageMtChartRenderViewModel_Navigation()
        {
            PlotNavigation = new PlotModel();
            //线条
            //var lineSerial = new LineSeries() { Title = "直线实例" };
            //lineSerial.Points.Add(new DataPoint(0, 0));
            //lineSerial.Points.Add(new DataPoint(10, 10));
            //PlotSpeed.Series.Add(lineSerial);

            //函数sin(x)
            var funcSerial = new FunctionSeries((x) => { return Math.Sin(x); }, 0, 10, 0.1, "y=sin(x)");
            funcSerial.Title = "导航";
            PlotNavigation.Series.Add(funcSerial);


            //定义y轴
            LinearAxis leftAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "范围值",//显示标题内容
                TitlePosition = 1,//显示标题位置
                TitleColor = OxyColor.Parse("#d3d3d3"),//显示标题位置
                IsZoomEnabled = true,//坐标轴缩放关闭
                IsPanEnabled = true,//图表缩放功能关闭
                //MajorGridlineStyle = LineStyle.Solid,//主刻度设置格网
                //MajorGridlineColor = OxyColor.Parse("#7379a0"),
                //MinorGridlineStyle = LineStyle.Dot,//子刻度设置格网样式
                //MinorGridlineColor = OxyColor.Parse("#666b8d")
            };
            //定义x轴
            LinearAxis bottomAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 10,
                Title = "时间",//显示标题内容
                TitlePosition = 1,//显示标题位置
                TitleColor = OxyColor.Parse("#d3d3d3"),//显示标题位置
                IsZoomEnabled = true,//坐标轴缩放关闭
                IsPanEnabled = true,//图表缩放功能关闭
                //MajorGridlineStyle = LineStyle.Solid,//主刻度设置格网
                //MajorGridlineColor = OxyColor.Parse("#7379a0"),
                //MinorGridlineStyle = LineStyle.Dot,//子刻度设置格网样式
                //MinorGridlineColor = OxyColor.Parse("#666b8d")
            };

            PlotNavigation.Axes.Add(leftAxis);
            PlotNavigation.Axes.Add(bottomAxis);
            PlotNavigation.Title = "导航";
        }
    }
}

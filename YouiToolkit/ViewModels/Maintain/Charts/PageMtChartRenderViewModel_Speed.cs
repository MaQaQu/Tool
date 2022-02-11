using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YouiToolkit.ViewModels
{
    public class PageMtChartRenderViewModel_Speed : ViewModelBase
    {
        Models.PageMaintainModel mo = Models.PageMaintainModel.CreateInstance();
        Models.PageMtMapRenderModel mapModel = Models.PageMtMapRenderModel.CreateInstance();
        List<DataPoint> dataPoints_Real = new List<DataPoint>();
        List<DataPoint> dataPoints_PlayBack = new List<DataPoint>();
        LineSeries lineSeries = new LineSeries();
        /// <summary>
        /// 画直线
        /// </summary>
        public PlotModel PlotSpeed { get; set; }
        public PageMtChartRenderViewModel_Speed()
        {
            PlotSpeed = new PlotModel();
            //线条
            lineSeries = new LineSeries() { Title = "速度折线图" };
            lineSeries.ItemsSource = dataPoints_Real;
            PlotSpeed.Series.Add(lineSeries);
            //定义y轴
            LinearAxis leftAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 7,
                Title = "速度值",//显示标题内容
                TitlePosition = 1,//显示标题位置
                TitleColor = OxyColor.Parse("#d3d3d3"),//显示标题位置
                IsZoomEnabled = true,//坐标轴缩放关闭
                IsPanEnabled = true,//图表缩放功能关闭
            };
            //定义y轴
            LinearAxis bottomAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = -6,
                Maximum = 0,
                Title = "时间",//显示标题内容
                TitlePosition = 1,//显示标题位置
                TitleColor = OxyColor.Parse("#d3d3d3"),//显示标题位置
                IsZoomEnabled = true,//坐标轴缩放关闭
                IsPanEnabled = true,//图表缩放功能关闭
            };
            //定义x轴
            //DateTimeAxis bottomAxis = new DateTimeAxis()
            //{
            //    Position = AxisPosition.Bottom,
            //    Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-6).Second),
            //    Maximum = DateTimeAxis.ToDouble(DateTime.Now.Second),
            //    StringFormat = "ss",
            //    Title = "时间",//显示标题内容
            //    TitlePosition = 1,//显示标题位置
            //    TitleColor = OxyColor.Parse("#d3d3d3"),//显示标题位置
            //    IsZoomEnabled = true,//坐标轴缩放关闭
            //    IsPanEnabled = true,//图表缩放功能关闭
            //};
            PlotSpeed.Axes.Add(leftAxis);
            PlotSpeed.Axes.Add(bottomAxis);
            PlotSpeed.Title = "速度";
        }
        public void UpdateSpeedData_Realtime()
        {
            //实时 显示最近的6s数据 X轴单位s 范围[-6,0]
            var lineSerial = new LineSeries() { Title = "车体速度" };
            if (dataPoints_Real.Count == 0)
            {
                dataPoints_Real.Add(new DataPoint(0, 0));
            }
            else if (dataPoints_Real.Count < 7)
            {
                dataPoints_Real.Add(new DataPoint(-dataPoints_Real.Count, dataPoints_Real.Count));
            }
            else
            {
                dataPoints_Real.RemoveAt(dataPoints_Real.Count - 1);
                for (int i = 0; i < dataPoints_Real.Count; i++)
                {
                    dataPoints_Real[i] = new DataPoint(dataPoints_Real[i].X - 1, dataPoints_Real[i].Y);
                }
                dataPoints_Real.Insert(0, new DataPoint(0, new Random().Next(1, 7)));
            }
            lineSeries.ItemsSource = dataPoints_Real;
            PlotSpeed.InvalidatePlot(true);
        }
        public void UpdateSpeedData_Playback()
        {
            dataPoints_Real.Clear();
            //获取开始时间到当前时间段内对应的速度数据

            //添加速度值点

            //刷新折线图数据源
            lineSeries.ItemsSource = dataPoints_PlayBack;
            //刷新折线图显示
            PlotSpeed.InvalidatePlot(true);
        }
    }
}

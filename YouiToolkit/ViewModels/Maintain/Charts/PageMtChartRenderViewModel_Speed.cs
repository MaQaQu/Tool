using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SuperPort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace YouiToolkit.ViewModels
{
    public class PageMtChartRenderViewModel_Speed : ViewModelBase
    {
        CommMariaDB commMariaDB = new CommMariaDB();
        Models.PageMaintainModel mo = Models.PageMaintainModel.CreateInstance();
        Models.PageMtMapRenderModel mapModel = Models.PageMtMapRenderModel.CreateInstance();
        Models.PageMtChartRenderModel_Speed Model_Speed = Models.PageMtChartRenderModel_Speed.CreateInstance();
        List<DataPoint> dataPoints_Real = new List<DataPoint>();
        List<DataPoint> dataPoints_PlayBack = new List<DataPoint>();
        LineSeries lineSeries = new LineSeries();
        private bool changeShowTypeToReal = true;
        private bool changeShowTypeToPlayBack = true;
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
            if (changeShowTypeToReal)
            {
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
                PlotSpeed.Axes.Clear();
                PlotSpeed.Axes.Add(leftAxis);
                PlotSpeed.Axes.Add(bottomAxis);
                changeShowTypeToReal = false;
                changeShowTypeToPlayBack = true;
            }
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
            if (changeShowTypeToPlayBack)
            {
                dataPoints_Real.Clear();
                changeShowTypeToPlayBack = false;
                changeShowTypeToReal = true;
            }
            DateTime dt = mapModel.PlayTime;
            if (Model_Speed.DateTime_Current != dt)
            {
                Model_Speed.DateTime_Current = dt;
                List<string> speedData = commMariaDB.GetSqlFileByTime_Speed(dt.ToString("yyyyMMddHHmmss"));
                if (speedData.Count > 0)
                {
                    UpdateSpeedData(speedData);
                    //获取对应时间速度值
                    GetSpeedDataByTime(dt);
                    //添加速度值点
                    SetDataPoints_PlayBack();
                    //刷新折线图数据源
                    lineSeries.ItemsSource = dataPoints_PlayBack;
                    //刷新折线图显示
                    PlotSpeed.InvalidatePlot(true);
                }
            }
        }
        public void ResetSpeedShowTime()
        {
            Model_Speed.DateTime_Current = DateTime.MinValue;
        }
        public void GetSpeedData()
        {
            string path = GetDirPath() + "\\simulatespeeddata_1.sql";
            List<string> sqlArrayList = commMariaDB.GetSqlFileAndSplit_Speed(path);
            UpdateSpeedData(sqlArrayList);
        }
        private string GetDirPath()
        {
            string strDir = string.Format(@"{0}\CacheData\simSpeedData", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
            DirectoryInfo dir = new DirectoryInfo(strDir);
            if (!dir.Exists)
            {
                CreateDir(strDir);
            }
            return strDir;
        }
        private void CreateDir(string directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            dirInfo.Create();
        }
        private void UpdateSpeedData(List<string> arrayList)
        {
            mo.dtSpeedData.Clear();
            DataRow dr;
            foreach (var arr in arrayList)
            {
                string split = ", ";
                string[] data = Regex.Split(arr.ToString(), split, RegexOptions.IgnoreCase);
                if (data.Count() >= 11)
                {
                    dr = mo.dtSpeedData.NewRow();
                    dr["ID"] = data[0];
                    dr["VechicleID"] = data[1];
                    dr["Speed"] = data[2];
                    dr["Unit"] = data[3].Replace("'", "");
                    dr["Speed_X"] = data[4];
                    dr["Unit_X"] = data[5].Replace("'", "");
                    dr["Speed_Y"] = data[6];
                    dr["Unit_Y"] = data[7].Replace("'", "");
                    dr["Speed_W"] = data[8];
                    dr["Unit_W"] = data[9].Replace("'", "");
                    dr["CurrentTime"] = data[10].Replace("'", "");
                    mo.dtSpeedData.Rows.Add(dr);
                }
            }
            arrayList.Clear();
            mo.dtSpeedData.DefaultView.Sort = "CurrentTime Desc";
            mo.dtSpeedData = mo.dtSpeedData.DefaultView.ToTable();
            GC.Collect();
        }
        private void GetSpeedDataByTime(DateTime current)
        {
            int index = -1;
            for (int i = 0; i < mo.dtSpeedData.Rows.Count; i++)
            {
                DateTime dt = Convert.ToDateTime(mo.dtSpeedData.Rows[i]["CurrentTime"]);
                if (current > dt)
                    break;
                else
                    index = i;
            }
            if (index != -1)
            {
                Model_Speed.Speed = mo.dtSpeedData.Rows[index]["Speed"].ToString();
                Model_Speed.Speed_X = mo.dtSpeedData.Rows[index]["Speed_X"].ToString();
                Model_Speed.Speed_Y = mo.dtSpeedData.Rows[index]["Speed_Y"].ToString();
                Model_Speed.Speed_W = mo.dtSpeedData.Rows[index]["Speed_W"].ToString();
            }
            else
            {
                Model_Speed.Speed = "";
                Model_Speed.Speed_X = "";
                Model_Speed.Speed_Y = "";
                Model_Speed.Speed_W = "";
            }
        }
        private void SetDataPoints_PlayBack()
        {
            if (Model_Speed.Speed != "")
            {
                double speed = Convert.ToDouble(Model_Speed.Speed) / 1000.000d;
                if (dataPoints_PlayBack.Count == 0)
                {
                    dataPoints_PlayBack.Add(new DataPoint(0, speed));
                }
                else
                {
                    if (dataPoints_PlayBack.Count >= 7)
                        dataPoints_PlayBack.RemoveAt(dataPoints_PlayBack.Count - 1);
                    for (int i = 0; i < dataPoints_PlayBack.Count; i++)
                    {
                        dataPoints_PlayBack[i] = new DataPoint(dataPoints_PlayBack[i].X - 1, dataPoints_PlayBack[i].Y);
                    }
                    dataPoints_PlayBack.Insert(0, new DataPoint(0, speed));
                }
            }
        }
    }
}

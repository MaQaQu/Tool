using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using SuperPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using System.Windows.Threading;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;
using YouiToolkit.Models;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMaintain.xaml 的交互逻辑
    /// </summary>
    public partial class PageMaintain : UserControl
    {
        Models.PageMaintainModel mo = Models.PageMaintainModel.CreateInstance();
        Models.PageMtMapRenderModel mapModel = Models.PageMtMapRenderModel.CreateInstance();
        internal PageMtMapRender PageMapRender { get; private set; }
        internal PageMtMapRenderForShow pageMtChartRender2 { get; set; }
        internal PageMtChartRender pageMtChartRender { get; set; }
        internal PageMtMapRenderViewModel pageMtMapRenderViewModel = null;
        DispatcherTimer timer;
        DispatcherTimer timer_Render;
        int playSpeed = 5;
        int playTick = 0;
        int playAddTime = 1000;
        int playAddTick = 0;
        int palyAddMultiple = 0;
        bool changePlayTime = false;
        bool startGoBack = false;
        bool startGofoward = false;
        bool isPlayWhenPressSlider = false;
        DateTime dtChangePlayTime = DateTime.MinValue;
        public PageMaintain()
        {
            InitializeComponent();
            PageMapRender = new PageMtMapRender();
            pageMtChartRender2 = new PageMtMapRenderForShow();
            pageMtChartRender = new PageMtChartRender();
            Reload();
        }
        private void Reload()
        {
            mapRender.ContextChanged += (s, e) =>
            {
                // 地图渲染状态信息变化事件
                mapRender.Context.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(mapRender.Context.MouseLoctMap):
                            runMouseLoctMap.Text = mapRender.Context.ToString(e.PropertyName);
                            break;
                        case nameof(mapRender.Context.OriginRotate):
                            runOriginRotate.Text = mapRender.Context.ToString(e.PropertyName);
                            break;
                        case nameof(mapRender.Context.OriginScale):
                            runOriginScale.Text = mapRender.Context.ToString(e.PropertyName);
                            break;
                    }
                };
            };
            List<string> lPlaySpeeds = new List<string>() { "0.5 x", "1.0 x", "1.5 x", "2.0 x" };
            cbPlaySpeed.ItemsSource = lPlaySpeeds;
            cbPlaySpeed.SelectedIndex = 1;
            //gridMap.ShowSubPage(PageMapRender);//MQChange
            //PageMapRender.Reload(MapRenderReloadTarget.MapCapture);
            gridChart.ShowSubPage(pageMtChartRender);
            pageMtMapRenderViewModel = new PageMtMapRenderViewModel();
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);//设置的间隔为100ms
                timer.Tick += timer_Tick;
                timer.IsEnabled = true;
            }
            if (timer_Render == null)
            {
                timer_Render = new DispatcherTimer();
                timer_Render.Interval = new TimeSpan(0, 0, 0, 0, 200);//设置的间隔为200ms
                timer_Render.Tick += timer_Render_Tick;
                timer_Render.IsEnabled = true;
            }
            sliderTime.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(sliderTime_MouseLeftButtonUp), true);
            sliderTime.AddHandler(Slider.MouseLeftButtonDownEvent, new MouseButtonEventHandler(sliderTime_MouseLeftButtonDown), true);
            buttonGoback.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(buttonGoback_MouseLeftButtonDown), true);
            buttonGoback.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(buttonGoback_MouseLeftButtonUp), true);
            buttonGofoward.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(buttonGofoward_MouseLeftButtonDown), true);
            buttonGofoward.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(buttonGofoward_MouseLeftButtonUp), true);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                switch (pageMtMapRenderViewModel.mapModel.ShowType)
                {
                    case (int)MtNavDataShowType.RealTime:
                        tbShowType.Text = "实时";
                        btChangeShowType.ToolTip = "点击切换为回放模式";
                        spPlayControls.Visibility = Visibility.Hidden;
                        break;
                    case (int)MtNavDataShowType.PlayBack:
                        tbShowType.Text = "回放";
                        btChangeShowType.ToolTip = "点击切换为实时模式";
                        spPlayControls.Visibility = Visibility.Visible;
                        break;
                }
            }
            catch { }
        }
        private void timer_Render_Tick(object sender, EventArgs e)
        {
            try
            {
                if (mo.videoPlayingFlag)
                {
                    DoChangeSliderPlayTime();
                    DoGobackOrfowardPlayTime();
                    DoChangePlayData();
                }
                if (mo.updateTimeBarFlag)
                {
                    DoUpdateTimeBar();
                }
            }
            catch { }
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            //弹出文件框
            OpenFileDialog openFileDialog = new()
            {
                Title = "选择数据源文件",
                Filter = "sql文件|*.sql",
                FileName = string.Empty,
                FilterIndex = 1,
                Multiselect = false,
                RestoreDirectory = true,
                DefaultExt = "sql",
            };
            if (openFileDialog.ShowDialog() ?? false)
            {
                //选择MariaDB文件
                string txtFile = openFileDialog.FileName;
                //读取文件内容
                if (pageMtMapRenderViewModel.OpenNavDataSqlFile(txtFile))
                {
                    mo.updateTimeBarFlag = true;
                    DispatcherHelper.BeginInvoke(() => Toast.Success(this, "打开成功！", ToastLocation.TopCenter));
                    stopPlay();
                }
                else
                {
                    DispatcherHelper.BeginInvoke(() => Toast.Success(this, "打开失败！", ToastLocation.TopCenter));
                }

            }
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(mo.strNavDataCacheFilePath))
            {
                DispatcherHelper.BeginInvoke(() => Toast.Error(this, "请先执行读取文件操作！", ToastLocation.TopCenter));
                //gridMap.ShowSubPage(pageMtChartRender2);
                MessageBoxX.Warning(this, "请先执行读取文件操作！", "保存失败！");
                //gridMap.ShowSubPage(PageMapRender);
                return;
            }
            SaveFileDialog saveFileDialog = new()
            {
                RestoreDirectory = true,
                DefaultExt = "*.sql;",
                Filter = "sql文件|*.sql",
            };
            if (saveFileDialog.ShowDialog() ?? false)
            {
                try
                {
                    string toPath = saveFileDialog.FileName.ToString();
                    if (pageMtMapRenderViewModel.SaveNavDataSqlFile(toPath))
                    {
                        DispatcherHelper.BeginInvoke(() => Toast.Success(this, "保存成功！", ToastLocation.TopCenter));
                    }
                    else
                    {
                        DispatcherHelper.BeginInvoke(() => Toast.Error(this, "保存失败！", ToastLocation.TopCenter));
                    }
                }
                catch (Exception ex)
                {
                    //gridMap.ShowSubPage(pageMtChartRender2);
                    MessageBoxX.Error(this, $"详细错误：{ex}", "保存失败");
                    //gridMap.ShowSubPage(PageMapRender);
                }
            }
        }
        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PageMtNavigationDataDownload nd = new PageMtNavigationDataDownload();
                nd.Topmost = true;
                nd.ShowDialog();
            }
            catch { }
        }
        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            switch (mo.videoPlayingFlag)
            {
                case false:
                    tbPlay.Style = (Style)FindResource("iconStop");
                    buttonPlay.ToolTip = "停止";
                    mo.videoPlayingFlag = true;
                    break;
                case true:
                    tbPlay.Style = (Style)FindResource("iconStart");
                    buttonPlay.ToolTip = "启动";
                    mo.videoPlayingFlag = false;
                    break;
            }
        }
        private void ButtonSuspend_Click(object sender, RoutedEventArgs e)
        {
            stopPlay();
        }
        private void buttonGoback_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startGoBack = true;
        }
        private void buttonGoback_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            startGoBack = false;
        }
        private void buttonGofoward_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startGofoward = true;
        }
        private void buttonGofoward_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            startGofoward = false;
        }
        private void cbPlaySpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbPlaySpeed.SelectedIndex)
            {
                case 0: playSpeed = 10; break;
                case 1: playSpeed = 5; break;
                case 2: playSpeed = 2; break;
                case 3: playSpeed = 1; break;
                default: playSpeed = 5; break;
            }
        }
        private void sliderTime_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dtChangePlayTime = new DateTime((long)sliderTime.Value);
            changePlayTime = true;
            mo.videoPlayingFlag = isPlayWhenPressSlider;
        }
        private void sliderTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPlayWhenPressSlider = mo.videoPlayingFlag;
            mo.videoPlayingFlag = false;
        }
        private void btChangeShowType_Click(object sender, RoutedEventArgs e)
        {
            stopPlay();
            pageMtMapRenderViewModel.ChangeShowType();
        }

        private void DoChangeSliderPlayTime()
        {
            if (changePlayTime)
            {
                changePlayTime = false;
                mapModel.PlayTime = dtChangePlayTime;
                textTime.Text = mapModel.PlayTime.ToString("yy/MM/dd HH:mm:ss");
                sliderTime.Value = mapModel.PlayTime.Ticks;
            }
        }
        private void DoGobackOrfowardPlayTime()
        {
            if (startGoBack || startGofoward)
            {
                //计次 增量递增
                if (playAddTick % 2 == 0)
                {
                    playAddTick = 1;
                    palyAddMultiple = palyAddMultiple >= 600 ? 600 : palyAddMultiple + 3;
                }
                else
                {
                    playAddTick++;
                }
                int addTime;
                switch (cbPlaySpeed.SelectedIndex)
                {
                    case 0: addTime = 500 * palyAddMultiple; break;
                    case 1: addTime = 1000 * palyAddMultiple; break;
                    case 2: addTime = 1500 * palyAddMultiple; break;
                    case 3: addTime = 2000 * palyAddMultiple; break;
                    default: addTime = 1000; break;
                }
                playAddTime = startGoBack ? -addTime : addTime;
            }
            else
            { playAddTime = 1000; playAddTick = 0; palyAddMultiple = 0; }
        }
        private void DoChangePlayData()
        {
            if (playTick % playSpeed == 0)
            {
                playTick = 1;
                if (pageMtMapRenderViewModel.IsInDate(mapModel.PlayTime, mapModel.StartTime, mapModel.EndTime))
                {
                    DateTime dtTemp = pageMtMapRenderViewModel.SetInDate(mapModel.PlayTime.AddMilliseconds(playAddTime), mapModel.StartTime, mapModel.EndTime);
                    if (mapModel.PlayTime != dtTemp)
                    {
                        mapModel.PlayTime = dtTemp;
                        pageMtMapRenderViewModel.PlayNavData(mapModel.PlayTime);
                    }
                }
                textTime.Text = mapModel.PlayTime.ToString("yy/MM/dd HH:mm:ss");
                sliderTime.Value = mapModel.PlayTime.Ticks;
            }
            else
            {
                playTick++;
            }
        }
        private void DoUpdateTimeBar()
        {
            pageMtMapRenderViewModel.ChangeShowTypeTo(MtNavDataShowType.PlayBack);
            pageMtMapRenderViewModel.GetAlarmData();
            spPlayControls.Visibility = Visibility.Visible;
            sliderTime.Minimum = mapModel.StartTime.Ticks;
            sliderTime.Maximum = mapModel.EndTime.Ticks;
            sliderTime.Ticks = GetAlarmTicks();
            mo.updateTimeBarFlag = false;
        }
        private DoubleCollection GetAlarmTicks()
        {
            DoubleCollection dc = new DoubleCollection();
            try
            {
                for (int i = 0; i < mo.dtAlarmData.Rows.Count; i++)
                {
                    double d = Convert.ToDateTime(mo.dtAlarmData.Rows[i]["StartTime"]).Ticks;
                    dc.Add(d);
                }
            }
            catch { }
            return dc;
        }
        private void stopPlay()
        {
            tbPlay.Style = (Style)FindResource("iconStart");
            buttonPlay.ToolTip = "启动";
            mo.videoPlayingFlag = false;
            startGoBack = false;
            startGofoward = false;
            mapModel.PlayTime = mapModel.StartTime;
            pageMtMapRenderViewModel.PlayNavData(mapModel.PlayTime);
            textTime.Text = mapModel.PlayTime.ToString("yy/MM/dd HH:mm:ss");
            sliderTime.Value = mapModel.PlayTime.Ticks;
        }
    }
}

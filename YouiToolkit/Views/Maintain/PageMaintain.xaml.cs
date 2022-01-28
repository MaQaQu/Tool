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
using YouiToolkit.Design;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMaintain.xaml 的交互逻辑
    /// </summary>
    public partial class PageMaintain : UserControl
    {
        Models.PageMaintainModel mo = Models.PageMaintainModel.CreateInstance();
        internal PageMtMapRender PageMapRender { get; private set; }
        internal PageMtMapRenderForShow pageMtChartRender2 { get; set; }
        internal PageMtChartRender pageMtChartRender { get; set; }
        internal PageMtMapRenderViewModel pageMtMapRenderViewModel = null;
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
            List<string> lPlaySpeeds = new List<string>() { "0.5 x", "1.0 x", "1.5 x", "2.0 x" };
            cbPlaySpeed.ItemsSource = lPlaySpeeds;
            cbPlaySpeed.SelectedIndex = 1;
            gridMap.ShowSubPage(PageMapRender);
            gridChart.ShowSubPage(pageMtChartRender);
            PageMapRender.Reload(MapRenderReloadTarget.MapCapture);
            pageMtMapRenderViewModel = new PageMtMapRenderViewModel();
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
                    DispatcherHelper.BeginInvoke(() => Toast.Success(this, "打开成功！", ToastLocation.TopCenter));
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
                gridMap.ShowSubPage(pageMtChartRender2);
                MessageBoxX.Warning(this, "请先执行读取文件操作！", "保存失败！");
                gridMap.ShowSubPage(PageMapRender);
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
                    gridMap.ShowSubPage(pageMtChartRender2);
                    MessageBoxX.Error(this, $"详细错误：{ex}", "保存失败");
                    gridMap.ShowSubPage(PageMapRender);
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

        private void ButtonSuspend_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

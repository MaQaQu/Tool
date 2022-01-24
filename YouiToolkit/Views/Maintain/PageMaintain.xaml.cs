using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using SuperPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        internal PageMtMapRender PageMapRender { get; private set; }
        internal PageMtChartRender pageMtChartRender { get; set; }
        public PageMaintain()
        {
            InitializeComponent();
            PageMapRender = new PageMtMapRender();
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
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModelLocator.PageMaintain.maintainModel.videoPlayingFlag)
            {
                case false:
                    tbPlay.Style = (Style)FindResource("iconStop");
                    buttonPlay.ToolTip = "停止";
                    ViewModelLocator.PageMaintain.maintainModel.videoPlayingFlag = true;
                    break;
                case true:
                    tbPlay.Style = (Style)FindResource("iconStart");
                    buttonPlay.ToolTip = "启动";
                    ViewModelLocator.PageMaintain.maintainModel.videoPlayingFlag = false;
                    break;
            }
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            //弹出文件框
            OpenFileDialog openFileDialog = new()
            {
                Title = "选择数据源文件",
                Filter = "csv文件|*.csv",
                FileName = string.Empty,
                FilterIndex = 1,
                Multiselect = false,
                RestoreDirectory = true,
                DefaultExt = "csv",
            };
            if (openFileDialog.ShowDialog() ?? false)
            {
                //选择MariaDB文件
                string txtFile = openFileDialog.FileName;
                //读取文件内容

            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                RestoreDirectory = true,
                DefaultExt = "*.csv;",
                Filter = "csv文件|*.csv",
            };
            if (saveFileDialog.ShowDialog() ?? false)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    MessageBoxX.Error(this, $"详细错误：{ex}", "保存失败");
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
    }
}

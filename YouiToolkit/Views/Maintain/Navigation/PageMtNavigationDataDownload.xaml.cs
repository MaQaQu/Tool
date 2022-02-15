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
using System.Windows.Shapes;
using System.Windows.Threading;
using YouiToolkit.Design;
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMtNavigationDataDownload.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtNavigationDataDownload : Window
    {
        PageMtMapRenderViewModel pageMtMapRenderViewModel = null;
        StackPanel sp = new StackPanel();
        string selectedFileName = "";
        PageMtNavDataConfirm deleteConfirm = null;
        PageMtNavDataConfirm downloadConfirm = null;
        DispatcherTimer timer;
        public PageMtNavigationDataDownload()
        {
            InitializeComponent();
            InitControl();
            pageMtMapRenderViewModel = new PageMtMapRenderViewModel();
            UpdateNavFilsList();
        }
        public void OverDownload()
        {
            if (downloadConfirm == null)
                this.Close();
        }
        private void InitControl()
        {
            KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                    case Key.Space:
                        ButtonDownload_Click(buttonDownload, new RoutedEventArgs());
                        break;
                    case Key.Escape:
                        ButtonCancle_Click(buttonCancle, new RoutedEventArgs());
                        break;
                    case Key.Delete:
                        ButtonDelete_Click(buttonDelete, new RoutedEventArgs());
                        break;
                }
            };
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);//设置的间隔为100ms
                timer.Tick += timer_Tick;
                timer.IsEnabled = true;
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                refreshDownloadState();
                refreshDeleteState();
            }
            catch { }
        }
        private void refreshDownloadState()
        {
            if (pageMtMapRenderViewModel.mo.overDownloadConfirmFlag)
            {
                if (downloadConfirm != null)
                {
                    downloadConfirm.Close();
                    pageMtMapRenderViewModel.mo.navDataDownloadStep = 0;
                    pageMtMapRenderViewModel.mo.downloadingFlag = false;
                    if (pageMtMapRenderViewModel.mo.navDataDownloadStep == 100)
                        pageMtMapRenderViewModel.mo.updateTimeBarFlag = true;
                    pageMtMapRenderViewModel.mo.overDownloadFlag = true;
                    pageMtMapRenderViewModel.mo.overDownloadConfirmFlag = false;
                    downloadConfirm = null;
                }
            }
        }
        private void refreshDeleteState()
        {
            if (pageMtMapRenderViewModel.mo.overDeleteConfirmFlag)
            {
                if (deleteConfirm != null)
                {
                    deleteConfirm.Close();
                    pageMtMapRenderViewModel.mo.navDataDeleteStep = 0;
                    pageMtMapRenderViewModel.mo.deletingFlag = false;
                    pageMtMapRenderViewModel.mo.overDeleteFlag = true;
                    pageMtMapRenderViewModel.mo.overDeleteConfirmFlag = false;
                    stackPanelNavFiles.Children.Clear();
                    UpdateNavFilsList();
                    deleteConfirm = null;
                }
            }
        }

        private void UpdateNavFilsList()
        {
            try
            {
                selectedFileName = "";
                pageMtMapRenderViewModel.GetFilesNameTable();
                System.Data.DataTable dt = pageMtMapRenderViewModel.mo.dtNavFilesName;
                if (dt != null && dt.Rows.Count > 0)
                {
                    string tbText = "";
                    int radioInPanelCount = 0;
                    bool doAddPanel = false;
                    NewPanel();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string id = dt.Rows[i]["ID"].ToString();
                        string name = dt.Rows[i]["FilesName"].ToString();
                        string date = dt.Rows[i]["Date"].ToString();
                        string starttime = dt.Rows[i]["StartTime"].ToString();
                        string endtime = dt.Rows[i]["EndTime"].ToString();
                        DateTime dtCurrent = Convert.ToDateTime(date);
                        string week = pageMtMapRenderViewModel.ConvertWeekToChn((int)dtCurrent.DayOfWeek);
                        string tbText_i = string.Format("{0}  {1}", dtCurrent.ToString("yyyy-MM-dd"), week);
                        if (doAddPanel && radioInPanelCount != 0)
                        {
                            radioInPanelCount = 0;
                            AddRaidoButtontoPanel();
                            NewPanel();
                            doAddPanel = false;
                        }
                        if (tbText != tbText_i)//新建日期栏
                        {
                            if (i != 0)
                                AddRaidoButtontoPanel();
                            NewPanel();
                            tbText = tbText_i;
                            TextBlock tb = new TextBlock();
                            tb.Text = tbText;
                            tb.FontSize = 20;
                            tb.Margin = new Thickness(30, 10, 3, 10);
                            tb.HorizontalAlignment = HorizontalAlignment.Left;
                            stackPanelNavFiles.Children.Add(tb);
                        }
                        if (radioInPanelCount < 4)
                        {
                            RadioButton rb = new RadioButton();
                            rb.Name = string.Format("{0}", name);
                            rb.Content = string.Format("{0}-{1}", starttime, endtime);
                            rb.Margin = new Thickness(15, 5, 0, 5);
                            rb.GroupName = "navFiles";
                            rb.Checked += radioButtonSelected;
                            sp.Children.Add(rb);
                            radioInPanelCount++;
                        }
                        if (radioInPanelCount == 4)
                        {
                            doAddPanel = true;
                        }
                        if (i == dt.Rows.Count - 1)
                        {
                            AddRaidoButtontoPanel();
                        }
                    }
                }
            }
            catch { }
        }
        private void AddRaidoButtontoPanel()
        {
            stackPanelNavFiles.Children.Add(sp);
        }
        private void NewPanel()
        {
            sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = HorizontalAlignment.Left;
        }
        private void radioButtonSelected(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            selectedFileName = radioButton.Name + ".sql";
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFileName))
            {
                MessageBoxX.Warning(this, "请先选择要下载的文件！", "下载失败");
                return;
            }
            pageMtMapRenderViewModel.mo.overDownloadFlag = false;
            downloadConfirm = new PageMtNavDataConfirm("请确认是否下载选中文件？", (int)Models.MtNavDataDownloadConfirmType.downloadConfirm, selectedFileName);
            downloadConfirm.ShowInTaskbar = false;
            downloadConfirm.Topmost = true;
            downloadConfirm.Show();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFileName))
            {
                MessageBoxX.Warning(this, "请先选择要删除的文件！", "删除失败");
                return;
            }
            pageMtMapRenderViewModel.mo.overDeleteFlag = false;
            deleteConfirm = new PageMtNavDataConfirm("请确认是否删除选中文件？", (int)Models.MtNavDataDownloadConfirmType.deleteConfirm, selectedFileName);
            deleteConfirm.ShowInTaskbar = false;
            deleteConfirm.Topmost = true;
            deleteConfirm.Show();
            //if (pageMtMapRenderViewModel.mo.overDeleteFlag)
            //{
            //    stackPanelNavFiles.Children.Clear();
            //    UpdateNavFilsList();
            //}
        }

        private void BdrMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

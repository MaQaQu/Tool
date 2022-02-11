using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using YouiToolkit.ViewModels;

namespace YouiToolkit.Views
{
    /// <summary>
    /// PageMtNavDataDownloadConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class PageMtNavDataDownloadConfirm : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        Models.PageMaintainModel mo = Models.PageMaintainModel.CreateInstance();
        PageMtMapRenderViewModel pageMtMapRenderViewModel = null;
        private int windowsType;//0-下载确认 1-删除确认
        private string confirmMsg;
        private string fileName;
        public PageMtNavDataDownloadConfirm()
        {
            InitializeComponent();
            InitControl();
        }

        public PageMtNavDataDownloadConfirm(string msg, int type) : this()
        {
            this.confirmMsg = msg;
            this.windowsType = type;
            if (!string.IsNullOrEmpty(confirmMsg))
                tbPrompt.Text = confirmMsg;
        }

        public PageMtNavDataDownloadConfirm(string msg, int type, string name) : this()
        {
            this.confirmMsg = msg;
            this.windowsType = type;
            if (!string.IsNullOrEmpty(confirmMsg))
                tbPrompt.Text = confirmMsg;
            if (!string.IsNullOrEmpty(name))
                fileName = name;
        }

        private void InitControl()
        {
            KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                    case Key.Space:
                        ButtoConfirm_Click(buttonConfirm, new RoutedEventArgs());
                        break;
                    case Key.Escape:
                        ButtonCancel_Click(buttonCancel, new RoutedEventArgs());
                        break;
                }
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);//设置的间隔为100ms
            timer.Tick += timer_Tick;
            timer.IsEnabled = true;
            pageMtMapRenderViewModel = new PageMtMapRenderViewModel();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateDownloadStatus();
            UpdateDeleteStatus();
        }

        private void UpdateDownloadStatus()
        {
            if (mo.downloadingFlag)
                pbDownload.Value = mo.navDataDownloadStep;
            if (mo.downloadingFlag && mo.navDataDownloadStep == 100)
            {
                Thread threadDown = new Thread(new ThreadStart(() => { OverDownload(); }));
                threadDown.Start();
            }
        }
        private void UpdateDeleteStatus()
        {
            if (mo.deletingFlag)
                pbDownload.Value = mo.navDataDeleteStep;
            if (mo.deletingFlag && mo.navDataDeleteStep == 100)
            {
                Thread threadDelete = new Thread(new ThreadStart(() => { OverDelete(); }));
                threadDelete.Start();
            }
        }

        private void OverDownload()
        {
            //下载完成
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                tbPrompt.Text = "下载完成！";
            });
            Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                //this.DialogResult = true;
                pageMtMapRenderViewModel.mo.overDownloadFlag = true;
                pageMtMapRenderViewModel.mo.updateTimeBarFlag = true;
                this.Close();
                mo.downloadingFlag = false;
            });
        }

        private void OverDelete()
        {
            //删除完成
            mo.navDataDeleteStep = 0;
            mo.deletingFlag = false;
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                tbPrompt.Text = "删除完成！";
            });
            Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                //this.DialogResult = true;
                pageMtMapRenderViewModel.mo.overDeleteFlag = true;
                this.Close();
            });
        }

        private void BdrMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.DragMove();
        }

        private void ButtoConfirm_Click(object sender, RoutedEventArgs e)
        {
            switch (windowsType)
            {
                case 1:
                    DoDownLoadConfirm();
                    break;
                case 2:
                    DoDeleteConfirm();
                    break;
                default:
                    break;
            }
        }
        private void DoDownLoadConfirm()
        {
            //确认下载，跳转到下载中弹窗
            mo.navDataDownloadStep = 0;
            mo.downloadingFlag = true;
            tbPrompt.Text = "正在下载，请稍候！";
            tbPrompt.FontSize = 15;
            buttonConfirm.IsEnabled = false;
            tbCancleText.Text = "返回";
            pbDownload.Visibility = Visibility.Visible;
            //ProgressBegin();
            Thread threadDown = new Thread(() =>
            pageMtMapRenderViewModel.DownloadNavData(fileName));
            threadDown.Start();
        }
        private void DoDeleteConfirm()
        {
            mo.navDataDeleteStep = 0;
            mo.deletingFlag = true;
            tbPrompt.Text = "正在删除，请稍后！";
            tbPrompt.FontSize = 15;
            buttonConfirm.IsEnabled = false;
            tbCancleText.Text = "返回";
            pbDownload.Visibility = Visibility.Visible;
            Thread threadDown = new Thread(() =>
            pageMtMapRenderViewModel.DeleteNavData(fileName));
            threadDown.Start();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消下载，退出确认弹窗
            this.Close();
        }

        private void ProgressBegin()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (mo.downloadingFlag)
                {
                    this.Dispatcher.BeginInvoke((ThreadStart)delegate { pbDownload.Value = mo.navDataDownloadStep; });
                    Thread.Sleep(100);
                }
            }));
            thread.Start();
        }
    }
}

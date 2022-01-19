using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouiToolkit.Ctrls;
using YouiToolkit.Design;
using YouiToolkit.Models;
using YouiToolkit.Utils;

namespace YouiToolkit.Views
{
    public partial class MainWindow : WindowX
    {
        public BaseCommand UserCommand => new BaseCommand((s) => Toast.Warning(this, "未实现"));
        public BaseCommand NaviCommand => new BaseCommand((s) => NaviCollapsedRequested());

        public PageRobot PageRobot { get; private set; }
        public PageMap PageMap { get; private set; }
        public PageAvoidObstacle PageAvoidObstacle { get; private set; }
        public PageMaintain PageMaintain { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            AssistManager.Assist.SetIP(Config.Instance.IPAddr);
            AssistManager.Assist.Activate();

            PageRobot = new PageRobot();
            PageMap = new PageMap();
            PageAvoidObstacle = new PageAvoidObstacle();
            PageMaintain = new PageMaintain();

            Loaded += (s, e) => LoadPage(PageRobot);

            KeyDown += (s, e) =>
            {
#if DEBUG
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    MessageCenter.Publish(MessageToken.PageMapListOpCapture, string.Empty);
                    AssistManager.Status.LidarPointCloud.DebugMapPrior(@$"{Environment.CurrentDirectory}..\..\..\..\..\Resources\Maps\map01.png", -32.048275f * 1000f, -5.777359f * 1000f, 0.000000f, 0.030000f * 1000f);
                }
#endif
            };

            AssistManager.Assist.ConnectStatusChanged += (s, e) =>
            {
                if (!e) DispatcherHelper.Invoke(() => Notice.Warning("\n检测到与机器人的连接已断开，请检查网络状态！", "网络连接中断"));
            };

            SizeChanged += (s, e) =>
            {
                //MessageCenter.Publish(MessageToken.PageMapRenderPause, string.Empty, WindowState == WindowState.Minimized);
            };
            IsVisibleChanged += (s, e) =>
            {
                //MessageCenter.Publish(MessageToken.PageMapRenderPause, string.Empty, WindowState == WindowState.Minimized);
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetIconVisibility(Visibility.Collapsed);
            SetTitleVisibility(Visibility.Collapsed);
        }

        protected void NaviCollapsedRequested()
        {
            if (gridNaviColumn.Width.Value > 45)
            {
                ScrollViewerHelper.SetScrollBarThickness(treeViewNavi, 0);
                textBlockVersion.Visibility = Visibility.Collapsed;
                imageTitle.Margin = new Thickness(0);
                imageTitle.Source = null;
                StoryboardUtils.BeginGridLengthStoryboard(gridNaviColumn, ColumnDefinition.WidthProperty, gridNaviColumn.Width, GridLengthUtil.ConvertToGridLength("45"));
            }
            else
            {
                StoryboardUtils.BeginGridLengthStoryboard(gridNaviColumn, ColumnDefinition.WidthProperty, gridNaviColumn.Width, GridLengthUtil.ConvertToGridLength("200"), completed: () =>
                {
                    ScrollViewerHelper.SetScrollBarThickness(treeViewNavi, 0);
                    textBlockVersion.Visibility = Visibility.Visible;
                    imageTitle.Opacity = 0;
                    imageTitle.Source = new BitmapImage(new Uri("/Resources/youibot_subtitle_white.png", UriKind.RelativeOrAbsolute));
                    imageTitle.Margin = new Thickness(0, 15, 0, 0);
                    StoryboardUtils.BeginDoubleStoryboard(imageTitle, Image.OpacityProperty, 0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
                });
            }
        }

        public void LoadPage(UserControl page)
        {
            if (page == null)
            {
                gridMain?.Children?.Clear();
                return;
            }
            if (gridMain.Children.IndexOf(page) >= 0)
            {
                return;
            }
            gridMain.Children.Clear();
            gridMain.Children.Add(page);
        }

        public void PageRequested(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string tag)
            {
                switch (tag.ToLower())
                {
                    case PageTag.Robot:
                        LoadPage(PageRobot);
                        break;
                    case PageTag.Map:
                        LoadPage(PageMap);
                        break;
                    case PageTag.AvoidObstacle:
                        LoadPage(PageAvoidObstacle);
                        break;
                    case PageTag.Maintain:
                        LoadPage(PageMaintain);
                        break;
                    default:
                        LoadPage(null);
                        break;
                }
            }
            if (sender is TreeViewItem treeViewItem && labelPageName != null)
            {
                labelPageName.Text = treeViewItem.Header.ToString();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView)
            {
                PageRequested(e.NewValue, e);
            }
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}

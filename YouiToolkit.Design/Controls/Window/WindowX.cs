using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using YouiToolkit.Logging;

namespace YouiToolkit.Design
{
    /// <summary>
    /// 可扩展窗体
    /// </summary>
    [TemplatePart(Name = "PART_GrdCaption", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_BtnMaximize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BtnMinimize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BtnClose", Type = typeof(Button))]
    public partial class WindowX : Window
    {
        #region [依赖属性]
        [DefaultValue(false)]
        public bool IsMaskVisible
        {
            get => (bool)GetValue(IsMaskVisibleProperty);
            set => SetValue(IsMaskVisibleProperty, value);
        }
        public static readonly DependencyProperty IsMaskVisibleProperty = DependencyProperty.Register("IsMaskVisible", typeof(bool), typeof(WindowX));

        public Brush MaskBrush
        {
            get => (Brush)GetValue(MaskBrushProperty);
            set => SetValue(MaskBrushProperty, value);
        }
        public static readonly DependencyProperty MaskBrushProperty = DependencyProperty.Register("MaskBrush", typeof(Brush), typeof(WindowX));

        /// <summary>
        /// 警用强制关闭
        /// ※诸如"Alt + F4"
        /// </summary>
        [DefaultValue(false)]
        public bool DisableForceClosing
        {
            get => (bool)GetValue(DisableForceClosingProperty);
            set => SetValue(DisableForceClosingProperty, value);
        }
        public static readonly DependencyProperty DisableForceClosingProperty = DependencyProperty.Register("DisableForceClosing", typeof(bool), typeof(WindowX));

        internal ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }
        internal static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(WindowX), new PropertyMetadata(new CloseCommand()));

        internal ICommand MinCommand
        {
            get => (ICommand)GetValue(MinCommandProperty);
            set => SetValue(MinCommandProperty, value);
        }
        internal static readonly DependencyProperty MinCommandProperty = DependencyProperty.Register("MinCommand", typeof(ICommand), typeof(WindowX), new PropertyMetadata(new MinCommand()));

        internal ICommand MaxCommand
        {
            get => (ICommand)GetValue(MaxCommandProperty);
            set => SetValue(MaxCommandProperty, value);
        }
        internal static readonly DependencyProperty MaxCommandProperty = DependencyProperty.Register("MaxCommand", typeof(ICommand), typeof(WindowX), new PropertyMetadata(new MaxCommand()));
        #endregion

        #region [属性]
        private bool closeHandler = true;
        private bool restoreIfMove = false;

        public HwndSource HwndSource { get; private set; }
        public IntPtr Handle { get; private set; }
        public WindowState WindowStatePrev { get; internal set; } = WindowState.Normal;

        public BaseCommand ConsoleManagerRequestedCommand
        {
            get => new BaseCommand()
            {
                Action = (o) => ConsoleManager.Show(),
            };
        }
        #endregion

        /// <summary>
        /// 静态构造
        /// </summary>
        static WindowX()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowX), new FrameworkPropertyMetadata(typeof(WindowX)));
        }

        /// <summary>
        /// 构造
        /// </summary>
        public WindowX()
        {
            KeyDown += (s, e) =>
            {
#if DEBUG
                if (e.Key == Key.F12)
                {
                    ConsoleManager.Show();
                }
#endif
            };

            Loaded += (s, e) =>
            {
                HwndSource = PresentationSource.FromVisual(this) as HwndSource;
                HwndSource.AddHook(new HwndSourceHook(WndProc));
                Handle = new WindowInteropHelper(this).Handle;

                if (GetTemplateChild("PART_GrdCaption") is Grid grdTitle)
                {
                    RegisterAsTitleHeader(grdTitle);
                }

                if (GetTemplateChild("PART_BtnMaximize") is Button buttonMaximize)
                {
                    bool isKeepHoving = false;

                    buttonMaximize.MouseEnter += (s, e) =>
                    {
                        DispatcherTimer dt = new DispatcherTimer()
                        {
                            Interval = TimeSpan.FromMilliseconds(2000),
                        };
                        dt.Tick += (s, e) =>
                        {
                            if (isKeepHoving)
                            {
                                // TODO: Win11布局
                            }
                        };
                        isKeepHoving = true;
                        dt.Start();
                    };
                    buttonMaximize.MouseLeave += (s, e) =>
                    {
                        isKeepHoving = false;
                    };
                }
            };

            SizeChanged += (s, e) =>
            {
                if (WindowStatePrev != WindowState)
                {
                    Button d = GetTemplateChild("PART_BtnMaximize") as Button;

#if false
                    FrameworkElement s1 = d?.GetTemplateChild("PART_max1");
                    FrameworkElement s2 = d?.GetTemplateChild("PART_max2");
                    FrameworkElement s3 = d?.GetTemplateChild("PART_max3");

                    if (s1 != null && s2 != null && s3 != null)
                    {
                        switch (WindowState)
                        {
                            case WindowState.Maximized:
                                s1.Visibility = Visibility.Collapsed;
                                s2.Visibility = Visibility.Visible;
                                s3.Visibility = Visibility.Visible;
                                break;
                            default:
                                s1.Visibility = Visibility.Visible;
                                s2.Visibility = Visibility.Collapsed;
                                s3.Visibility = Visibility.Collapsed;
                                break;
                        }
                    }
#else
                    Label label = d?.GetTemplateChild("PART_max1") as Label;

                    if (label != null)
                    {
                        switch (WindowState)
                        {
                            case WindowState.Maximized:
                                //label.Content = IconFontHelper.GetSymbol("&#xea0c;");
                                label.Content = IconFontHelper.GetSymbol("&#xea0d;");
                                break;
                            default:
                                label.Content = IconFontHelper.GetSymbol("&#xea0a;");
                                break;
                        }
                    }
#endif
                    WindowStatePrev = WindowState;
                }
            };
        }

        /// <summary>
        /// 全局事件处理
        /// </summary>
        protected private virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => (IntPtr)0;

        /// <summary>
        /// 请求注册窗体标题栏行为
        /// </summary>
        public virtual void RegisterAsTitleHeaderRequested(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement titleHeader)
            {
                RegisterAsTitleHeader(titleHeader);
            }
        }

        /// <summary>
        /// 注册窗体标题栏行为
        /// </summary>
        public virtual void RegisterAsTitleHeader(UIElement titleHeader)
        {
            titleHeader.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2)
                {
                    if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
                    {
                        switch (WindowState)
                        {
                            case WindowState.Normal:
                                WindowState = WindowState.Maximized;
                                break;
                            case WindowState.Maximized:
                                WindowState = WindowState.Normal;
                                break;
                        }
                    }
                }
                else
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        restoreIfMove = true;
                    }
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragMove();
                    }
                }
            };

            titleHeader.MouseLeftButtonUp += (s, e) => restoreIfMove = false;

            titleHeader.MouseMove += (s, e) =>
            {
                if (restoreIfMove)
                {
                    restoreIfMove = false;
                    var mouseX = e.GetPosition(this).X;
                    var width = RestoreBounds.Width;
                    var x = mouseX - width / 2;

                    if (x < 0)
                    {
                        x = 0;
                    }
                    else if (x + width > SystemParameters.WorkArea.Width)
                    {
                        x = SystemParameters.WorkArea.Width - width;
                    }

                    Left = x;
                    Top = 0;
                    WindowState = WindowState.Normal;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragMove();
                    }
                }
            };
        }

        /// <summary>
        /// 内容渲染事件处理
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            InvalidateVisual();
        }

        /// <summary>
        /// 退出中事件处理
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DisableForceClosing && closeHandler)
            {
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// 强制关闭
        /// </summary>
        public void ForceClose()
        {
            closeHandler = false;
            Close();
        }

        public void SetIconVisibility(Visibility v)
        {
            FrameworkElement image = GetTemplateChild("PART_ImgIcon") as FrameworkElement;

            if (image != null)
            {
                image.Visibility = v;
            }
        }

        public void SetTitleVisibility(Visibility v)
        {
            FrameworkElement title = GetTemplateChild("PART_TxtTitle") as FrameworkElement;

            if (title != null)
            {
                title.Visibility = v;
            }
        }
    }

    /// <summary>
    /// 关闭命令
    /// </summary>
    internal class CloseCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var window = parameter as WindowX;
            window.ForceClose();
        }
    }

    /// <summary>
    /// 最大化命令
    /// </summary>
    internal class MaxCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var window = parameter as WindowX;

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
            }
            else
            {
                window.WindowState = WindowState.Maximized;
            }
        }
    }

    /// <summary>
    /// 最小化命令
    /// </summary>
    internal class MinCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => (parameter as WindowX).WindowState = WindowState.Minimized;
    }
}

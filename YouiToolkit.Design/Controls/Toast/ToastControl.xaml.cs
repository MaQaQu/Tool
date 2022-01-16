using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace YouiToolkit.Design
{
    public partial class ToastControl : UserControl
    {
        public Window Window { get; internal set; } = null;
        public FrameworkElement Owner { get; private set; } = null;
        public Popup Popup { get; internal set; } = null;
        public DispatcherTimer Timer { get; set; } = null;
        public Thickness OffsetMargin { get; set; } = new Thickness(15);
        public ToastConfig Options { get; set; } = null;

        public ToastControl() : this(null, string.Empty)
        {
        }

        public ToastControl(FrameworkElement owner, string message, ToastConfig options = null)
        {
            InitializeComponent();
            DataContext = this;
            Options = options;
            SetupConfig();
            Message = message;
            Owner = owner ?? Application.Current.MainWindow;
            Window = Owner.FindWindow();
            Window.Closed += (s, e) => Close();

            Size fontSize = textBlockToast.MeasureString(message);
            columnText.Width = new GridLength(fontSize.Width);
            Width = columnText.Width.Value + columnIcon.Width.Value + 12;
        }

        private void SetupConfig()
        {
            if (Options != null)
            {
                Height = Options.Height;
                Time = Options.Time;
                MessageBoxIcon = Options.MessageBoxIcon;
                IconSize = Options.IconSize;
                Location = Options.Location;
                Background = Options.Background;
                Foreground = Options.Foreground;
                FontStyle = Options.FontStyle;
                FontStretch = Options.FontStretch;
                FontSize = Options.FontSize;
                FontFamily = Options.FontFamily;
                FontWeight = Options.FontWeight;
                BorderBrush = Options.BorderBrush;
                BorderThickness = Options.BorderThickness;
                CornerRadius = Options.CornerRadius;
                HorizontalContentAlignment = Options.HorizontalContentAlignment;
                VerticalContentAlignment = Options.VerticalContentAlignment;
                OffsetMargin = Options.OffsetMargin;
            }
        }

        internal void ShowCore()
        {
            DispatcherHelper.Invoke(() =>
            {
                Popup = new Popup()
                {
                    Width = Width,
                    Height = Height,
                    PopupAnimation = PopupAnimation.Fade,
                    AllowsTransparency = true,
                    StaysOpen = true,
                    Placement = PlacementMode.Relative,
                    IsOpen = false,
                    Child = this,
                };
                Popup.PlacementTarget = Window;
                Window.LocationChanged += UpdatePosition;
                Window.SizeChanged += UpdatePosition;
                SetPopupOffset(Popup, this);
                Popup.Closed += (s, e) =>
                {
                    Popup popup = s as Popup;
                    if (popup == null)
                    {
                        return;
                    }
                    ToastControl t = popup.Child as ToastControl;
                    if (t == null)
                    {
                        return;
                    }
                };
                Popup.IsOpen = true;
            });
            Timer = new DispatcherTimer();
            Timer.Tick += (sender, e) =>
            {
                Popup.IsOpen = false;
                Window.LocationChanged -= UpdatePosition;
                Window.SizeChanged -= UpdatePosition;
            };
            Timer.Interval = new TimeSpan(0, 0, 0, 0, Options.Time);
            Timer.Start();
        }

        internal virtual void UpdatePosition(object sender, EventArgs e)
        {
            var up = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (up == null || Popup == null)
            {
                return;
            }
            SetPopupOffset(Popup, this);
            up.Invoke(Popup, null);
        }

        internal Point GetRelativeLocation() => Owner.RelativeLocation(Window);

        internal static void SetPopupOffset(Popup popup, ToastControl toast)
        {
            double ownerWidth = toast.Owner.ActualWidth;
            double ownerHeight = toast.Owner.ActualHeight;
            double x = SystemParameters.WorkArea.X * Application.Current.MainWindow.GetDpiScale();
            double y = SystemParameters.WorkArea.Y * Application.Current.MainWindow.GetDpiScale();
            Point p = toast.GetRelativeLocation();

            if (popup.PlacementTarget == null)
            {
                ownerWidth = SystemParameters.WorkArea.Size.Width * Application.Current.MainWindow.GetDpiScale();
                ownerHeight = SystemParameters.WorkArea.Size.Height * Application.Current.MainWindow.GetDpiScale();
            }
            switch (toast.Location)
            {
                case ToastLocation.Left:
                    popup.HorizontalOffset = popup.Width;
                    popup.VerticalOffset = (ownerHeight - popup.Height - 38) / 2;
                    break;
                case ToastLocation.Right:
                    popup.HorizontalOffset = ownerWidth - 16;
                    popup.VerticalOffset = (ownerHeight - popup.Height - 38) / 2;
                    break;
                case ToastLocation.TopLeft:
                    popup.HorizontalOffset = popup.Width;
                    break;
                case ToastLocation.TopCenter:
                    popup.HorizontalOffset = (ownerWidth - popup.Width - 16) / 2 + p.X;
                    popup.VerticalOffset = toast.OffsetMargin.Top + p.Y;
                    break;
                case ToastLocation.TopRight:
                    popup.HorizontalOffset = ownerWidth - 16;
                    break;
                case ToastLocation.BottomLeft:
                    popup.HorizontalOffset = popup.Width;
                    popup.VerticalOffset = ownerHeight - popup.Height - 38;
                    break;
                case ToastLocation.BottomCenter:
                    popup.HorizontalOffset = (ownerWidth - popup.Width - 16) / 2;
                    popup.VerticalOffset = ownerHeight - popup.Height - 38;
                    break;
                case ToastLocation.BottomRight:
                    popup.HorizontalOffset = (ownerWidth - popup.Width - 16);
                    popup.VerticalOffset = ownerHeight - popup.Height - 38;
                    break;
                case ToastLocation.Center:
                    popup.HorizontalOffset = (ownerWidth - popup.Width - 16) / 2;
                    popup.VerticalOffset = (ownerHeight - popup.Height - 38) / 2;
                    break;
            }
            //Trace.WriteLine($"toast: {popup.HorizontalOffset}, {popup.VerticalOffset}");
        }

        public void Close()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
            Popup.IsOpen = false;
            Window.LocationChanged -= UpdatePosition;
            Window.SizeChanged -= UpdatePosition;
        }

        private string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        private static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ToastControl), new PropertyMetadata(string.Empty));

        private CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        private static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToastControl), new PropertyMetadata(new CornerRadius(5)));

        private double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        private static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(ToastControl), new PropertyMetadata(26.0));

        private new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        private static new readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ToastControl), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#E1E1E1")));

        private new Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
        private static new readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ToastControl), new PropertyMetadata(new Thickness(0.5d)));

        private new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        private static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(ToastControl), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#FAFAFA")));

        private new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }
        private static new readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ToastControl), new PropertyMetadata(HorizontalAlignment.Left));

        private new VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }
        private static new readonly DependencyProperty VerticalContentAlignmentProperty = DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(ToastControl), new PropertyMetadata(VerticalAlignment.Center));

        private new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        private new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ToastControl), new PropertyMetadata(100d));

        private new double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        private new static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(ToastControl), new PropertyMetadata(48.0));

        public MessageBoxIcon MessageBoxIcon
        {
            get { return (MessageBoxIcon)GetValue(MessageBoxIconProperty); }
            set { SetValue(MessageBoxIconProperty, value); }
        }

        public static readonly DependencyProperty MessageBoxIconProperty = DependencyProperty.Register("MessageBoxIcon", typeof(MessageBoxIcon), typeof(ToastControl));

        private int Time
        {
            get { return (int)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }
        private static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(int), typeof(ToastControl), new PropertyMetadata(ToastConfig.NormalTime));

        private ToastLocation Location
        {
            get { return (ToastLocation)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
        private static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(ToastLocation), typeof(ToastControl), new PropertyMetadata(ToastLocation.TopCenter));
    }
}

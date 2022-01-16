using System.Windows;
using System.Windows.Controls;

namespace YouiToolkit.Design
{
    internal partial class MessageBoxXIcon : UserControl
    {
        public MessageBoxXIcon()
        {
            InitializeComponent();
        }

        public MessageBoxIcon MessageBoxIcon
        {
            get { return (MessageBoxIcon)GetValue(MessageBoxIconProperty); }
            set { SetValue(MessageBoxIconProperty, value); }
        }

        public static readonly DependencyProperty MessageBoxIconProperty =
            DependencyProperty.Register("MessageBoxIcon", typeof(MessageBoxIcon), typeof(MessageBoxXIcon), new PropertyMetadata(MessageBoxIcon.None, OnMessageBoxIconChanged));


        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(MessageBoxXIcon), new PropertyMetadata(5.0));


        private static void OnMessageBoxIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var icon = d as MessageBoxXIcon;
            switch (icon.MessageBoxIcon)
            {
                case MessageBoxIcon.Success:
                    icon.GrdSuccess.Visibility = Visibility.Visible;
                    break;
                case MessageBoxIcon.Info:
                    icon.GrdInfo.Visibility = Visibility.Visible;
                    break;
                case MessageBoxIcon.Error:
                    icon.GrdError.Visibility = Visibility.Visible;
                    break;
                case MessageBoxIcon.Warning:
                    icon.GrdWarn.Visibility = Visibility.Visible;
                    break;
                case MessageBoxIcon.Question:
                    icon.GrdQuestion.Visibility = Visibility.Visible;
                    break;

            }
        }
    }
}

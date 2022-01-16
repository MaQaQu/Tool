using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace YouiToolkit.Design
{
    public class Loading : Control
    {
        #region Constructor
        static Loading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Loading), new FrameworkPropertyMetadata(typeof(Loading)));
        }
        #endregion

        #region Property
        /// <summary>
        /// 获取或设置加载控件的状态
        /// </summary>
        [DefaultValue(false)]
        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }

        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(Loading));

        /// <summary>
        /// 获取或设置加载控件的基础样式。默认为Standard。
        /// </summary>
        public LoadingStyle LoadingStyle
        {
            get { return (LoadingStyle)GetValue(LoadingStyleProperty); }
            set { SetValue(LoadingStyleProperty, value); }
        }

        public static readonly DependencyProperty LoadingStyleProperty =
            DependencyProperty.Register("LoadingStyle", typeof(LoadingStyle), typeof(Loading), new PropertyMetadata(LoadingStyle.Standard));
        #endregion

        #region Internal Property
        internal double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        internal static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(Loading), new PropertyMetadata(0.0));

        internal double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        internal static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(Loading), new PropertyMetadata(100.0));

        internal double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        internal static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Loading), new PropertyMetadata(0.0));
        #endregion
    }

    public enum LoadingStyle
    {
        Standard,
        Wave,
        Ring,
        Ring2,
        Classic
    }
}

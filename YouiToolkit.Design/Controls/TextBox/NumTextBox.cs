using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YouiToolkit.Design
{
    /// <summary>
    /// 数字输入框
    /// </summary>
    public class NumTextBox : TextBox
    {
        public NumTextBox()
        {
            MinValue = decimal.MinValue;
            MaxValue = decimal.MaxValue;
            PointLenth = 2;
            NumberLength = 13;
        }

        /// <summary>
        /// 输入数据类型
        /// </summary>
        public NumTextBoxType NumType
        {
            get => (NumTextBoxType)GetValue(NumTypeProperty);
            set => SetValue(NumTypeProperty, value);
        }
        public static readonly DependencyProperty NumTypeProperty = DependencyProperty.Register("NumType", typeof(NumTextBoxType), typeof(NumTextBox));

        /// <summary>
        /// 最大值
        /// </summary>
        public decimal MaxValue
        {
            get => (decimal)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(decimal), typeof(NumTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(CheckProperty)));

        /// <summary>
        /// 最小值
        /// </summary>
        public decimal MinValue
        {
            get => (decimal)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(decimal), typeof(NumTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(CheckProperty)));

        /// <summary>
        /// 小数点前的位数
        /// </summary>
        public int NumberLength
        {
            get => (int)GetValue(NumberLengthProperty);
            set => SetValue(NumberLengthProperty, value);
        }
        public static readonly DependencyProperty NumberLengthProperty = DependencyProperty.Register("NumberLength", typeof(int), typeof(NumTextBox));

        /// <summary>
        /// 小数点后位数长度
        /// </summary>
        public int PointLenth
        {
            get => (int)GetValue(PointLenthProperty);
            set => SetValue(PointLenthProperty, value);
        }
        public static readonly DependencyProperty PointLenthProperty = DependencyProperty.Register("PointLenth", typeof(int), typeof(NumTextBox));

        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix
        {
            get => (string)GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register("Suffix", typeof(string), typeof(NumTextBox), new PropertyMetadata(string.Empty));

        private string val = "";
        /// <summary>
        /// 设定最大值最小值依赖属性回调函数
        /// </summary>
        private static void CheckProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumTextBox ntb)
            {
                if (ntb.MaxValue < ntb.MinValue)
                {
                    ntb.MaxValue = ntb.MinValue;
                }
            }
        }

        /// <summary>
        /// 重写KeyDown事件，提供与事件有关的数据，过滤输入数据格式
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

            string txt = Text;
            int ind = CaretIndex;
            if (txt.Contains("."))
            {
                // 控制小数点后输入位数
                if (txt.Split('.')[1].Length >= PointLenth && ind > txt.Split('.')[0].Length && SelectionLength == 0)
                {
                    e.Handled = true;
                    return;
                }
                // 控制小数点前输入位数（有小数点）
                else if (txt.Split('.')[0].Length >= NumberLength && ind <= txt.Split('.')[0].Length)
                {
                    e.Handled = true;
                    return;
                }
            }
            // 控制小数点前输入位数（无小数点）
            else if (txt.Length == NumberLength && e.Key != Key.Decimal && e.Key != Key.OemPeriod)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                val = ".";
            }
            else
            {

                val = "";
            }
            switch (NumType)
            {

                case NumTextBoxType.UInt:
                    //屏蔽非法按键
                    if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key.ToString() == "Tab")
                    {
                        e.Handled = false;
                    }
                    else if (e.Key >= Key.D0 && e.Key <= Key.D9 && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                        if (e.Key.ToString() != "RightCtrl")
                        {
                        }
                    }
                    break;
                case NumTextBoxType.Int:
                    //屏蔽非法按键
                    if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Subtract || e.Key.ToString() == "Tab")
                    {
                        if ((txt.Contains("-") || CaretIndex != 0) && e.Key == Key.Subtract)
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                    }
                    else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemMinus) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        if ((txt.Contains("-") || CaretIndex != 0) && e.Key == Key.OemMinus)
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                        if (e.Key.ToString() != "RightCtrl")
                        { }
                    }
                    break;
                case NumTextBoxType.Decimal:
                    //屏蔽非法按键
                    if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key == Key.Subtract || e.Key.ToString() == "Tab")
                    {
                        if (txt.Contains(".") && e.Key == Key.Decimal)
                        {
                            e.Handled = true;
                            return;
                        }
                        else if ((txt.Contains("-") || CaretIndex != 0) && e.Key == Key.Subtract)
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                    }
                    else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod || e.Key == Key.OemMinus) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        if (txt.Contains(".") && e.Key == Key.OemPeriod)
                        {
                            e.Handled = true;
                            return;
                        }
                        else if ((txt.Contains("-") || CaretIndex != 0) && e.Key == Key.OemMinus)
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                        if (e.Key.ToString() != "RightCtrl")
                        { }
                    }
                    break;
                case NumTextBoxType.UDecimal:
                default:
                    //屏蔽非法按键
                    if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab")
                    {
                        if (txt.Contains(".") && e.Key == Key.Decimal)
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                    }
                    else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        if (txt.Contains(".") && e.Key == Key.OemPeriod)
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                        if (e.Key.ToString() != "RightCtrl")
                        { }
                    }

                    break;
            }
            base.OnKeyDown(e);
        }
        /// <summary>
        ///粘贴内容过滤，设定最大值、最小值，限制小数点输入长度
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            int t1 = Text.Length;
            if (t1 != 0)//用于是否可以将文本空置空
            {
                decimal d = 0;
                if (Text != "-" && Text != "." && Text != "-0" && Text != "-." && Text != "-0." && val != ".")
                {
                    if (decimal.TryParse(Text, out d))
                    {
                        if (NumType == NumTextBoxType.Decimal || NumType == NumTextBoxType.UDecimal)
                        {
                            if (d.ToString().Split('.')[0].Length > NumberLength)//
                            {
                                d = 0;
                            }
                            else
                            {
                                d = Math.Round(d, PointLenth, MidpointRounding.AwayFromZero);
                            }
                        }
                        else
                            if (d.ToString().Split('.')[0].Length > NumberLength)//
                        {
                            d = 0;
                        }
                        else
                        {
                            d = Math.Round(d, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                    if (Math.Abs(t1 - d.ToString().Length) > 0)
                    {
                        Text = d.ToString();
                        CaretIndex = d.ToString().Length;
                    }
                    else
                    {
                        Text = d.ToString();
                    }

                }
                if ((NumType == NumTextBoxType.UDecimal || NumType == NumTextBoxType.UInt) && Text.Contains("-"))
                {
                    Text = Math.Abs(d).ToString();
                }
                if ((NumType == NumTextBoxType.UInt || NumType == NumTextBoxType.Int) && Text.Contains("."))
                {
                    Text = int.Parse(d.ToString()).ToString();
                }
            }
            base.OnTextChanged(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            SelectAll();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            // 失去光标，确定最大值最小值
            if (decimal.TryParse(Text, out decimal d))
            {
                if (d < MinValue)
                {
                    d = MinValue;
                    Text = d.ToString();
                }

                else if (d > MaxValue)
                {
                    d = MaxValue;
                    Text = d.ToString();
                }
            }
            else if (string.IsNullOrEmpty(Text))
            {
                Text = string.Empty;
            }
            else
            {
                Text = d.ToString();
            }
            base.OnLostFocus(e);
        }
    }

    public enum NumTextBoxType
    {
        Decimal,
        UDecimal,
        Int,
        UInt,
    }
}

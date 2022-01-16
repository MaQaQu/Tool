using System.Windows;
using System.Windows.Input;

namespace YouiToolkit.Design
{
    public static class FocusUtils
    {
        public static void SetFocusable(this FrameworkElement element, bool focusable = true)
        {
            element.Focusable = focusable;
            if (focusable)
            {
                element.FocusVisualStyle = null;
                element.MouseLeftButtonDown += (s, e) =>
                {
                    element.SetFocused();
                };
            }
        }

        public static void SetFocused(this FrameworkElement element)
        {
            _ = element.Focus();
            Keyboard.Focus(element);
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(element), element);
        }

        public static FrameworkElement GetFocused(this DependencyObject element)
        {
            return FocusManager.GetFocusedElement(FocusManager.GetFocusScope(element ?? Application.Current.MainWindow)) as FrameworkElement;
        }

        /// <summary>
        /// 实时调试：获取当前焦点控件
        /// <code>
        /// FocusUtils.GetFocusedElement().Name
        /// </code>
        /// </summary>
        /// <param name="element">依赖对象</param>
        /// <returns>焦点控件</returns>
        public static FrameworkElement GetFocusedElement(DependencyObject element = null)
        {
            return FocusManager.GetFocusedElement(FocusManager.GetFocusScope(element ?? Application.Current.MainWindow)) as FrameworkElement;
        }
    }
}

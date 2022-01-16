using System.Windows;
using System.Windows.Input;

namespace YouiToolkit.Design
{
    /// <summary>
    /// 系统事件转路由事件参数
    /// </summary>
    public class MouseButtonProcEventArgs : MouseButtonEventArgs
    {
        public new MouseButton ChangedButton { get; private set; } = (MouseButton)(-1);
        public new int ClickCount { get; private set; } = 1;
        public new MouseButtonState ButtonState { get; private set; } = MouseButtonState.Released;
        public new MouseButtonState LeftButton => ChangedButton == MouseButton.Left ? ButtonState : MouseButtonState.Released;
        public new MouseButtonState RightButton => ChangedButton == MouseButton.Right ? ButtonState : MouseButtonState.Released;
        public new MouseButtonState MiddleButton => ChangedButton == MouseButton.Middle ? ButtonState : MouseButtonState.Released;
        public new MouseButtonState XButton1 => ChangedButton == MouseButton.XButton1 ? ButtonState : MouseButtonState.Released;
        public new MouseButtonState XButton2 => ChangedButton == MouseButton.XButton2 ? ButtonState : MouseButtonState.Released;

        public MouseButtonProcEventArgs(MouseButton button, RoutedEvent @event, int clickCount = 1)
            : this(Mouse.PrimaryDevice, 0, button, @event, clickCount)
        {
        }

        public MouseButtonProcEventArgs(MouseDevice mouse, int timestamp, MouseButton button, RoutedEvent @event, int clickCount = 1)
            : base(mouse, timestamp, button)
        {
            ChangedButton = button;
            ClickCount = clickCount;
            ButtonState = @event.Name.Equals("MouseDown") ? MouseButtonState.Pressed : MouseButtonState.Released;
        }
    }
}

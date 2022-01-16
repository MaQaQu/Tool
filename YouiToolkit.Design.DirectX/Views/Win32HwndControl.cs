using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace YouiToolkit.Design
{
    /// <summary>
    /// DirectX句柄控件
    /// </summary>
    public abstract class Win32HwndControl : HwndHost
    {
        protected private IntPtr Hwnd { get; private set; }
        protected bool HwndInitialized { get; private set; }

        private const string WindowClass = "HwndDirectXWrapper";

        protected Point mousePosition = new Point(0, 0);
        public virtual Point MousePosition => mousePosition;

        public double ActualWidthDpi => DpiUtil.CalcDpi(ActualWidth);
        public double ActualHeightDpi => DpiUtil.CalcDpi(ActualHeight);

        protected Win32HwndControl()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush brush = new SolidColorBrush(Colors.White);
            Pen pen = new Pen(brush, 0);
            drawingContext.DrawRectangle(brush, pen, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();
            HwndInitialized = true;

            Loaded -= OnLoaded;
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Uninitialize();
            HwndInitialized = false;

            Unloaded -= OnUnloaded;

            Dispose();
        }

        protected abstract void Initialize();
        protected abstract void Uninitialize();
        protected abstract void Resized();

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var wndClass = new NativeMethods.WndClassEx();
            wndClass.cbSize = (uint)Marshal.SizeOf(wndClass);
            wndClass.hInstance = NativeMethods.GetModuleHandle(null);
            wndClass.lpfnWndProc = NativeMethods.DefaultWindowProc;
            wndClass.lpszClassName = WindowClass;
            wndClass.style = NativeMethods.CS_DBLCLKS;
            wndClass.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_ARROW);
            NativeMethods.RegisterClassEx(ref wndClass);

            Hwnd = NativeMethods.CreateWindowEx(
                0, WindowClass, "", NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE,
                0, 0, (int)Width, (int)Height, hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, 0);

            return new HandleRef(this, Hwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd.Handle);
            Hwnd = IntPtr.Zero;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateWindowPos();

            base.OnRenderSizeChanged(sizeInfo);

            if (HwndInitialized)
            {
                Resized();
            }
        }

        protected virtual void OnPaint()
        {
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_PAINT:
                    OnPaint();
                    break;
                case NativeMethods.WM_LBUTTONDOWN:
                    RaiseMouseButtonEvent(MouseButton.Left, Mouse.MouseDownEvent);
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    RaiseMouseButtonEvent(MouseButton.Left, Mouse.MouseUpEvent);
                    break;
                case NativeMethods.WM_LBUTTONDBLCLK:
                    RaiseMouseButtonEvent(MouseButton.Left, Mouse.MouseDownEvent, 2);
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    RaiseMouseButtonEvent(MouseButton.Right, Mouse.MouseDownEvent);
                    break;
                case NativeMethods.WM_RBUTTONUP:
                    RaiseMouseButtonEvent(MouseButton.Right, Mouse.MouseUpEvent);
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    RaiseMouseButtonEvent(MouseButton.Right, Mouse.MouseDownEvent, 2);
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    RaiseMouseButtonEvent(MouseButton.Middle, Mouse.MouseDownEvent);
                    break;
                case NativeMethods.WM_MBUTTONUP:
                    RaiseMouseButtonEvent(MouseButton.Middle, Mouse.MouseUpEvent);
                    break;
                case NativeMethods.WM_MBUTTONDBLCLK:
                    RaiseMouseButtonEvent(MouseButton.Middle, Mouse.MouseDownEvent, 2);
                    break;
                case NativeMethods.WM_MOUSEWHEEL:
                    int zDelta = NativeMethods.GET_WHEEL_DELTA_WPARAM(wParam);
                    RaiseMouseWheelEvent(Mouse.MouseWheelEvent, zDelta);
                    break;
                case NativeMethods.WM_MOUSEMOVE:
                    mousePosition = new Point(NativeMethods.GET_X_LPARAM(lParam), NativeMethods.GET_Y_LPARAM(lParam));
                    RaiseMouseEvent(Mouse.MouseMoveEvent);
                    ActiveNativeMouseEvent();
                    break;
                case NativeMethods.WM_MOUSELEAVE:
                    RaiseMouseEvent(Mouse.MouseLeaveEvent);
                    break;
                case NativeMethods.WM_MOUSEHOVER:
                    break;
            }
            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        private void ActiveNativeMouseEvent()
        {
            var tme = new NativeMethods.TRACKMOUSEEVENTS();
            tme.cbSize = (uint)Marshal.SizeOf(tme);
            tme.dwFlags = (uint)(NativeMethods.TrackerEventFlags.TME_LEAVE | NativeMethods.TrackerEventFlags.TME_HOVER);
            tme.hWnd = Handle;
            tme.dwHoverTime = 10;
            NativeMethods.TrackMouseEvent(ref tme);
        }

        protected virtual void RaiseMouseButtonEvent(MouseButton button, RoutedEvent e, int clickCount = 1)
        {
            RaiseEvent(new MouseButtonProcEventArgs(button, e, clickCount)
            {
                RoutedEvent = e,
                Source = this,
            });
        }

        protected virtual void RaiseMouseWheelEvent(RoutedEvent e, int delta)
        {
            RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, 0, delta)
            {
                RoutedEvent = e,
                Source = this,
            });
        }

        protected virtual void RaiseMouseEvent(RoutedEvent e)
        {
            RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
            {
                RoutedEvent = e,
                Source = this,
            });
        }
    }
}

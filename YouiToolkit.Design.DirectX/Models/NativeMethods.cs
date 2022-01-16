using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Design
{
    internal class NativeMethods
    {
        public const int CS_VREDRAW = 0x0001;
        public const int CS_HREDRAW = 0x0002;
        public const int CS_DBLCLKS = 0x0008;
        public const int CS_OWNDC = 0x0020;
        public const int CS_CLASSDC = 0x0040;
        public const int CS_PARENTDC = 0x0080;
        public const int CS_NOCLOSE = 0x0200;
        public const int CS_SAVEBITS = 0x0800;
        public const int CS_BYTEALIGNCLIENT = 0x1000;
        public const int CS_BYTEALIGNWINDOW = 0x2000;
        public const int CS_GLOBALCLASS = 0x4000;
        public const int CS_IME = 0x00010000;
        public const int CS_DROPSHADOW = 0x00020000;

        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;

        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;

        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSELAST = 0x020A;
        public const int WM_MOUSEHOVER = 0x02A1;
        public const int WM_MOUSELEAVE = 0x02A3;

        public const int WM_PAINT = 0x000F;

        public const int IDC_ARROW = 32512;

        [StructLayout(LayoutKind.Sequential)]
        public struct WndClassEx
        {
            public uint cbSize;
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static readonly WndProc DefaultWindowProc = DefWindowProc;

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(
            int exStyle,
            string className,
            string windowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string module);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx([In] ref WndClassEx lpwcx);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);



        internal static ushort HIWORD(IntPtr value) => (ushort)((((long)value) >> 0x10) & 0xFFFF);
        internal static ushort HIWORD(uint value) => (ushort)(value >> 0x10);

        public static ushort LOWORD(uint value) => (ushort)(value & 0xFFFF);
        public static ushort LOWORD(IntPtr value) => (ushort)((long)value & 0xFFFF);

        public static byte LOWBYTE(ushort value) => (byte)(value & 0xFF);
        public static byte HIGHBYTE(ushort value) => (byte)(value >> 8);

        internal static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam) => (short)HIWORD(wParam);
        internal static int GET_WHEEL_DELTA_WPARAM(uint wParam) => (short)HIWORD(wParam);

        /// <summary>
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        /// <remark>#define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))</remark>
        public static int GET_X_LPARAM(IntPtr wParam) => LOWORD(wParam);


        //#define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))
        public static int GET_Y_LPARAM(IntPtr wParam) => HIWORD(wParam);

        [StructLayout(LayoutKind.Sequential)]
        internal struct TRACKMOUSEEVENTS
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hWnd;
            public uint dwHoverTime;
        }
        internal enum TrackerEventFlags : uint
        {
            TME_HOVER = 0x00000001,
            TME_LEAVE = 0x00000002,
            TME_QUERY = 0x40000000,
            TME_CANCEL = 0x80000000
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);
    }
}

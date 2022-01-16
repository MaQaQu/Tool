using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace YouiToolkit.Design
{
    /// <summary>
    /// 控件帮助类
    /// </summary>
    public static class Win32Helper
    {
        /// <summary>
        /// 投递消息
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr handle, int msg, uint wParam, uint lParam);

        /// <summary>
        /// 发送消息
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// 控件更新（发送WM_PAINT）
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int UpdateWindow(IntPtr hWnd);

        /// <summary>
        /// 控件动画
        /// </summary>
        /// <param name="hwnd">控件句柄</param>
        /// <param name="dwTime">持续时间</param>
        /// <param name="dwFlags">动画效果<see cref="EAnimateWindow"/></param>
        /// <returns>结果</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

        /// <summary>
        /// 寻找窗体句柄
        /// </summary>
        /// <param name="lpClassName">类名</param>
        /// <param name="lpWindowName">标题</param>
        /// <returns>窗体句柄</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 闪烁窗体
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="nCmdShow">显示命令</param>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 遍历子窗体
        /// </summary>
        /// <param name="hWndParent">父窗体</param>
        /// <param name="lpfn">回调方法</param>
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hWndParent, EnumChildWindowsCallBack lpfn, int lParam);

        /// <summary>
        /// 遍历子窗体回调方法
        /// </summary>
        public delegate bool EnumChildWindowsCallBack(IntPtr hwnd, int lParam);

        /// <summary>
        /// 获取窗体名称长度
        /// </summary>
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// 获取窗体名称
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);

        /// <summary>
        /// 获取系统度量值
        /// </summary>
        /// <param name="nIndex">标识符</param>
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        /// <summary>
        /// 加载外部库
        /// </summary>
        /// <param name="lpFileName">库名</param>
        /// <returns>库句柄</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// 添加临时系统字体资源
        /// </summary>
        /// <param name="lpFileName">字体路劲</param>
        /// <returns>添加结果</returns>
        [DllImport("gdi32.dll", EntryPoint = "AddFontResource", CharSet = CharSet.Auto)]
        public static extern int AddFontResource(string lpFileName);

        /// <summary>
        /// 添加临时系统字体资源
        /// </summary>
        /// <param name="lpFileName">字体路劲</param>
        /// <returns>添加结果</returns>
        [DllImport("gdi32.dll")]
        public static extern int AddFontResourceEx(string lpFileName, int fl, IntPtr res);

        /// <summary>
        /// 添加临时系统字体资源
        /// </summary>
        /// <param name="lpFileName">字体路劲</param>
        /// <returns>添加结果</returns>
        public static int AddFontResourceEx(string lpFileName, bool isPrivate = true)
        {
            int FR_PRIVATE = 0x10, FR_NOT_ENUM = 0x20;

            return AddFontResourceEx(lpFileName, isPrivate ? FR_PRIVATE : FR_NOT_ENUM, IntPtr.Zero);
        }

        /// <summary>
        /// 移除临时系统字体资源
        /// </summary>
        /// <param name="lpFileName">字体路劲</param>
        /// <returns>移除结果</returns>
        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResource", CharSet = CharSet.Auto)]
        public static extern bool RemoveFontResource(string lpFileName);

        /// <summary>
        /// 移除临时系统字体资源
        /// </summary>
        /// <param name="lpFileName">字体路劲</param>
        /// <returns>移除结果</returns>
        [DllImport("gdi32.dll")]
        public static extern bool RemoveFontResourceEx(string lpFileName);

        /// <summary>
        /// 设定指定控件的属性
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="nIndex">属性偏移</param>
        /// <param name="wndproc">属性新值</param>
        /// <returns>结果</returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);

        /// <summary>
        /// 获取指定控件的属性
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="nIndex">属性偏移</param>
        /// <param name="wndproc">属性新值</param>
        /// <returns>结果</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// 获得与调用线程的消息队列相关的活动窗口的窗口句柄
        /// </summary>
        /// <returns>窗口句柄</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        /// <summary>
        /// 对指定的源设备环境区域中的像素进行位块转换
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

        /// <summary>
        /// 删除指定的设备上下文环境
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        /// <summary>
        /// 删除绘制有关系统资源
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        /// <summary>
        /// 创建与指定的设备环境相关的设备兼容的位图
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        /// <summary>
        /// 创建一个与应用程序的当前显示器兼容的系统资源
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        /// <summary>
        /// 选择一对象到指定的设备上下文环境中
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

        /// <summary>
        /// 设置字间距
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern int SetTextCharacterExtra(IntPtr hdc, int nCharExtra);

        /// <summary>
        /// 返回桌面窗口的句柄
        /// </summary>
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 获取指定句柄窗口的设备环境
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);

        /// <summary>
        /// 截图
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        /// <summary>
        /// 删除指定的设备上下文环境
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        /// <summary>
        /// 删除指定的设备上下文环境
        /// </summary>
        [DllImport("user32.dll")]
        public static extern uint ReleaseDC(uint hwnd, uint hdc);

        /// <summary>
        /// 显示文本插入标记
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);

        /// <summary>
        /// 隐藏文本插入标记
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

        /// <summary>
        /// 获取文本插入标记位置
        /// </summary>
        /// <param name="p">位置</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool GetCaretPos(out Point p);

        /// <summary>
        /// 获取指定点的窗口的句柄
        /// </summary>
        [DllImport("user32.dll")]
        public static extern uint WindowFromPoint(int x, int y);

        /// <summary>
        /// 指定窗口从一个设备上下文中提取一个句柄
        /// </summary>
        [DllImport("user32.dll")]
        public static extern uint GetDC(uint hwnd);

        /// <summary>
        /// 检索指定坐标点的像素的RGB颜色值
        /// </summary>
        [DllImport("gdi32.dll")]
        public static extern int GetPixel(uint hDC, int x, int y);

        /// <summary>
        /// 安装钩子
        /// </summary>
        public delegate int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, MouseHookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// 卸载钩子
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// 调用下一个钩子
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 显示滚动条
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        /// <summary>
        /// 写入注册表
        /// </summary>
        /// <param name="lpszSection">节</param>
        /// <param name="lpszKeyName">健</param>
        /// <param name="lpszString">值</param>
        /// <returns>操作结果</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        /// <summary>
        /// 取最后错误
        /// </summary>
        /// <returns>错误代码</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        /// <summary>
        /// 检测是否为设计模式
        /// </summary>
        /// <returns>是否为设计模式</returns>
        public static bool IsDesignMode()
        {
            bool returnFlag = false;

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
            return returnFlag;
        }
    }
}

using System.Windows;

namespace YouiToolkit.Design
{
    public static class DebugUtil
    {
        public static bool IsDebug =>
#if DEBUG
            true;
#else
            false;
#endif
        public static Visibility VisibleOnDebug => IsDebug ? Visibility.Visible : Visibility.Collapsed;
    }
}

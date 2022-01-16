using System.Windows;

namespace YouiToolkit.Design
{
    public static class Toast
    {
        public static void Information(FrameworkElement owner, string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
            => Show(owner, message, new ToastConfig(MessageBoxIcon.Info, location, offsetMargin, time));

        public static void Success(FrameworkElement owner, string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
            => Show(owner, message, new ToastConfig(MessageBoxIcon.Success, location, offsetMargin, time));

        public static void Error(FrameworkElement owner, string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
            => Show(owner, message, new ToastConfig(MessageBoxIcon.Error, location, offsetMargin, time));

        public static void Warning(FrameworkElement owner, string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
            => Show(owner, message, new ToastConfig(MessageBoxIcon.Warning, location, offsetMargin, time));

        public static void Question(FrameworkElement owner, string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
            => Show(owner, message, new ToastConfig(MessageBoxIcon.Question, location, offsetMargin, time));

        public static void Show(FrameworkElement owner, string message, ToastConfig options = null)
        {
            var toast = new ToastControl(owner, message, options);
            toast.ShowCore();
        }
    }
}

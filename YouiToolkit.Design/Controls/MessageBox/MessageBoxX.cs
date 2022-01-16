using System;
using System.Collections.Generic;
using System.Windows;

namespace YouiToolkit.Design
{
    public static class MessageBoxX
    {
        public static IDictionary<string, MessageBoxXConfig> Config { get; }

        static MessageBoxX()
        {
            Config = new Dictionary<string, MessageBoxXConfig>();
        }

        public static MessageBoxResult Information(DependencyObject owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Info), customized);

        public static MessageBoxResult Information(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(null, message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Info), customized);

        public static MessageBoxResult Success(DependencyObject owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Success), customized);

        public static MessageBoxResult Success(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(null, message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Success), customized);

        public static MessageBoxResult Warning(DependencyObject owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Warning), customized);

        public static MessageBoxResult Warning(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(null, message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Warning), customized);

        public static MessageBoxResult Error(DependencyObject owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Error), customized);

        public static MessageBoxResult Error(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, Action<object> customized = null)
            => ShowCore(null, message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Error), customized);

        public static MessageBoxResult Question(DependencyObject owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.YesNo, Action<object> customized = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Question), customized);

        public static MessageBoxResult Question(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.YesNo, Action<object> customized = null)
            => ShowCore(null, message, title, messageBoxButton, new MessageBoxXConfig(MessageBoxIcon.Question), customized);

        public static MessageBoxResult Show(DependencyObject owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxXConfig config = null, Action<object> customized = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, messageBoxButton, config, customized);

        public static MessageBoxResult Show(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxXConfig config = null, Action<object> customized = null)
            => ShowCore(null, message, title, messageBoxButton, config, customized);

        private static MessageBoxResult ShowCore(Window owner, string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxXConfig config = null, Action<object> customized = null)
        {
            var msb = new MessageBoxXDialog(owner, message, title, messageBoxButton, config ??= new MessageBoxXConfig());

            WindowX windowX = null;

            if (config.InteractOwnerMask && owner != null && owner is WindowX)
            {
                windowX = owner as WindowX;
            }

            if (windowX != null)
            {
                windowX.IsMaskVisible = true;
            }
            msb.Owner = owner;
            customized?.Invoke(msb);
            msb.ShowDialog();
            if (windowX != null)
            {
                windowX.IsMaskVisible = false;
            }
            return msb.MessageBoxResult;
        }
    }
}

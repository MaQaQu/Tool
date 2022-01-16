using System;
using System.Collections.Generic;
using System.Windows;

namespace YouiToolkit.Design
{
    public static class PendingBox
    {
        public static IPendingHandler Show(string message, string title = null, bool cancelable = false, PendingBoxConfig config = null)
            => Show(null, message, title, cancelable, config);

        public static IPendingHandler Show(DependencyObject owner, string message, string title = null, bool cancelable = false, PendingBoxConfig configurations = null)
            => ShowCore(owner is Window ? owner as Window : owner?.FindWindow(), message, title, cancelable, configurations ??= new PendingBoxConfig());

        private static IPendingHandler ShowCore(Window owner, string message, string title, bool cancelable, PendingBoxConfig config)
        {
            var msb = new PendingBoxDialog(owner, message, title, cancelable, config);
            var handler = new PendingHandler(msb);

            handler.CloseOnCanceled = config.CloseOnCanceled;

            WindowX windowX = null;

            if (config.InteractOwnerMask && owner != null && owner is WindowX)
                windowX = owner as WindowX;

            if (windowX != null)
                windowX.IsMaskVisible = true;
            msb.Closed += (s, e) =>
            {
                if (windowX != null)
                    windowX.IsMaskVisible = false;
                handler.RaiseClosedEvent(s, e);
            };
            msb.Canceled += (s, e) =>
            {
                handler.RaiseCanceledEvent(s, e);
            };
            msb.Show();

            return handler;
        }
    }
}

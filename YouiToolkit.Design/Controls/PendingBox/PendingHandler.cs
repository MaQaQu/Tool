using System;

namespace YouiToolkit.Design
{
    public class PendingHandler : IPendingHandler
    {
        public PendingBoxDialog PendingBoxDialog { get; private set; }

        public event EventHandler Closed;
        public event EventHandler Cancel;

        public string Message
        {
            get => DispatcherHelper.Invoke(() => PendingBoxDialog.Message);
            set => DispatcherHelper.Invoke(() => PendingBoxDialog.Message = value);
        }

        public bool Cancelable
        {
            get => DispatcherHelper.Invoke(() => PendingBoxDialog.Cancelable);
            set => DispatcherHelper.Invoke(() => PendingBoxDialog.Cancelable = value);
        }

        public bool canceled = false;
        public bool Canceled => canceled;

        public bool CloseOnCanceled { get; set; } = false;

        public PendingHandler(PendingBoxDialog pendingBoxDialog)
        {
            PendingBoxDialog = pendingBoxDialog;
        }

        public void Close()
            => PendingBoxDialog?.ForceClose();

        public void RaiseClosedEvent(object sender, EventArgs e)
            => Closed?.Invoke(sender, e);

        public void RaiseCanceledEvent(object sender, EventArgs e)
        {
            Cancel?.Invoke(sender, e);
            if (CloseOnCanceled)
            {
                Close();
            }
            canceled = true;
        }
    }
}

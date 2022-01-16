using System;

namespace YouiToolkit.Design
{
    public abstract class CancelableEventArgs : EventArgs
    {
        public bool Canceled { get; set; } = false;
    }
}

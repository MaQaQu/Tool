using System;

namespace YouiToolkit.Design
{
    public abstract class HandlabledEventArgs : EventArgs
    {
        public bool Handled { get; set; } = false;
    }
}

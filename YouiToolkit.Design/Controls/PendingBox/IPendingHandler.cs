using System;

namespace YouiToolkit.Design
{
    public interface IPendingHandler
    {
        public event EventHandler Closed;
        public event EventHandler Cancel;

        public string Message { get; set; }
        public bool Cancelable { get; set; }
        public bool Canceled { get; }

        public void Close();
    }
}

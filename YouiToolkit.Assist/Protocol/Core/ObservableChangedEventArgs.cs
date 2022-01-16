using System;
using System.ComponentModel;

namespace YouiToolkit.Assist
{
    public class ObservableChangedEventArgs : PropertyChangedEventArgs
    {
        public object OldValue { get; set; } = default;
        public object NewValue { get; set; } = default;
        public DateTime TimeStamp { get; set; } = default;

        public ObservableChangedEventArgs(string propertyName)
            : base(propertyName)
        {
        }

        public ObservableChangedEventArgs(string propertyName, object oldValue, object newValue, DateTime timeStamp = default)
            : this(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
            TimeStamp = timeStamp;
        }
    }
}

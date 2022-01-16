using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YouiToolkit.Design.DirectX
{
    public class DirectXPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual bool OnPropertyChanging(object oldValue, object newValue, [CallerMemberName] string propertyName = null) => true;
    }
}

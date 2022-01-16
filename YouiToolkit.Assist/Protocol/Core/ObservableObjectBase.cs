using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace YouiToolkit.Assist
{
    public class ObservableObjectBase
    {
        public event EventHandler<ObservableChangedEventArgs> PropertyChanged;
        public virtual bool Async => false;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null, bool? async = null)
        {
            if (async ?? Async)
            {
#if false
                PropertyChanged?.BeginInvoke(this, new ObservableChangedEventArgs(propertyName), null, null);
#else
                Task.Run(() => PropertyChanged?.BeginInvoke(this, new ObservableChangedEventArgs(propertyName) { TimeStamp = DateTime.Now }, null, null));
#endif
            }
            else
            {
                PropertyChanged?.Invoke(this, new ObservableChangedEventArgs(propertyName) { TimeStamp = DateTime.Now });
            }
        }

        protected virtual void RaisePropertyChanged(object oldValue, object newValue, [CallerMemberName] string propertyName = null, bool? async = null)
        {
            if (async ?? Async)
            {
#if false
                PropertyChanged?.BeginInvoke(this, new ObservableChangedEventArgs(propertyName, oldValue, newValue), null, null);
#else
                Task.Run(() => PropertyChanged?.Invoke(this, new ObservableChangedEventArgs(propertyName, oldValue, newValue, DateTime.Now)));
#endif
            }
            else
            {
                PropertyChanged?.Invoke(this, new ObservableChangedEventArgs(propertyName, oldValue, newValue, DateTime.Now));
            }
        }

        protected bool Set<T>(ref T field, T newValue = default, [CallerMemberName] string propertyName = null, bool notifyMust = false)
        {
            if (!notifyMust && EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            //VerifyPropertyName(propertyName);

            var oldValue = field;
            field = newValue;

            RaisePropertyChanged(oldValue, field, propertyName);

            return true;
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            var myType = GetType();

#if NETFX_CORE
            var info = myType.GetTypeInfo();

            if (!string.IsNullOrEmpty(propertyName)
                && info.GetDeclaredProperty(propertyName) == null)
            {
                // Check base types
                var found = false;

                while (info.BaseType != typeof(Object))
                {
                    info = info.BaseType.GetTypeInfo();

                    if (info.GetDeclaredProperty(propertyName) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new ArgumentException("Property not found", propertyName);
                }
            }
#else
            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetProperty(propertyName) == null)
            {
#if !SILVERLIGHT
                var descriptor = this as ICustomTypeDescriptor;

                if (descriptor != null)
                {
                    if (descriptor.GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName))
                    {
                        return;
                    }
                }
#endif

                throw new ArgumentException("Property not found", propertyName);
            }
#endif
        }
    }
}

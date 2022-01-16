using System;
using System.Windows.Input;

namespace YouiToolkit.Design
{
    public class BaseCommand : ICommand
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        public virtual Action<object>? Action { get; set; }

        public bool canExecute = true;
        public virtual bool CanExecuted
        {
            get => canExecute;
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public BaseCommand(Action<object>? action)
        {
            Action = action;
        }

        public BaseCommand()
        {
        }

        public virtual bool CanExecute(object parameter) => true;

        public virtual void Execute(object parameter) => Action?.Invoke(parameter);
    }
}

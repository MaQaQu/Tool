using System;
using System.Windows;
using System.Windows.Input;

namespace YouiToolkit.Design
{
    public partial class WindowDialog : Window
    {
        public event EventHandler<WindowDialogResultEventArgs> ResultCommandExecuted;

        public BaseCommand ResultCommand => new BaseCommand()
        {
            Action = (o) => OnResultCommandExecuted(o),
        };

        protected WindowDialogResult result = WindowDialogResult.None;
        public WindowDialogResult Result
        {
            get => result;
            internal set
            {
                result = value;
                Close();
            }
        }

        protected FrameworkElement control = null;
        public FrameworkElement Control
        {
            get => control;
            set
            {
                control = value;
                if (control != null)
                {
                    contentPresenter.Content = value;
                }
            }
        }

        public WindowDialog()
        {
            InitializeComponent();

            border.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            };

            KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                    case Key.Space:
                        //ResultCommand.Execute(WindowDialogResult.OK);
                        //ResultCommand.Execute(WindowDialogResult.Yes);
                        break;
                    case Key.Escape:
                        //ResultCommand.Execute(WindowDialogResult.Cancel);
                        //ResultCommand.Execute(WindowDialogResult.No);
                        break;
                }
            };
        }

        public WindowDialogResult ShowDialog(DependencyObject d = null)
            => ShowDialog(d is Window ? d as Window : Window.GetWindow(d));

        public WindowDialogResult ShowDialog(Window owner)
        {
            WindowX windowX = null;

            if (owner != null && owner is WindowX)
            {
                windowX = owner as WindowX;
            }

            if (windowX != null)
            {
                windowX.IsMaskVisible = true;
            }
            Owner = owner;
            base.ShowDialog();
            if (windowX != null)
            {
                windowX.IsMaskVisible = false;
            }
            return Result;
        }

        public virtual void OnResultCommandExecuted(object? o)
        {
            WindowDialogResultEventArgs e = null;

            if (ResultCommandExecuted != null)
            {
                e = new WindowDialogResultEventArgs(o);
                ResultCommandExecuted?.Invoke(this, e);
            }
            if (e != null && e.Handled)
            {
                return;
            }
            if (o is WindowDialogResult result)
            {
                Result = result;
            }
        }

        public static WindowDialog Create<T>() where T : WindowDialogControl
        {
            var dialog = new WindowDialog()
            {
                Control = Activator.CreateInstance(typeof(T)) as T,
            };
            if (dialog.Control is IWindowDialogControl control)
            {
                control.Owner = dialog;
                dialog.Title = control.Title;
            }
            return dialog;
        }
    }
}

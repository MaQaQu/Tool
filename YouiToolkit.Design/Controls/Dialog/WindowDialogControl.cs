using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YouiToolkit.Design
{
    public class WindowDialogControl : UserControl, IWindowDialogControl
    {
        protected WindowDialog owner = null;
        public WindowDialog Owner
        {
            get => owner;
            set
            {
                if (value != null && owner != value)
                {
                    owner = value;
                    owner.ResultCommandExecuted -= ResultCommandExecuted;
                    owner.ResultCommandExecuted += ResultCommandExecuted;
                }
            }
        }

        public string Title { get; set; } = string.Empty;

        protected virtual void ResultCommandExecuted(object sender, WindowDialogResultEventArgs e)
        {
        }
    }
}

using System.Windows.Controls;
using System.Windows.Input;
using YouiToolkit.Design;

namespace YouiToolkit.Views
{
    public partial class DialogControlMapNew : WindowDialogControl
    {
        public string MapName => textBoxMapName.Text;

        public DialogControlMapNew()
        {
            InitializeComponent();

            textBoxMapName.MouseMove += (s, e) =>
            {
                e.Handled = true;
            };
        }

        protected override void ResultCommandExecuted(object sender, WindowDialogResultEventArgs e)
        {
            if (e.DialogResult == WindowDialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(textBoxMapName.Text))
                {
                    MessageBoxX.Warning(this, "请输入地图名称", "提示");
                    e.Handled = true;
                }
            }
        }
    }
}

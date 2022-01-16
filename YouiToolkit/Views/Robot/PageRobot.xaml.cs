using System.Windows.Controls;
using YouiToolkit.Ctrls;

namespace YouiToolkit.Views
{
    public partial class PageRobot : UserControl
    {
        public PageRobot()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                buttonConnect.Click += (s, e) =>
                {
                    AssistManager.Assist.SetIP(textBoxConnect.Text);
                };
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YouiToolkit.Design
{
    public partial class DataGridButtonGroupColumnButton : Button
    {
        public string TextIcon
        {
            get { return (string)GetValue(TextIconProperty); }
            set { SetValue(TextIconProperty, value); }
        }

        public static readonly DependencyProperty TextIconProperty =
            DependencyProperty.Register("TextIcon", typeof(string), typeof(DataGridButtonGroupColumnButton), new PropertyMetadata("&#xe9ed;"));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DataGridButtonGroupColumnButton), new PropertyMetadata("NEW"));

        public DataGridButtonGroupColumnButton()
        {
            InitializeComponent();
        }
    }
}

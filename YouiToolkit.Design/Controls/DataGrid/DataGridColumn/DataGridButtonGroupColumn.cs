using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public class DataGridButtonGroupColumn : DataGridBoundColumn
    {
        public static event EventHandler<DataGridButtonGroupColumnButton> LoadingColumn;

        /// <summary>
        /// 分隔符
        /// </summary>
        public static char SplitComma = '`';

        public new DataGrid DataGridOwner => base.DataGridOwner;

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return GenerateButtonGroup(cell, dataItem);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return GenerateButtonGroup(cell, dataItem);
        }

        private StackPanel GenerateButtonGroup(DataGridCell cell, object dataItem)
        {
            StackPanel stackPanel = (cell != null) ? (cell.Content as StackPanel) : null;
            if (stackPanel == null)
            {
                stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
            }

            string buttonNames = null;
            DataGridColumnAttribute attr = null;
            DataGridButtonGroupColumnStyle style = null;

            try
            {
                dynamic dataItemDyn = dataItem as dynamic;
                buttonNames = dataItemDyn![cell.TabIndex];
                string name = TableModelHelper.GetNames(dataItem.GetType()).GetValue(cell.TabIndex) as string;
                attr = dataItem.GetType().GetProperty(name).GetCustomAttribute<DataGridColumnAttribute>();

                if (attr != null && attr.RenderStyleType != null)
                {
                    style = Activator.CreateInstance(attr.RenderStyleType) as DataGridButtonGroupColumnStyle;
                }
            }
            catch
            {
            }

            if (buttonNames != null)
            {
                string[] s = buttonNames.Split(SplitComma);
                for (int i = default; i < s.Length; i++)
                {
                    string buttonName = s[i];
                    if (buttonName.IsNullOrEmpty())
                    {
                        continue;
                    }
                    DataGridButtonGroupColumnButton button = new DataGridButtonGroupColumnButton();

                    if (style != null)
                    {
                        style.ApplyStyle(button, i);
                    }
                    //button.FontFamily = button.FindResource("IcoMoon") as FontFamily;
                    if (button.Content is TextBlock textBlockContent)
                    {
                        string text = IconFontHelper.GetSymbol(buttonName);
                        string[] texts = text.Split(' ');

                        if (texts.Length >= 2)
                        {
                            button.TextIcon = texts[0];
                            button.Text = texts[1];
                        }
                        else
                        {
                            button.Content = IconFontHelper.GetSymbol(buttonName);
                        }
                    }
                    else
                    {
                        button.Content = IconFontHelper.GetSymbol(buttonName);
                    }
                    stackPanel.Children.Add(button);
                    button.Tag = i;
                    button.TabIndex = i;
                    LoadingColumn?.Invoke(this, button);
                }
            }
            return stackPanel;
        }
    }
}

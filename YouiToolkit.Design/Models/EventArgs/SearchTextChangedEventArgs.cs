using System.Windows;

namespace YouiToolkit.Design
{
    public class SearchTextChangedEventArgs : RoutedEventArgs
    {
        public string Text { get; set; }

        public SearchTextChangedEventArgs(string text, RoutedEvent routedEvent) : base(routedEvent)
        {
            Text = text;
        }
    }

    public delegate void SearchTextChangedEventHandler(object sender, SearchTextChangedEventArgs e);
}

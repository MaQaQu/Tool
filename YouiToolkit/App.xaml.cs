using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using YouiToolkit.Logging;

namespace YouiToolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Exit += (s, e) =>
            {
            };

            DispatcherUnhandledException += (object s, DispatcherUnhandledExceptionEventArgs e) =>
            {
                Logger.Fatal("DispatcherUnhandledException", e.ToString());
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Logger.Fatal("UnhandledException", e.ToString());
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Logger.Fatal("UnobservedTaskException", e.ToString());
            };
        }
    }
}

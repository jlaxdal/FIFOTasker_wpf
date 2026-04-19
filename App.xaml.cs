using System;
using System.Threading;
using System.Windows;

namespace FIFOTasker_wpf
{
    public partial class App : System.Windows.Application
    {
        private static Mutex? _mutex;
        private const string MutexName = "Global\\FIFOTasker_SingleInstanceMutex";

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            _mutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                System.Windows.MessageBox.Show("FIFOTasker is already running.",
                                "FIFOTasker",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                _mutex?.Dispose();
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // Create and show the main window manually
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
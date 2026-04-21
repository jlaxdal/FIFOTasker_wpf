using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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

            // We initialize the database here to ensure it's ready before the main window is shown
            InitializeDatabase();

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

        private void InitializeDatabase()
        {
            try
            {
                using var context = new AppDbContext();

                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FIFOTasker.db");

                if (!File.Exists(dbPath) || new FileInfo(dbPath).Length < 8192)   // small file = incomplete
                {
                    context.Database.EnsureDeleted();     // safe on first run (empty db)
                    context.Database.Migrate();           // applies migrations properly
                }
                else
                {
                    context.Database.Migrate();           // apply any pending migrations
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to initialize database:\n{ex.Message}",
                                "Database Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
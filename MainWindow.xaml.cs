using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;        // for NotifyIcon and ContextMenuStrip
using System.Drawing;              // for Icon
using System.IO;                   // for Path

namespace FIFOTasker_wpf
{
    public partial class MainWindow : Window
    {
        private readonly TaskService _taskService;
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        //private readonly AutoStartService _autoStartService = new();
        private readonly AutoStartService _autoStartService = new AutoStartService();
        private readonly ConfigService _configService = new ConfigService();

        public MainWindow()
        {
            InitializeComponent();

            _taskService = new TaskService();

            // Make window draggable by clicking anywhere on the border
            MouseDown += MainWindow_MouseDown;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            HandleFirstRun();
            SetupTrayIcon();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.RemoveWindowShadow(this);
            WindowHelper.PositionMainWindow(this);

            Topmost = true;
            ShowInTaskbar = false;

            RefreshMainWindow();
        }

        private void HandleFirstRun()
        {
            if (_configService.IsFirstRun())
            {
                var result = System.Windows.MessageBox.Show(
                    "Would you like FIFOTasker to start automatically when you log in?",
                    "First Run - Auto Start",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                _autoStartService.SetAutoStart(result == MessageBoxResult.Yes);
                _configService.MarkFirstRunCompleted();
            }
        }

        private void SetupTrayIcon()
        {
            string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FIFOTasker.ico");

            _notifyIcon = new NotifyIcon
            {
                Icon = File.Exists(icoPath)
            ? new System.Drawing.Icon(icoPath)
            : System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Text = "FIFOTasker",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("All Tasks", null, Tray_AllTasks_Click);
            contextMenu.Items.Add("Options", null, Tray_Options_Click);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit Program", null, Tray_Exit_Click);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        // Tray handlers
        private void Tray_AllTasks_Click(object? sender, EventArgs e)
        {
            var allWindow = new AllTasksWindow(_taskService);
            allWindow.ShowDialog();
        }

        private void Tray_Options_Click(object? sender, EventArgs e)
        {
            var optionsWindow = new OptionsWindow(_autoStartService);
            optionsWindow.ShowDialog();
        }

        private void Tray_Exit_Click(object? sender, EventArgs e)
        {
            _notifyIcon?.Dispose();
            _taskService.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // Prevent closing via X button - force use of tray Exit
            e.Cancel = true;
            Hide();   // just hide main window instead of closing
        }

        public void RefreshMainWindow()
        {
            var activeTask = _taskService.GetActiveTask();

            if (activeTask != null)
            {
                txtActiveTask.Text = activeTask.Summary.Length > 70
                    ? activeTask.Summary.Substring(0, 67) + "..."
                    : activeTask.Summary;

                btnFinish.IsEnabled = true;
            }
            else
            {
                txtActiveTask.Text = "No active task";
                btnFinish.IsEnabled = false;
            }

            txtCount.Text = _taskService.GetUnfinishedCount().ToString();
        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            var active = _taskService.GetActiveTask();
            if (active == null) return;

            var confirm = new ConfirmFinishWindow(active.Summary);
            confirm.Owner = this;
            confirm.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (confirm.ShowDialog() == true)
            {
                _taskService.FinishCurrentTask();
                RefreshMainWindow();
            }
        }

        private void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            var listWindow = new TaskListWindow(_taskService);
            listWindow.Owner = this;
            listWindow.ShowDialog();
            RefreshMainWindow();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newTaskWindow = new NewTaskWindow(_taskService);
            newTaskWindow.Owner = this;
            newTaskWindow.ShowDialog();
            RefreshMainWindow();
        }
    }
}
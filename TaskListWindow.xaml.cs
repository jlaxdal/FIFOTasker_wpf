using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;   // for INotifyPropertyChanged if needed later

namespace FIFOTasker_wpf
{
    public partial class TaskListWindow : Window
    {
        private readonly TaskService _taskService;

        public TaskListWindow(TaskService taskService)
        {
            InitializeComponent();
            _taskService = taskService;
            Owner = System.Windows.Application.Current.MainWindow;
            PositionAboveMainWindow();
            LoadTasks();
        }

        private void PositionAboveMainWindow()
        {
            if (Owner is MainWindow main && !double.IsNaN(main.Left))
            {
                Left = main.Left;
                Top = main.Top - Height - 8;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        private void LoadTasks()
        {
            var unfinished = _taskService.GetUnfinishedTasks(50);

            // Add a helper property for binding
            foreach (var task in unfinished)
            {
                task.IsNotActive = task.Id != _taskService.GetActiveTask()?.Id;
            }

            lvTasks.ItemsSource = unfinished;

            var active = _taskService.GetActiveTask();
            txtActiveDetails.Text = active?.DetailedDescription ?? "No detailed description.";
        }

        private void Escalate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is int taskId)
            {
                var active = _taskService.GetActiveTask();
                if (active?.Id == taskId) return;

                _taskService.EscalateTask(taskId);
                LoadTasks();
                ((MainWindow)Owner).RefreshMainWindow();
            }
        }

        private void LvTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Not required
        }
    }
}
using System.Windows;

namespace FIFOTasker_wpf
{
    public partial class NewTaskWindow : Window
    {
        private readonly TaskService _taskService;

        public NewTaskWindow(TaskService taskService)
        {
            InitializeComponent();
            _taskService = taskService;
            Owner = System.Windows.Application.Current.MainWindow;
            PositionAboveMainWindow();
        }

        private void PositionAboveMainWindow()
        {
            if (Owner is MainWindow main)
            {
                Left = main.Left;
                Top = main.Top - Height - 8;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSummary.Text))
            {
                System.Windows.MessageBox.Show("Summary line is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _taskService.AddTask(txtSummary.Text.Trim(), txtDetails.Text?.Trim());
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
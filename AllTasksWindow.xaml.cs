using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace FIFOTasker_wpf
{
    public partial class AllTasksWindow : Window
    {
        private readonly TaskService _taskService;

        public AllTasksWindow(TaskService taskService)
        {
            InitializeComponent();
            _taskService = taskService;
            LoadAllTasks();
        }

        private void LoadAllTasks()
        {
            var allTasks = _taskService.GetAllTasks();

            // Unfinished tasks first (FIFO order), then finished tasks
            var unfinished = allTasks.Where(t => t.Finished == null).ToList();
            var finished = allTasks.Where(t => t.Finished != null).ToList();

            var combined = unfinished.Concat(finished).ToList();

            lvAllTasks.ItemsSource = combined;

            // Grey out finished tasks
            lvAllTasks.ItemContainerStyle = new Style(typeof(System.Windows.Controls.ListViewItem));
            lvAllTasks.ItemContainerStyle.Setters.Add(new Setter(
                System.Windows.Controls.ListViewItem.ForegroundProperty,
                new System.Windows.Data.Binding
                {
                    Path = new PropertyPath("Finished"),
                    Converter = new FinishedToBrushConverter()
                }));
        }

        // Simple converter for grey finished tasks
        public class FinishedToBrushConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return (value is DateTime finished && finished != default(DateTime))
                    ? new SolidColorBrush(Colors.Gray)
                    : new SolidColorBrush(Colors.Black);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
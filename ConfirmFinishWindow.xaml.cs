using System.Windows;

namespace FIFOTasker_wpf
{
    public partial class ConfirmFinishWindow : Window
    {
        public ConfirmFinishWindow(string summary)
        {
            InitializeComponent();
            txtSummary.Text = summary;
            Owner = System.Windows.Application.Current.MainWindow;
            PositionAboveMainWindow();
        }

        private void PositionAboveMainWindow()
        {
            if (Owner is MainWindow main)
            {
                Left = main.Left;
                Top = main.Top - Height - 8;   // 8px gap above main window
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
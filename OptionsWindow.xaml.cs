using System.Windows;

namespace FIFOTasker_wpf
{
    public partial class OptionsWindow : Window
    {
        private readonly AutoStartService _autoStartService;

        public OptionsWindow(AutoStartService autoStartService)
        {
            InitializeComponent();
            _autoStartService = autoStartService;
            chkAutoStart.IsChecked = _autoStartService.IsEnabled();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _autoStartService.SetAutoStart(chkAutoStart.IsChecked == true);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
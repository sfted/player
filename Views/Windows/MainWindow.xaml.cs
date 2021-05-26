using Player.Core.Utils;
using Player.ViewModels.Windows;
using Player.Views.Windows;
using System.Windows;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!Settings.IsMusicLoaded)
            {
                Hide();
                var viewModel = new FirstStartupViewModel();
                var window = new FirstStartupWindow { Owner = this };
                viewModel.FinishedInitialSetup += () =>
                {
                    Show();
                    window.Close();
                };
                window.DataContext = viewModel;
                window.Show();
            }

            DataContext = new MainViewModel();
        }
    }
}

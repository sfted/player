using Player.Core.Utils;
using Player.ViewModels.Windows;
using Player.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

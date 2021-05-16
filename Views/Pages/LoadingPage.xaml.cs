using Player.ViewModels.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoaderPage.xaml
    /// </summary>
    public partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LoadingPageViewModel;
            vm.StartLoadingCommand.Execute(null);
        }
    }
}

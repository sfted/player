using Player.ViewModels.Pages;
using Player.ViewModels.Windows;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new MainPageViewModel(mainViewModel);
        }
    }
}

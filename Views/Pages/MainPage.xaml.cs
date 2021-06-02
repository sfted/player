using Player.Core.Utils.Interfaces;
using Player.ViewModels.Pages;
using Player.ViewModels.Windows;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, IUpdateable
    {
        public MainPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new MainPageViewModel(mainViewModel);
        }

        public void Update()
        {
            ((MainPageViewModel)DataContext).LoadPlaylists();
        }
    }
}

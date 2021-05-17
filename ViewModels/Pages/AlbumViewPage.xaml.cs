using Player.ViewModels.Windows;
using System.Windows.Controls;

namespace Player.ViewModels.Pages
{
    /// <summary>
    /// Interaction logic for AlbumViewPage.xaml
    /// </summary>
    public partial class AlbumViewPage : Page
    {
        public MainViewModel MainViewModel { get; private set; }

        public AlbumViewPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;
        }
    }
}

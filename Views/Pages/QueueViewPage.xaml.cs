using Player.ViewModels.Windows;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for QueueViewPage.xaml
    /// </summary>
    public partial class QueueViewPage : Page
    {
        public MainViewModel MainViewModel { get; private set; }

        public QueueViewPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;
        }
    }
}

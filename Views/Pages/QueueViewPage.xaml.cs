using Player.Core.Entities;
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
        public PlaybackQueue Queue { get; private set; }

        public QueueViewPage(MainViewModel mainViewModel, PlaybackQueue queue)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;
            Queue = queue;
            DataContext = queue;
        }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            listView.ScrollIntoView(Queue.NowPlayingTrack);
        }
    }
}

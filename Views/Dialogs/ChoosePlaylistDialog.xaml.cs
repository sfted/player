using Player.Core.Entities;
using System.Collections.Generic;
using System.Windows;

namespace Player.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ChoosePlaylistDialog.xaml
    /// </summary>
    public partial class ChoosePlaylistDialog : Window
    {
        // будет переписано с применением MVVM после 1.0
        public Playlist SelectedPlaylist { get; set; }
        public List<Playlist> Playlists { get; private set; }

        public ChoosePlaylistDialog(List<Playlist> playlists)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Playlists = playlists;
            InitializeComponent();
            DataContext = this;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            if (SelectedPlaylist != null)
                DialogResult = true;
        }
    }
}

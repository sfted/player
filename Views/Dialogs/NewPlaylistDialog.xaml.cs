using Microsoft.Win32;
using Player.Core.Entities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Player.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for NewPlaylistDialogWindow.xaml
    /// </summary>
    public partial class NewPlaylistDialog : Window
    {
        // будет переписано с применением MVVM

        public Playlist Playlist { get; private set; }

        public string PlaylistTitle { get; set; }
        public string CoverSource { get; private set; }

        public NewPlaylistDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            PlaylistTitle = "Новый плейлист";

            InitializeComponent();
            DataContext = this;
            TitleTextBox.Focus();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            Playlist = new Playlist() { Title = PlaylistTitle };

            if (CoverSource != null && File.Exists(CoverSource))
                Playlist.Cover = Cover.FromFileName(CoverSource);

            DialogResult = true;
        }

        private void SelectCoverClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png;";
            if (dialog.ShowDialog() == true)
            {
                CoverSource = dialog.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.DecodePixelHeight = 256;
                bitmap.DecodePixelWidth = 256;
                bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                bitmap.EndInit();

                CoverImage.Source = bitmap;
            }
        }
    }
}

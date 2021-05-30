using Microsoft.EntityFrameworkCore;
using Player.Core;
using Player.Core.Entities;
using Player.ViewModels.Windows;
using System.Linq;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for AlbumViewPage.xaml
    /// </summary>
    public partial class AlbumViewPage : Page
    {
        public MainViewModel MainViewModel { get; private set; }

        public AlbumViewPage(MainViewModel mainViewModel, Album album)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;

            using (var db = new ApplicationContext())
            {
                var trackedAlbum = db.Albums.Find(album.Id);

                var tracks = db.Entry(trackedAlbum)
                    .Collection(a => a.Tracks)
                    .Query()
                    .Select(ApplicationContext.TrackFast())
                    .ToList();

                var genres = db.Entry(trackedAlbum)
                    .Collection(a => a.Genres)
                    .Query()
                    .Select(ApplicationContext.GenreFast())
                    .ToList();

                db.DetachEntity(trackedAlbum);

                trackedAlbum.Tracks = tracks;
                trackedAlbum.Genres = genres;

                // TODO: сделать иначе???
                trackedAlbum.Tracks.Sort((t1, t2) => t1.Number.CompareTo(t2.Number));

                DataContext = trackedAlbum;
            }
        }
    }
}

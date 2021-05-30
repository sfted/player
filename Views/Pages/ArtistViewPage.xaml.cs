using Microsoft.EntityFrameworkCore;
using Player.Core;
using Player.Core.Entities;
using Player.ViewModels.Windows;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for ArtistViewPage.xaml
    /// </summary>
    public partial class ArtistViewPage : Page
    {
        public MainViewModel MainViewModel { get; private set; }

        public ArtistViewPage(MainViewModel mainViewModel, Artist artist)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;

            using (var db = new ApplicationContext())
            {
                var trackedArtist = db.Artists.Find(artist.Id);

                var tracks = db.Entry(trackedArtist)
                    .Collection(a => a.Tracks)
                    .Query()
                    .Select(ApplicationContext.TrackFast())
                    .ToList();

                var albums = db.Entry(trackedArtist)
                    .Collection(a => a.Albums)
                    .Query()
                    .Select(ApplicationContext.AlbumFast())
                    .ToList();

                db.DetachEntity(trackedArtist);

                trackedArtist.Tracks = tracks;
                trackedArtist.Albums = albums;

                // TODO: сделать сортировку
                DataContext = trackedArtist;
            }
        }
    }
}

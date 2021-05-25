using Microsoft.EntityFrameworkCore;
using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Windows;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Player.ViewModels.Pages
{
    public class MainPageViewModel : Notifier
    {
        public List<Track> Tracks { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }

        public MainViewModel MainViewModel { get; private set; }

        public MainPageViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            using (var db = new ApplicationContext())
            {
                Tracks = db.Tracks.Include(track => track.Artists)
                                  .ToList();

                Artists = db.Artists.Include(artist => artist.Tracks)
                                    .Include(artist => artist.Albums)
                                    .ToList();

                Albums = db.Albums.Include(album => album.Artists)
                                  .Include(album => album.Genres)
                                  .ToList();
            }
            //foreach (Track track in db.Tracks.Include(track => track.Artists))
            //    Tracks.Add(track);
            //foreach (Album album in db.Albums.Include(album => album.Artists).Include(album => album.Genres))
            //    Albums.Add(album);
            //foreach (Artist artist in db.Artists.Include(artist => artist.Tracks).Include(artist => artist.Albums))
            //    Artists.Add(artist);
        }
    }
}

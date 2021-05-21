using Microsoft.EntityFrameworkCore;
using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player.ViewModels.Pages
{
    public class MainPageViewModel : Notifier
    {
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        public ObservableCollection<Album> Albums { get; set; } = new ObservableCollection<Album>();
        public ObservableCollection<Artist> Artists { get; set; } = new ObservableCollection<Artist>();

        public MainViewModel MainViewModel { get; private set; }

        public MainPageViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            var db = new ApplicationContext();

            foreach (Track track in db.Tracks.Include(track => track.Artists))
                Tracks.Add(track);
            foreach (Album album in db.Albums.Include(album => album.Artists).Include(album => album.Genres))
                Albums.Add(album);
            foreach (Artist artist in db.Artists.Include(artist => artist.Tracks).Include(artist => artist.Albums))
                Artists.Add(artist);
        }
    }
}

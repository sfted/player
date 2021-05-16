using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils.MVVM;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Player.ViewModels.Windows
{
    public class MainViewModel : Notifier
    {
        private Page currentPage;
        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
            }
        }

        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        public ObservableCollection<Album> Albums { get; set; } = new ObservableCollection<Album>();
        public ObservableCollection<Artist> Artists { get; set; } = new ObservableCollection<Artist>();

        public MainViewModel()
        {
            var db = new ApplicationContext();
            foreach (Track track in db.Tracks)
                Tracks.Add(track);
            foreach (Album album in db.Albums)
                Albums.Add(album);
            foreach (Artist artist in db.Artists)
                Artists.Add(artist);
        }
    }
}

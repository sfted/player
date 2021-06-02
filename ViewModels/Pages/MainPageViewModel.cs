using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Windows;
using Player.Views.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Player.ViewModels.Pages
{
    public class MainPageViewModel : Notifier
    {
        public List<Track> Tracks { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public Folder RootFolder { get; set; }

        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();

        public MainViewModel MainViewModel { get; private set; }

        private RelayCommand addNewPlaylistCommand;
        public RelayCommand AddNewPlaylistCommand
        {
            get => addNewPlaylistCommand ??= new RelayCommand
            (
                obj =>
                {
                    var dialog = new NewPlaylistDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        using (var db = new ApplicationContext())
                        {
                            db.Playlists.Add(dialog.Playlist);
                            if(dialog.Playlist.Cover != null)
                                db.Covers.Add(dialog.Playlist.Cover);
                            db.SaveChanges();
                            Playlists.Add(dialog.Playlist);
                        }
                    }
                }
            );
        }

        public MainPageViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            using (var db = new ApplicationContext())
            {
                Tracks = db.LoadTracksFast();
                Albums = db.LoadAlbumsFast();
                Artists = db.LoadArtistsFast();
                var playlists = db.LoadPlaylistsFast();

                foreach (var p in playlists)
                    Playlists.Add(p);

                // TODO: доработать
                // (не помню зачем тут трай-кетч)
                try
                {
                    RootFolder = LoadRootFolder(db);
                }
                catch { }
            }
        }

        private static Folder LoadRootFolder(ApplicationContext db)
        {
            var rootFolder = db.Folders.Find(1);

            var tracks = db.Entry(rootFolder)
                .Collection(a => a.Tracks)
                .Query()
                .Select(ApplicationContext.TrackFast())
                .ToList();

            var folders = db.Entry(rootFolder)
                .Collection(a => a.Folders)
                .Query()
                .Select(ApplicationContext.FolderFast())
                .ToList();

            db.DetachEntity(rootFolder);

            rootFolder.Tracks = tracks;
            rootFolder.Folders = folders;

            return rootFolder;
        }
    }
}

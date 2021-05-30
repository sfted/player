using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Player.ViewModels.Pages
{
    public class MainPageViewModel : Notifier
    {
        public List<Track> Tracks { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public Folder RootFolder { get; set; }

        public MainViewModel MainViewModel { get; private set; }

        public MainPageViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            using (var db = new ApplicationContext())
            {
                Tracks = db.LoadTracksFast();
                Albums = db.LoadAlbumsFast();
                Artists = db.LoadArtistsFast();

                // TODO: доработать
                // (не помню зачем тут трай-кетч)
                try
                {
                    RootFolder = LoadRootFolder(db);
                }
                catch { }
            }
        }

        private Folder LoadRootFolder(ApplicationContext db)
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

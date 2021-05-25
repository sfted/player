using Player.Core;
using Player.Core.Entities;
using Player.ViewModels.Windows;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for FolderViewPage.xaml
    /// </summary>
    public partial class FolderViewPage : Page
    {
        public MainViewModel MainViewModel { get; private set; }

        public FolderViewPage(MainViewModel mainViewModel, Folder folder)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;

            using (var db = new ApplicationContext())
            {
                var trackedFolder = db.Folders.Find(folder.Id);
                db.Entry(trackedFolder).Collection(f => f.Folders).Load();
                db.Entry(trackedFolder).Collection(f => f.Tracks).Load();
                foreach (var track in trackedFolder.Tracks)
                {
                    db.Entry(track).Collection(t => t.Artists).Load();
                    db.Entry(track).Reference(t => t.Album).Load();
                    db.Entry(track.Album).Collection(a => a.Artists).Load();
                    db.Entry(track.Album).Collection(a => a.Genres).Load();
                    foreach(var artist in track.Artists)
                        db.Entry(artist).Collection(a => a.Albums).Load();
                }
                DataContext = trackedFolder;
            }
        }
    }
}

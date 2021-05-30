using Player.Core;
using Player.Core.Entities;
using Player.ViewModels.Windows;
using System.Linq;
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

                var tracks = db.Entry(trackedFolder)
                    .Collection(a => a.Tracks)
                    .Query()
                    .Select(ApplicationContext.TrackFast())
                    .ToList();

                var folders = db.Entry(trackedFolder)
                    .Collection(a => a.Folders)
                    .Query()
                    .Select(ApplicationContext.FolderFast())
                    .ToList();

                db.DetachEntity(trackedFolder);

                trackedFolder.Tracks = tracks;
                trackedFolder.Folders = folders;

                // TODO: сделать сортировку
                DataContext = trackedFolder;
            }
        }
    }
}

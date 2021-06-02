using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils.Interfaces;
using System.Linq;
using System.Windows.Controls;

namespace Player.Views.Pages
{
    /// <summary>
    /// Interaction logic for PlaylistViewPage.xaml
    /// </summary>
    public partial class PlaylistViewPage : Page
    {
        public PlaylistViewPage(Playlist playlist)
        {
            InitializeComponent();

            using (var db = new ApplicationContext())
            {
                var trackedPlaylist = db.Playlists.Find(playlist.Id);

                var tracks = db.Entry(trackedPlaylist)
                    .Collection(a => a.Tracks)
                    .Query()
                    .Select(ApplicationContext.TrackFast())
                    .ToList();

                db.DetachEntity(trackedPlaylist);

                trackedPlaylist.Tracks = tracks;

                // TODO: сделать сортировку

                DataContext = trackedPlaylist;
            }
        }
    }
}

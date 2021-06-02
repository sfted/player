using Player.Core.Entities.Interfaces;
using Player.Core.Utils.MVVM;
using System.Windows;

namespace Player.Core.Entities
{
    public class Playlist : TrackListWithCover, IPlayable
    {
        // после 1.0 это будет перенесено в отдельную ViewModel
        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get => deleteCommand ??= new RelayCommand
            (
                obj =>
                {
                    var result = MessageBox.Show
                    (
                        "Удалить этот плейлист?",
                        "Подтвердите действие",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No
                    );

                    if(result == MessageBoxResult.Yes)
                    {
                        using (var db = new ApplicationContext())
                        {
                            var trackedPlaylist = db.Playlists.Find(Id);
                            db.Playlists.Remove(trackedPlaylist);
                            db.SaveChanges();

                            MessageBox.Show
                            (
                                "Плейлист удален!",
                                "Готово",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                        }
                    }
                }
            );
        }
    }
}

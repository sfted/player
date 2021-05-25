using Player.Core.Entities;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using Player.Views.Pages;
using System.Windows.Controls;

namespace Player.ViewModels.Windows
{
    public class MainViewModel : Notifier
    {
        public Navigation Navigation { get; private set; }

        private RelayCommand navigateToCommand;
        public RelayCommand NavigateToCommand
        {
            get => navigateToCommand ??= new RelayCommand(obj =>
            {
                if (obj is Page page)
                    Navigation.NavigateTo(page);
            });
        }

        private RelayCommand navigateBackCommand;
        public RelayCommand NavigateBackCommand
        {
            get => navigateBackCommand ??= new RelayCommand
            (
                obj => Navigation.NavigateBack(),
                obj => Navigation.CanNavigateBack()
            );
        }

        private RelayCommand openEntityCommand;
        public RelayCommand OpenEntityCommand
        {
            get => openEntityCommand ??= new RelayCommand(obj =>
            {
                Page page;
                if (obj is Album album)
                {
                    album.Tracks.Sort((t1, t2) => t1.Number.CompareTo(t2.Number));
                    page = new AlbumViewPage(this) { DataContext = album };
                }
                else if (obj is Artist artist)
                    page = new ArtistViewPage(this) { DataContext = artist };
                else return;

                Navigation.NavigateTo(page);
            });
        }

        public MainViewModel()
        {
            Navigation = new Navigation(new MainPage(this));
        }
    }
}

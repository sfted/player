﻿using Player.Core;
using Player.Core.Entities;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Pages;
using Player.Views.Dialogs;
using Player.Views.Pages;
using System.Windows.Controls;

namespace Player.ViewModels.Windows
{
    public class MainViewModel : Notifier
    {
        public Navigation Navigation { get; private set; } = new Navigation();
        public Core.Player Player { get; private set; } = new Core.Player();

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
                obj =>
                {
                    Navigation.NavigateBack();
                    Navigation.UpdateCurrentPage();
                },
                obj => Navigation.CanNavigateBack()
            );
        }

        private RelayCommand openEntityCommand;
        public RelayCommand OpenEntityCommand
        {
            get => openEntityCommand ??= new RelayCommand(obj =>
            {
                // будет переписано после 1.0
                bool navigated = false;
                Page page;
                if (obj is Album album)
                {
                    page = new AlbumViewPage(this, album);
                }
                else if (obj is Artist artist)
                {
                    page = new ArtistViewPage(this, artist);
                }
                else if (obj is Folder folder)
                {
                    page = new FolderViewPage(this, folder);
                }
                else if (obj is PlaybackQueue queue)
                {
                    page = new QueueViewPage(this, queue);
                    if (Navigation.CurrentPage.DataContext == page.DataContext)
                        Navigation.NavigateBack();
                    else
                        Navigation.NavigateTo(page);

                    navigated = true;
                }
                else if (obj is Playlist playlist)
                {
                    page = new PlaylistViewPage(this, playlist);
                }
                else return;

                if (Navigation.CurrentPage.DataContext != page.DataContext && !navigated)
                    Navigation.NavigateTo(page);
            });
        }

        private RelayCommand addToPlaylistCommand;
        public RelayCommand AddToPlaylistCommand
        {
            get => addToPlaylistCommand ??= new RelayCommand
            (
                obj =>
                {
                    if (obj is Track track)
                    {
                        using (var db = new ApplicationContext())
                        {
                            var dialog = new ChoosePlaylistDialog(db.LoadPlaylistsFast());
                            if (dialog.ShowDialog() == true)
                            {
                                var playlist = db.Playlists.Find(dialog.SelectedPlaylist.Id);
                                var trackedTrack = db.Tracks.Find(track.Id);
                                playlist.Tracks.Add(trackedTrack);
                                db.SaveChanges();

                                db.DetachEntity(playlist);
                                db.DetachEntity(trackedTrack);
                            }
                        }
                    }
                }
            );
        }

        public MainViewModel()
        {
            if (!Settings.MusicIsLoaded)
                GoToTheWelcomePage();
            else
                GoToTheMainPage();
        }

        private void GoToTheWelcomePage()
        {
            var vm = new WelcomePageViewModel { LibraryDirectory = Settings.MusicDirectory };
            vm.FinishedInitialSetup += GoToTheLoadingPage;
            var page = new WelcomePage { DataContext = vm };

            Navigation.IsNavigationAllowed = false;
            Navigation.NavigateTo(page);
        }

        private void GoToTheLoadingPage()
        {
            var vm = new LoadingPageViewModel { LibraryDirectory = Settings.MusicDirectory };
            vm.FinishedLoading += () =>
            {
                Settings.MusicIsLoaded = true;
                Settings.Save();
                Navigation.ClearJournal();
                Navigation.IsNavigationAllowed = true;
                GoToTheMainPage();
            };
            var page = new LoadingPage { DataContext = vm };

            Navigation.NavigateTo(page);
        }

        private void GoToTheMainPage()
        {
            Navigation.NavigateTo(new MainPage(this));
        }
    }
}

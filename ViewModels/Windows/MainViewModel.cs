using Player.Core.Entities;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Pages;
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
                obj => Navigation.NavigateBack(),
                obj => Navigation.CanNavigateBack()
            );
        }

        private RelayCommand openEntityCommand;
        public RelayCommand OpenEntityCommand
        {
            get => openEntityCommand ??= new RelayCommand(obj =>
            {
                // будет переписано в будущем
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
                else if (obj is Core.Player player)
                {
                    page = new QueueViewPage(this) { DataContext = player };
                    if (Navigation.CurrentPage.DataContext == page.DataContext)
                        Navigation.NavigateBack();
                    else
                        Navigation.NavigateTo(page);

                    navigated = true;
                }
                else return;

                if (Navigation.CurrentPage.DataContext != page.DataContext && !navigated)
                    Navigation.NavigateTo(page);
            });
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

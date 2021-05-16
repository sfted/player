using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using Player.ViewModels.Pages;
using Player.Views.Pages;
using System;
using System.Windows.Controls;

namespace Player.ViewModels.Windows
{
    public class FirstStartupViewModel : Notifier
    {
        private readonly WelcomePage welcomePage = new();
        private readonly LoadingPage loadingPage = new();

        public event Action FinishedInitialSetup;

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

        private bool finishedLoading = false;
        public bool FinishedLoading
        {
            get => finishedLoading;
            set
            {
                finishedLoading = value;
                NotifyPropertyChanged(nameof(FinishedLoading));
            }
        }

        public FirstStartupViewModel()
        {
            MoveToTheWelcomePage();
        }

        private void MoveToTheWelcomePage()
        {
            var welcomeVm = new WelcomePageViewModel { LibraryDirectory = Settings.MusicDirectory };
            welcomeVm.FinishedInitialSetup += MoveToTheLoadingPage;

            welcomePage.DataContext = welcomeVm;
            CurrentPage = welcomePage;
        }

        private void MoveToTheLoadingPage()
        {
            var loadingVM = new LoadingPageViewModel { LibraryDirectory = Settings.MusicDirectory };
            loadingVM.FinishedLoading += () =>
            {
                FinishedLoading = true;
                FinishedInitialSetup();
            };

            loadingPage.DataContext = loadingVM;
            CurrentPage = loadingPage;
        }
    }
}

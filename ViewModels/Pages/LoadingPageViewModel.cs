using Player.Core;
using Player.Core.Utils.MVVM;
using System;

namespace Player.ViewModels.Pages
{
    public class LoadingPageViewModel : Notifier
    {
        public event Action FinishedLoading;

        private string libraryDirectory;
        public string LibraryDirectory
        {
            get => libraryDirectory;
            set
            {
                libraryDirectory = value;
                NotifyPropertyChanged(nameof(LibraryDirectory));
            }
        }

        private FileLoader loader;
        public FileLoader Loader
        {
            get => loader;
            set
            {
                loader = value;
                NotifyPropertyChanged(nameof(Loader));
            }
        }

        private RelayCommand startLoadingCommand;
        public RelayCommand StartLoadingCommand
        {
            get => startLoadingCommand ??= new RelayCommand
            (
                obj => LoadFiles(),
                obj => LoaderIsNotRunning()
            );
        }

        private bool LoaderIsNotRunning()
        {
            if (Loader != null)
                return !Loader.IsRunning;
            else
                return true;
        }

        private void LoadFiles()
        {
            Loader = new FileLoader(LibraryDirectory);
            Loader.LoadingCompleted += FinishedLoading;
            Loader.LoadFilesAsync();
        }
    }
}

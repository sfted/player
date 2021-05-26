using Ookii.Dialogs.Wpf;
using Player.Core;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using System;
using System.Windows;

namespace Player.ViewModels.Pages
{
    public class WelcomePageViewModel : Notifier
    {
        public event Action FinishedInitialSetup;

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

        private RelayCommand chooseDirectoryCommand;
        public RelayCommand ChooseDirectoryCommand
        {
            get => chooseDirectoryCommand ??= new RelayCommand(obj => ChooseDirectory());
        }

        private RelayCommand checkDirectoryCommand;
        public RelayCommand CheckDirectoryCommand
        {
            get => checkDirectoryCommand ??= new RelayCommand(obj => CheckDirectory());
        }

        private void ChooseDirectory()
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                SelectedPath = LibraryDirectory
            };

            if (dialog.ShowDialog().GetValueOrDefault())
                LibraryDirectory = dialog.SelectedPath;
        }

        private void CheckDirectory()
        {
            switch (FileLoader.GetDirectoryStatus(LibraryDirectory))
            {
                case FileLoader.LibraryDirectoryStatus.OK:
                    FinishInitialSetup();
                    break;

                case FileLoader.LibraryDirectoryStatus.DirectoryNotFound:
                    MessageBox.Show("Такой папки не существует!\nПроверь правильность ввода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case FileLoader.LibraryDirectoryStatus.NoMusicFilesFound:
                    var result = MessageBox.Show("В этой папке нет музыки!\nПродолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes) FinishInitialSetup();
                    break;
            }
        }

        private void FinishInitialSetup()
        {
            Settings.MusicDirectory = LibraryDirectory;
            Settings.Save();
            FinishedInitialSetup();
        }
    }
}

using Player.Core.Utils;
using System;
using System.IO;
using System.Windows;

namespace Player
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string APPLICATION_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Music Player\\";
        public static readonly string DATABASE_URI = APPLICATION_DIRECTORY + "database.sqlite";
        public static readonly string SETTINGS_URI = APPLICATION_DIRECTORY + "settings.json";

        public App()
        {
            InitializeDirectories();
            Settings.Load(SETTINGS_URI);
        }

        private static void InitializeDirectories()
        {
            if (!Directory.Exists(APPLICATION_DIRECTORY))
                Directory.CreateDirectory(APPLICATION_DIRECTORY);
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Settings.Save();
        }
    }
}

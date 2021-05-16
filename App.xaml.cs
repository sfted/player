using Player.Core.Utils;
using Serilog;
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
        //public static readonly string LOG_URI = APPLICATION_DIRECTORY + "logs\\.log";
        public static readonly string ALBUM_ARTS_DIRECTORY = APPLICATION_DIRECTORY + "arts\\";

        public App()
        {
            InitializeDirectories();
            InitializeLogger();

            Log.Information("I'm alive!");
            Log.Fatal("🅰🅱🅾🅱🅰");

            Settings.Load(SETTINGS_URI);
        }

        private static void InitializeDirectories()
        {
            if (!Directory.Exists(APPLICATION_DIRECTORY))
                Directory.CreateDirectory(APPLICATION_DIRECTORY);

            if (!Directory.Exists(ALBUM_ARTS_DIRECTORY))
                Directory.CreateDirectory(ALBUM_ARTS_DIRECTORY);
        }

        private static void InitializeLogger()
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
                // TODO: add write to file option (?)
                .CreateLogger();

            Log.Logger = log;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Log.Information("Завершение работы приложения...");
            Settings.Save();
        }
    }
}

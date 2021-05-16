using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;

namespace Player.Core.Utils
{
    public static class Settings
    {
        private static string path;
        private static SettingsProvider settings;

        public static bool IsMusicLoaded
        {
            get { return settings.FilesAreLoaded; }
            set { settings.FilesAreLoaded = value; }
        }

        public static string MusicDirectory
        {
            get { return settings.LibraryDirectory; }
            set { settings.LibraryDirectory = value; }
        }

        private class SettingsProvider
        {
            public bool FilesAreLoaded { get; set; }
            public string LibraryDirectory { get; set; }

            public SettingsProvider(bool isBlank = false)
            {
                if (isBlank)
                {
                    FilesAreLoaded = false;
                    //LibraryDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    LibraryDirectory = @"D:\Music\New";
                }
            }
        }

        public static void Save()
        {
            Log.Information("Сохранение настроек...");
            File.WriteAllText(path, JObject.FromObject(settings).ToString());
        }

        public static void Load(string settings_uri)
        {
            path = settings_uri;
            Log.Information("Поиск файла настроек по пути '{Path}'", path);
            if (!File.Exists(path))
            {
                Log.Warning("Файл настроек не найден. Загружаю дефолтные настройки...");
                settings = new SettingsProvider(true);
            }
            else
            {
                Log.Information("Файл настроек найден. Загружаю...");
                settings = JObject.Parse(File.ReadAllText(path)).ToObject<SettingsProvider>();
            }
            Log.Debug("Текущие настройки: {@Settings}", settings);
        }
    }
}

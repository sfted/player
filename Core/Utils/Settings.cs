using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Player.Core.Utils
{
    public static class Settings
    {
        private static string path;
        private static SettingsProvider settings;

        public static bool MusicIsLoaded
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
            File.WriteAllText(path, JObject.FromObject(settings).ToString());
        }

        public static void Load(string settings_uri)
        {
            path = settings_uri;
            if (!File.Exists(path))
            {
                settings = new SettingsProvider(true);
            }
            else
            {
                settings = JObject.Parse(File.ReadAllText(path)).ToObject<SettingsProvider>();
            }
        }
    }
}

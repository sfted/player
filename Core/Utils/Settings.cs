using Newtonsoft.Json.Linq;
using Player.Core.Entities;
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
            get { return settings.MusicIsLoaded; }
            set { settings.MusicIsLoaded = value; }
        }

        public static string MusicDirectory
        {
            get { return settings.LibraryDirectory; }
            set { settings.LibraryDirectory = value; }
        }

        public static bool ShuffleIsEnabled
        {
            get { return settings.ShuffleIsEnabled; }
            set { settings.ShuffleIsEnabled = value; }
        }

        public static PlaybackQueue.RepeatModes RepeatMode
        {
            get { return settings.RepeatMode; }
            set { settings.RepeatMode = value; }
        }

        public static double Volume
        {
            get { return settings.Volume; }
            set { settings.Volume = value; }
        }

        private class SettingsProvider
        {
            public bool MusicIsLoaded { get; set; }
            public string LibraryDirectory { get; set; }
            public bool ShuffleIsEnabled { get; set; }
            public PlaybackQueue.RepeatModes RepeatMode { get; set; }
            public double Volume { get; set; }

            public SettingsProvider(bool isBlank = false)
            {
                if (isBlank)
                {
                    MusicIsLoaded = false;
                    LibraryDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
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

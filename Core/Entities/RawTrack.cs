using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TagLib;

namespace Player.Core.Entities
{
    public class RawTrack
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public string Title { get; set; }
        public string[] Artists { get; set; }
        public string AlbumTitle { get; set; }
        public string[] AlbumArtists { get; set; }
        public uint AlbumYear { get; set; }
        public string[] AlbumGenres { get; set; }
        public uint TrackNumber { get; set; }
        public uint AlbumTrackCount { get; set; }
        public uint DiscNumber { get; set; }
        public uint AlbumDiscCount { get; set; }
        public IPicture AlbumArt { get; set; }
        public int BitRate { get; set; }
        public TimeSpan Duration { get; set; }

        public RawTrack(string fileName)
        {
            FileName = fileName;
        }

        public void LoadMetadata()
        {
            var fileInfo = new FileInfo(FileName);
            FileSize = fileInfo.Length;
            FillTags(TagLib.File.Create(FileName));
        }

        private void FillTags(TagLib.File tfile)
        {
            // может быть улучшено в будущем
            var tagsFromFileName = Path.GetFileNameWithoutExtension(FileName).Split('-');
            Title = tfile.Tag.Title;
            if (Title == null)
            {
                // может быть улучшено в будущем
                if (tagsFromFileName.Length > 1)
                    Title = tagsFromFileName[1].Trim();
                else
                    Title = tagsFromFileName[0].Trim();
            }

            Artists = tfile.Tag.Performers;
            if (Artists.Length == 0)
            {
                // может быть улучшено в будущем
                if (tagsFromFileName.Length > 1)
                    Artists = new string[1] { tagsFromFileName[0].Trim() };
                else
                    Artists = new string[1] { "Unknown artist" };
            }

            AlbumArtists = tfile.Tag.AlbumArtists;
            if (AlbumArtists.Length == 0)
                AlbumArtists = new string[1] { "Unknown artist" };

            AlbumGenres = tfile.Tag.Genres;
            if (AlbumGenres.Length == 0)
                AlbumGenres = new string[1] { "Unknown genre" };

            AlbumTitle = tfile.Tag.Album ?? "Unknown album";
            AlbumYear = tfile.Tag.Year;
            TrackNumber = tfile.Tag.Track;
            AlbumTrackCount = tfile.Tag.TrackCount;
            DiscNumber = tfile.Tag.Disc;
            AlbumDiscCount = tfile.Tag.DiscCount;

            if (tfile.Tag.Pictures.Length > 0)
                AlbumArt = tfile.Tag.Pictures[0];

            BitRate = tfile.Properties.AudioBitrate;
            Duration = tfile.Properties.Duration;
        }
    }
}

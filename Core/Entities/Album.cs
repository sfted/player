using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Player.Core.Entities
{
    public class Album : BaseEntity
    {
        public string Title { get; set; }
        public uint Year { get; set; }
        public uint TrackCount { get; set; }
        public uint DiscCount { get; set; }
        public string AlbumArtUri { get; set; }

        public List<Track> Tracks { get; set; } = new List<Track>();
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Genre> Genres { get; set; } = new List<Genre>();

        public void RenderAlbumArtUri(string artsDirectory)
        {
            AlbumArtUri = $"{artsDirectory}{Id}.jpg";
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

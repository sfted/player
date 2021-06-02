using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Player.Core.Entities
{
    public class Track : BaseEntity
    { 
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public uint Number { get; set; }
        public uint Disc { get; set; }
        public int BitRate { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public Album Album { get; set; }
        public virtual List<Artist> Artists { get; set; } = new List<Artist>();
        public virtual List<Playlist> Playlists { get; set; } = new List<Playlist>();

        [NotMapped]
        public double DurationInSeconds
        {
            get => Duration.TotalSeconds;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

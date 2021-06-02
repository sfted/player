using Player.Core.Entities.Interfaces;
using System.Collections.Generic;

namespace Player.Core.Entities
{
    public class Album : TrackListWithCover, IPlayable
    {
        public uint Year { get; set; }
        public uint TrackCount { get; set; }
        public uint DiscCount { get; set; }

        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}

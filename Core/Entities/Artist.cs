using Player.Core.Entities.Interfaces;
using System.Collections.Generic;

namespace Player.Core.Entities
{
    public class Artist : BaseEntity, IPlayable
    {
        public string Name { get; set; }
        public List<Album> Albums { get; set; } = new List<Album>();
        public List<Track> Tracks { get; set; } = new List<Track>();

        public override string ToString()
        {
            return Name;
        }
    }
}

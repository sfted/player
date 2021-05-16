using System;
using System.Collections.Generic;

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
        public List<Artist> Artists { get; set; } = new List<Artist>();

        public override string ToString()
        {
            return Title;
        }
    }
}

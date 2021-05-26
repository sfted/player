using Player.Core.Entities.Interfaces;
using System.Collections.Generic;

namespace Player.Core.Entities
{
    public class Folder : BaseEntity, IPlayable
    {
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public uint Depth { get; set; }
        public int? FolderId { get; set; }
        public List<Folder> Folders { get; set; }
        public List<Track> Tracks { get; set; }

        public Folder()
        {
            Folders = new List<Folder>();
            Tracks = new List<Track>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

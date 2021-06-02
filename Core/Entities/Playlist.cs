using Player.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player.Core.Entities
{
    public class Playlist : BaseEntity, IPlayable
    {
        public string Title { get; set; }
        public List<Track> Tracks { get; set; }
    }
}

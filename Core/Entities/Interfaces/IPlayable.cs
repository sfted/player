using System.Collections.Generic;

namespace Player.Core.Entities.Interfaces
{
    public interface IPlayable
    {
        public List<Track> Tracks { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player.Core.Entities
{
    public class Genre : BaseEntity
    {
        public List<Album> Albums { get; set; } = new List<Album>();
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

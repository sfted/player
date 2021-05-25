using Microsoft.EntityFrameworkCore;
using Player.Core.Entities;
using System.Linq;

namespace Player.Core
{
    class ApplicationContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumArt> Arts { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Folder> Folders { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string fileName = "Filename=" + App.DATABASE_URI;
            optionsBuilder.UseSqlite(fileName);
        }

        public byte[] GetAlbumArtDataById(int id)
        {
            /* 
             * https://stackoverflow.com/questions/27641606/loading-a-large-amount-of-images-to-be-displayed-in-a-wrappanel
             * https://metanit.com/sharp/entityframeworkcore/5.7.php
             * https://stackoverflow.com/questions/34967116/how-to-combine-find-and-asnotracking
             */

            return Arts.AsNoTracking().First(a => a.Id == id).Data;
        }
    }
}
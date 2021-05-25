using Microsoft.EntityFrameworkCore;
using Player.Core.Entities;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Player.Core
{
    class ApplicationContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumArt> Arts { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string fileName = "Filename=" + App.DATABASE_URI;
            optionsBuilder.UseSqlite(fileName);
        }

        public BitmapImage LoadAlbumArtById(int id, int size)
        {
            /* 
             * https://stackoverflow.com/questions/27641606/loading-a-large-amount-of-images-to-be-displayed-in-a-wrappanel
             * https://metanit.com/sharp/entityframeworkcore/5.7.php
             * https://stackoverflow.com/questions/34967116/how-to-combine-find-and-asnotracking
             */

            byte[] data = Arts.AsNoTracking().First(a => a.Id == id).Data;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(data);
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = size;
            bitmap.DecodePixelHeight = size;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
    }
}
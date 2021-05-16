using Microsoft.EntityFrameworkCore;
using Player.Core.Entities;

namespace Player.Core
{
    class ApplicationContext : DbContext
    {

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string fileName = "Filename=" + App.DATABASE_URI;
            //var log = ConfigureLogger(App.DATABASE_LOG_URI);
            optionsBuilder.UseSqlite(fileName);
            //optionsBuilder.LogTo(log.Verbose, LogLevel.Debug);
        }

        //private Logger ConfigureLogger(string filePath)
        //{
        //    return new LoggerConfiguration()
        //           .MinimumLevel.Verbose()
        //           .WriteTo.File(
        //               path: filePath,
        //               rollingInterval: RollingInterval.Day,
        //               encoding: Encoding.Unicode,
        //               outputTemplate: "{Message:l}{NewLine}"
        //           ).CreateLogger();
        //}
    }
}
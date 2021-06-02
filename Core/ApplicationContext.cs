using Microsoft.EntityFrameworkCore;
using Player.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Player.Core
{
    class ApplicationContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

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

            return Covers.AsNoTracking().First(a => a.Id == id).Data;
        }

        public void DetachEntity(BaseEntity entity)
        {
            Entry(entity).State = EntityState.Detached;
        }

        public List<Track> LoadTracksFast()
        {
            return Tracks.Select(TrackFast())
            .AsNoTracking()
            .ToList();
        }

        public List<Album> LoadAlbumsFast()
        {
            return Albums.Select(AlbumFast())
            .AsNoTracking()
            .ToList();
        }

        public List<Artist> LoadArtistsFast()
        {
            return Artists.Select(ArtistFast())
            .AsNoTracking()
            .ToList();
        }

        public List<Playlist> LoadPlaylistsFast()
        {
            return Playlists.Select(PlaylistFast())
            .AsNoTracking()
            .ToList();
        }

        public static Expression<Func<Track, Track>> TrackFast()
        {
            return track => new Track
            {
                Id = track.Id,
                Title = track.Title,
                FileName = track.FileName,
                Duration = track.Duration,
                Number = track.Number,

                Artists = track.Artists.Select(artist => new Artist
                {
                    Id = artist.Id,
                    Name = artist.Name
                }).ToList(),

                Album = new Album
                {
                    Id = track.Album.Id,
                    Title = track.Album.Title,
                    CoverId = track.Album.CoverId
                }
            };
        }

        public static Expression<Func<Album, Album>> AlbumFast()
        {
            return album => new Album
            {
                Id = album.Id,
                Title = album.Title,
                CoverId = album.CoverId,
                Year = album.Year,

                Artists = album.Artists.Select(artist => new Artist
                {
                    Id = artist.Id,
                    Name = artist.Name
                }).ToList()
            };
        }

        public static Expression<Func<Artist, Artist>> ArtistFast()
        {
            return artist => new Artist
            {
                Id = artist.Id,
                Name = artist.Name
            };
        }

        public static Expression<Func<Folder, Folder>> FolderFast()
        {
            return folder => new Folder
            {
                Id = folder.Id,
                Name = folder.Name
            };
        }

        public static Expression<Func<Genre, Genre>> GenreFast()
        {
            return genre => new Genre
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }


        public static Expression<Func<Playlist, Playlist>> PlaylistFast()
        {
            return playlist => new Playlist
            {
                Id = playlist.Id,
                Title = playlist.Title,
                CoverId = playlist.CoverId
            };
        }

    }
}
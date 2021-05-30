using Player.Core.Entities;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TagLib;

namespace Player.Core
{
    // загружает данные из файлов в промежуточный класс RawTrack
    public class FileLoader : Notifier
    {
        // сделать перечисление вместо массива строк?
        private static readonly string[] SUPPORTED_FORMATS =
        {
            // временно только мп3
            "mp3"
        };

        private readonly ApplicationContext database = new ApplicationContext();
        private List<Album> albums { get; set; } = new List<Album>();
        private List<Artist> artists { get; set; } = new List<Artist>();
        private List<Genre> genres { get; set; } = new List<Genre>();
        private List<Folder> folders { get; set; } = new List<Folder>();
        private readonly Folder rootFolder = new Folder { Depth = 0 };

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public event Action LoadingCompleted;

        private int progressPercentage = 0;
        public int ProgressPercentage
        {
            get { return progressPercentage; }
            set
            {
                progressPercentage = value;
                NotifyPropertyChanged(nameof(ProgressPercentage));
            }
        }

        private bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                NotifyPropertyChanged(nameof(IsRunning));
            }
        }

        public enum LibraryDirectoryStatus
        {
            DirectoryNotFound,
            NoMusicFilesFound,
            OK
        }

        public static LibraryDirectoryStatus GetDirectoryStatus(string pathToLibrary)
        {
            if (!Directory.Exists(pathToLibrary))
                return LibraryDirectoryStatus.DirectoryNotFound;

            string[] files = Directory.GetFiles(pathToLibrary, "*.*", SearchOption.AllDirectories);
            foreach (string format in SUPPORTED_FORMATS)
            {
                // если есть хотя бы один файл с одним из поддерживаемых форматов, то вернуть статус OK
                string musicFile = string.Empty;
                try
                {
                    musicFile = files.First(s => s.Contains(format));
                }
                catch { }

                if (!string.IsNullOrEmpty(musicFile))
                {
                    return LibraryDirectoryStatus.OK;
                }
            }

            return LibraryDirectoryStatus.NoMusicFilesFound;
        }

        public FileLoader(string pathToLibrary)
        {
            rootFolder.Name = pathToLibrary.Split('\\').Last();
            rootFolder.RelativePath = pathToLibrary.Remove(pathToLibrary.IndexOf(rootFolder.Name), rootFolder.Name.Length);
            rootFolder.RelativePath = rootFolder.RelativePath.Remove(rootFolder.RelativePath.Length - 1, 1);
            folders.Add(rootFolder);
            database.Folders.Add(rootFolder);

            worker.WorkerReportsProgress = true;
            worker.DoWork += (object sender, DoWorkEventArgs e) => LoadFiles(pathToLibrary);
            worker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => ProgressPercentage = e.ProgressPercentage;
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                ProgressPercentage = 100;
                database.SaveChanges();
                LoadingCompleted();
            };

            // TODO: обработчик отмены/ошибок
        }

        public void LoadFilesAsync()
        {
            worker.RunWorkerAsync();
        }

        private void LoadFiles(string pathToLibrary)
        {
            IsRunning = true;
            string[] files = Directory.GetFiles(pathToLibrary, "*.*", SearchOption.AllDirectories);
            int progress = 0;
            int goal = files.Length;

            foreach (string format in SUPPORTED_FORMATS)
            {
                var filteredFiles = files.Where(s => s.Contains(format));

                foreach (string file in filteredFiles)
                {
                    LoadNewFile(file, pathToLibrary);
                    progress++;
                    int percent = Convert.ToInt32((double)progress / (double)goal * 100);
                    worker.ReportProgress(percent);
                }
            }
        }

        private void LoadNewFile(string fileName, string pathToLibrary)
        {
            var raw = new RawTrack(fileName);
            try
            {
                raw.LoadMetadata();
            }
            catch (UnsupportedFormatException)
            {
                return;
            }
            catch (TagLib.CorruptFileException)
            {
                return;
            }

            var track = new Track
            {
                BitRate = raw.BitRate,
                Disc = raw.DiscNumber,
                FileName = raw.FileName,
                FileSize = raw.FileSize,
                Duration = raw.Duration,
                Number = raw.TrackNumber,
                Title = raw.Title,
            };

            AddToNeededFolder(track, pathToLibrary);
            database.Tracks.Add(track);

            var album = FindExistingOrCreateNewAlbum
            (
                raw.AlbumTitle,
                raw.AlbumYear,
                raw.AlbumTrackCount,
                raw.AlbumDiscCount,
                raw.AlbumArt
            );

            album.Tracks.Add(track);

            foreach (string artistName in raw.Artists) FindExistingOrCreateNewArtist(artistName).Tracks.Add(track);
            foreach (string artistName in raw.AlbumArtists) FindExistingOrCreateNewArtist(artistName).Albums.Add(album);
            foreach (string genreName in raw.AlbumGenres) FindExistingOrCreateNewGenre(genreName).Albums.Add(album);

            _ = raw;
        }

        private void AddToNeededFolder(Track track, string pathToLibrary)
        {
            string relativePathToFile = Path.GetDirectoryName(track.FileName).Remove(0, pathToLibrary.Length);

            if (relativePathToFile.Length > 0 && relativePathToFile.Contains('\\'))
                relativePathToFile = relativePathToFile.Remove(0, 1);

            string[] folders = relativePathToFile.Split('\\');
            if ((folders.Length == 0) ||
                (folders.Length == 1 && folders[0] == ""))
            {
                rootFolder.Tracks.Add(track);
            }
            else if (folders.Length == 1)
            {
                var folder = FindExistingOrCreateNewFolder(folders[0], @"\");
                folder.Depth = 1;
                folder.Tracks.Add(track);
                rootFolder.Folders.Add(folder);
            }
            else
            {
                var previousFolder = FindExistingOrCreateNewFolder(folders[0], @"\");
                previousFolder.Depth = 1;
                rootFolder.Folders.Add(previousFolder);

                for (int i = 1; i < folders.Length; i++)
                {
                    string folderName = folders[i];
                    var folder = FindExistingOrCreateNewFolder(
                        folderName, previousFolder.RelativePath + previousFolder.Name + @"\");

                    folder.Depth = (uint)i + 1;
                    previousFolder.Folders.Add(folder);

                    if (i == folders.Length - 1)
                        folder.Tracks.Add(track);
                    else
                        previousFolder = folder;
                }
            }
        }

        private Folder FindExistingOrCreateNewFolder(string name, string relativePath)
        {
            var folder = folders.Find(ByNameAndRelativePath(name, relativePath));
            if (folder == null)
            {
                folder = new Folder { Name = name, RelativePath = relativePath };
                folders.Add(folder);
                database.Folders.Add(folder);
            }
            return folder;
        }

        private Album FindExistingOrCreateNewAlbum(string title, uint year, uint trackCount, uint discCount, byte[] albumArtData)
        {
            // я не думаю, что какие-то два случайных исполнителя выпустят в один и тот же год
            // по альбому в одинаковыми названиями и одинаковым количеством треков.
            // да еще и каков шанс что два таких альбома окажутся в медиатеке юзернейма
            var album = albums.Find(ByAlbumTitleYearAndTrackCount(title, year, trackCount));
            if (album == null)
            {
                album = new Album
                {
                    DiscCount = discCount,
                    Title = title,
                    TrackCount = trackCount,
                    Year = year
                };

                if (albumArtData != null)
                {
                    var art = new AlbumArt { Data = albumArtData };
                    album.Art = art;
                    database.Arts.Add(art);
                }

                albums.Add(album);
                database.Albums.Add(album);
            }
            return album;
        }

        private Artist FindExistingOrCreateNewArtist(string name)
        {
            // здесь уже ничего не поделаешь, бывает встречаются исполнители с одинаковыми именами
            var artist = artists.Find(ByArtistName(name));
            if (artist == null)
            {
                artist = new Artist { Name = name };
                artists.Add(artist);
                database.Artists.Add(artist);
            }
            return artist;
        }

        private Genre FindExistingOrCreateNewGenre(string name)
        {
            var genre = genres.Find(ByGenreName(name));
            if (genre == null)
            {
                genre = new Genre { Name = name };
                genres.Add(genre);
                database.Genres.Add(genre);
            }
            return genre;
        }

        private static Predicate<Folder> ByNameAndRelativePath(string name, string relativePath)
        {
            return delegate (Folder folder)
            {
                return (folder.Name == name) &&
                       (folder.RelativePath == relativePath);
            };
        }

        private static Predicate<Album> ByAlbumTitleYearAndTrackCount(string title, uint year, uint trackCount)
        {
            return delegate (Album album)
            {
                return (album.Title == title) &&
                       (album.Year == year) &&
                       (album.TrackCount == trackCount);
            };
        }

        private static Predicate<Artist> ByArtistName(string name)
        {
            return delegate (Artist artist)
            {
                return (artist.Name == name);
            };
        }

        private static Predicate<Genre> ByGenreName(string name)
        {
            return delegate (Genre genre)
            {
                return (genre.Name == name);
            };
        }
    }
}
using Player.Core.Entities;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
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

        private ApplicationContext database = new ApplicationContext();
        private List<Album> albums { get; set; } = new List<Album>();
        private List<Artist> artists { get; set; } = new List<Artist>();
        private List<Genre> genres { get; set; } = new List<Genre>();

        private BackgroundWorker worker = new BackgroundWorker();

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
            // ✅ TODO: дописать класс для интеграции с UI 
            worker.WorkerReportsProgress = true;
            worker.DoWork += (object sender, DoWorkEventArgs e) => LoadFiles(pathToLibrary);
            worker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => ProgressPercentage = e.ProgressPercentage;
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                ProgressPercentage = 100;
                database.SaveChanges();
                database.Dispose();
                Settings.IsMusicLoaded = true;
                Settings.Save();
                Log.Information("Скан библиотеки завершен.");
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

            Log.Information("Первичный скан библиотеки завершен. Найдено {Count} файлов.", files.Length);
            foreach (string format in SUPPORTED_FORMATS)
            {
                var filteredFiles = files.Where(s => s.Contains(format));
                Log.Information("Найдено {Count} файлов в формате '{Format}'. Начинаю сканирование и загрузку этих файлов в БД.",
                    filteredFiles.Count(), format);

                foreach (string file in filteredFiles)
                {
                    LoadNewFile(file);
                    progress++;
                    int percent = Convert.ToInt32((double)progress / (double)goal * 100);
                    worker.ReportProgress(percent);
                }
            }
        }

        private void LoadNewFile(string fileName)
        {
            var raw = new RawTrack(fileName);
            try
            {
                raw.LoadMetadata();
            }
            catch (UnsupportedFormatException)
            {
                Log.Error("Формат файла '{FileName}' не поддерживается. Пропускаю...", Path.GetFileName(fileName));
                return;
            }
            catch(TagLib.CorruptFileException)
            {
                Log.Error("Файл '{FileName}' поврежден. Пропускаю...", Path.GetFileName(fileName));
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
                Title = raw.Title
            };
            database.Tracks.Add(track);

            var album = FindExistingOrCreateNewAlbum(raw.AlbumTitle, raw.AlbumYear, raw.AlbumTrackCount, raw.AlbumDiscCount);
            album.Tracks.Add(track);

            foreach (string artistName in raw.Artists) FindExistingOrCreateNewArtist(artistName).Tracks.Add(track);
            foreach (string artistName in raw.AlbumArtists) FindExistingOrCreateNewArtist(artistName).Albums.Add(album);
            foreach (string genreName in raw.AlbumGenres) FindExistingOrCreateNewGenre(genreName).Albums.Add(album);
            database.SaveChanges();

            if (raw.AlbumArt != null)
            {
                album.RenderAlbumArtUri(App.ALBUM_ARTS_DIRECTORY);
                SaveAlbumArt(album.AlbumArtUri, raw.AlbumArt);
            }
            else
            {
                // еще не протестировано, не уверен будет ли работать
                // TODO: протестировать (да и вообще переделать к хуям???) при построении UI.
                album.AlbumArtUri = @"pack://application:,,,/Player;component/Resources/Images/album-art-placeholder.jpg";
            }

            _ = raw;
        }

        private Album FindExistingOrCreateNewAlbum(string title, uint year, uint trackCount, uint discCount)
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

        private static void SaveAlbumArt(string path, IPicture art)
        {
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    var ms = new MemoryStream(art.Data.Data);
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ms));
                    using var fileStream = new FileStream(path, FileMode.Create);
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось сохранить обложку альбома по пути: '{path}'", path);
            }
        }
    }
}

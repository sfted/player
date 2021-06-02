using Player.Core.Entities.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media.Imaging;

namespace Player.Core.Entities
{
    public abstract class TrackListWithCover : BaseEntity, IPlayable
    {
        public string Title { get; set; }
        public int? CoverId { get; set; }
        public virtual List<Track> Tracks { get; set; } = new List<Track>();
        public virtual Cover Cover { get; set; }

        private const int TINY_COVER_DIMENSION_SIZE = 32;
        private const int SMALL_COVER_DIMENSION_SIZE = 128;
        private const int MEDIUM_COVER_DIMENSION_SIZE = 256;
        private const int BIG_COVER_DIMENSION_SIZE = 512;
        private const int LARGE_COVER_DIMENSION_SIZE = 1024;

        private BitmapSource coverTine;
        private BitmapSource coverSmall;
        private BitmapSource coverMedium;
        private BitmapSource coverBig;
        private BitmapSource coverLarge;

        [NotMapped]
        public BitmapSource CoverTiny
        {
            get
            {
                if (coverTine == null)
                    coverTine = LoadAlbumArt(CoverId, TINY_COVER_DIMENSION_SIZE);

                return coverTine;
            }
            set
            {
                coverTine = value;
                NotifyPropertyChanged(nameof(CoverTiny));
            }
        }

        [NotMapped]
        public BitmapSource CoverSmall
        {
            get
            {
                if (coverSmall == null)
                    coverSmall = LoadAlbumArt(CoverId, SMALL_COVER_DIMENSION_SIZE);

                return coverSmall;
            }
            set
            {
                coverSmall = value;
                NotifyPropertyChanged(nameof(CoverSmall));
            }
        }

        [NotMapped]
        public BitmapSource CoverMedium
        {
            get
            {
                if (coverMedium == null)
                    coverMedium = LoadAlbumArt(CoverId, MEDIUM_COVER_DIMENSION_SIZE);

                return coverMedium;
            }
            set
            {
                coverMedium = value;
                NotifyPropertyChanged(nameof(CoverMedium));
            }
        }

        [NotMapped]
        public BitmapSource CoverBig
        {
            get
            {
                if (coverBig == null)
                    coverBig = LoadAlbumArt(CoverId, BIG_COVER_DIMENSION_SIZE);

                return coverBig;
            }
            set
            {
                coverBig = value;
                NotifyPropertyChanged(nameof(CoverBig));
            }
        }

        [NotMapped]
        public BitmapSource CoverLarge
        {
            get
            {
                if (coverLarge == null)
                    coverLarge = LoadAlbumArt(CoverId, LARGE_COVER_DIMENSION_SIZE);

                return coverLarge;
            }
            set
            {
                coverLarge = value;
                NotifyPropertyChanged(nameof(CoverLarge));
            }
        }

        private static BitmapImage LoadAlbumArt(int? id, int size)
        {
            if (id != null)
            {
                using var db = new ApplicationContext();

                var data = db.GetAlbumArtDataById((int)id);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(data);
                bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = size;
                bitmap.DecodePixelHeight = size;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
            else return null;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

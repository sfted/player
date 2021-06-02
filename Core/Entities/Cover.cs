using System;
using System.Drawing;
using System.IO;

namespace Player.Core.Entities
{
    public class Cover : BaseEntity
    {
        public byte[] Data { get; set; }

        public static Cover FromFileName(string filename, int thumbnailSize = 1024)
        {
            var fullsizeImage = Image.FromFile(filename);
            var image = fullsizeImage.GetThumbnailImage(thumbnailSize, thumbnailSize, null, IntPtr.Zero);
            byte[] data;
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                data = stream.ToArray();
            }
            return new Cover { Data = data };
        }
    }
}
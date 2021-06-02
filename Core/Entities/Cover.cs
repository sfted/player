using System.Drawing;
using System.IO;

namespace Player.Core.Entities
{
    public class Cover : BaseEntity
    {
        public byte[] Data { get; set; }

        public static Cover FromFileName(string filename)
        {
            Image img = Image.FromFile(filename);
            byte[] data;
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                data = stream.ToArray();
            }
            return new Cover { Data = data };
        }
    }
}
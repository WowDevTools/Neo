using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using WoWEditor6.IO;

namespace WoWEditor6.UI
{
    static class ThumbnailCache
    {
        public static Action<string> ThumnailAdded;

        private static readonly string mFilename = $@"Cache\ThumbCache-{((int)FileManager.Instance.Version).ToString()}.bin";
        private static Dictionary<string, Bitmap> mCache = new Dictionary<string, Bitmap>();
        private const int MaxWidth = 114;
        private const int MaxHeight = 114;

        public static void Read()
        {
            if (!File.Exists(mFilename)) return;

            using (FileStream fs = File.OpenRead(mFilename))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                Func<int, bool> AssertSize = i => reader.BaseStream.Position + i <= reader.BaseStream.Length; //Data overflow check

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string key = reader.ReadCString(); //Filename

                    if (!AssertSize(4)) return;
                    int size = reader.ReadInt32(); //Image byte size

                    if (!AssertSize(size) || size == 0) return;
                    mCache.Add(key, BytesToImage(reader.ReadBytes(size)));
                }
            }
        }

        public static void Write(bool clear = false)
        {
            if (mCache.Count == 0) return;

            if (!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");

            using (FileStream fs = File.OpenWrite(mFilename))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                foreach (var kvp in mCache)
                {
                    writer.Write(Encoding.UTF8.GetBytes(kvp.Key)); //Filename CString
                    writer.Write((byte)0);

                    var img = ImageToBytes(kvp.Value);
                    writer.Write(img.Length); //Image byte length
                    writer.Write(img); //Image bytes
                }
            }

            if (clear)
                mCache.Clear();
        }

        public static void Reload()
        {
            Write(true);
            Read();
        }

        public static void Cache(string filename, Bitmap image, bool force = false)
        {
            if (force || !mCache.ContainsKey(filename))
            {
                mCache.Add(filename, ResizeImage(image));
                ThumnailAdded?.Invoke(filename);
            }                
        }

        public static bool IsCached(string filename)
        {
            return mCache.ContainsKey(filename);
        }

        public static Bitmap TryGetThumbnail(string filename, Bitmap defaultImage)
        {
            if (IsCached(filename))
                return mCache[filename];

            return defaultImage;
        }


        private static byte[] ImageToBytes(Bitmap image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Tiff); //Use Jpeg for smaller images
                byte[] bmpBytes = ms.GetBuffer();
                image.Dispose();
                ms.Close();
                return bmpBytes;
            }
        }

        private static Bitmap BytesToImage(byte[] data)
        {
            ImageConverter converter = new ImageConverter();
            return new Bitmap((Image)converter.ConvertFrom(data));
        }

        private static Bitmap ResizeImage(Bitmap image)
        {
            Bitmap bmp = new Bitmap(MaxWidth, MaxHeight);
            var graph = System.Drawing.Graphics.FromImage(bmp);
            float scale = Math.Min((float)MaxWidth / image.Width, (float)MaxHeight / image.Height);
            if (scale > 1) scale = 1; //Only downsize

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);
            var brush = new SolidBrush(Properties.Settings.Default.AssetRenderBackgroundColor);

            graph.FillRectangle(brush, new RectangleF(0, 0, MaxWidth, MaxHeight));
            graph.DrawImage(image, new Rectangle((MaxWidth - scaleWidth) / 2, (MaxHeight - scaleHeight) / 2, scaleWidth, scaleHeight));

            return bmp;
        }
    }
}

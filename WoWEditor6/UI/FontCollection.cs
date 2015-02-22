using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace WoWEditor6.UI
{
    public static class FontCollection
    {
        private static PrivateFontCollection gFontCollection = new PrivateFontCollection();
        private static List<WeakReference<Font>> gFontsList = new List<WeakReference<Font>>();

        public unsafe static void Initialize()
        {
            gFontCollection = new PrivateFontCollection();
            var fileListing = IO.FileManager.Instance.FileListing;
            foreach (var file in fileListing.RootEntry.Children["Fonts"].Children.Values)
            {
                using (var stream = IO.FileManager.Instance.Provider.OpenFile(@"Fonts\" + file.Name))
                {
                    if (stream == null)
                        continue;

                    var fileBuffer = new byte[stream.Length];
                    stream.Read(fileBuffer, 0, (int)stream.Length);

                    fixed (byte* ptr = fileBuffer)
                        gFontCollection.AddMemoryFont((IntPtr)ptr, (int)stream.Length);

                    Log.Debug("Loaded font file: " + file.Name);
                }
            }
        }

        public static Font GetFont(string name, int height, FontStyle style)
        {
            Font font = null;
            var upperName = name.ToUpperInvariant();

            gFontsList.RemoveAll(fontRef =>
            {
                Font curFont;
                if (!fontRef.TryGetTarget(out curFont))
                    return true;

                if (curFont.Name.ToUpperInvariant() == upperName &&
                    curFont.Height == height && curFont.Style == style)
                    font = curFont;

                return false;
            });

            if (font != null)
                return font;

            font = new Font(name, height, style, GraphicsUnit.Pixel);
            gFontsList.Add(new WeakReference<Font>(font));
            return font;
        }
    }
}

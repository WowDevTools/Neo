using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace WoWEditor6.UI
{
    public static class FontCollection
    {
        private static readonly PrivateFontCollection gCollection = new PrivateFontCollection();
        private static readonly List<WeakReference<Font>> gFontsList = new List<WeakReference<Font>>();

        public unsafe static void Initialize()
        {
            IO.FileManager.Instance.LoadComplete += () =>
            {
                var fileListing = IO.FileManager.Instance.FileListing;
                foreach (var file in fileListing.RootEntry.Children["fonts"].Children.Values)
                {
                    using (var stream = IO.FileManager.Instance.Provider.OpenFile("Fonts\\" + file.Name))
                    {
                        if (stream == null || stream.Length == 0)
                        {
                            Log.Warning("Unable to load font file: " + file.Name);
                            continue;
                        }

                        var fileBuffer = new byte[stream.Length];
                        stream.Read(fileBuffer, 0, (int)stream.Length);

                        fixed (byte* ptr = fileBuffer)
                            gCollection.AddMemoryFont((IntPtr)ptr, fileBuffer.Length);
                    }
                }

                foreach (var font in gCollection.Families)
                    Log.Debug("Loaded font: " + font.Name);
            };
        }

        public static Font GetFont(string name, float size, FontStyle style)
        {
            Font font = null;
            var upperName = name.ToUpperInvariant();

            lock (gFontsList)
            {
                gFontsList.RemoveAll(fontRef =>
                {
                    Font curFont;
                    if (!fontRef.TryGetTarget(out curFont))
                        return true;

                    if (curFont.Name.ToUpperInvariant() == upperName &&
                        curFont.Size == size && curFont.Style == style)
                        font = curFont;

                    return false;
                });

                if (font != null)
                    return font;

                var family = gCollection.Families.FirstOrDefault(
                    f => f.Name.ToUpperInvariant() == upperName);

                if (family == null)
                    family = new FontFamily(name);

                font = new Font(family, size, style, GraphicsUnit.Pixel);
                gFontsList.Add(new WeakReference<Font>(font));
                return font;
            }
        }
    }
}

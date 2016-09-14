using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectWrite;

namespace Neo.UI
{
    class Font
    {
        private TextFormat mFormat;
        public readonly string Family;
        private readonly FontStyle mStyle;
        public readonly FontWeight Weight;
        public readonly float Size;

        public Font(string family, FontStyle style, FontWeight weight, float size)
        {
            Family = family;
            mStyle = style;
            Weight = weight;
            Size = size;
        }

        public void OnUpdateTarget(Factory factory)
        {
            mFormat = new TextFormat(factory, Family, Weight, mStyle, Size);
        }

        public static implicit operator TextFormat(Font font)
        {
            return font.mFormat;
        }
    }

    static class Fonts
    {
        public static FontCollection Cache { get; private set; }

        static Fonts()
        {
            Cache = new FontCollection();
        }

        public static void Initialize(Factory factory)
        {
            Cache.OnUpdateFactory(factory);
        }
    }

    class FontCollection
    {
        private Factory mFactory;
        private readonly List<Font> mFonts = new List<Font>();

        public void OnUpdateFactory(Factory factory)
        {
            mFactory = factory;
            foreach (var font in mFonts)
                font.OnUpdateTarget(factory);
        }

        public Font this[string family, float size, FontWeight weight = FontWeight.Normal]
        {
            get
            {
                var font = mFonts.FirstOrDefault(f => f.Family == family && Math.Abs(f.Size - size) < 1 && f.Weight == weight);
                if (font != null)
                    return font;

                font = new Font(family, FontStyle.Normal, weight, size);
                font.OnUpdateTarget(mFactory);
                mFonts.Add(font);
                return font;
            }
        }
    }
}

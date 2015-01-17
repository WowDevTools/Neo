using System.Collections.Generic;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI
{
    class BrushCollection
    {
        private readonly Dictionary<uint, SolidBrush> mBrushes = new Dictionary<uint, SolidBrush>();
        private RenderTarget mTarget;

        public void OnUpdateTarget(RenderTarget target)
        {
            foreach (var pair in mBrushes)
                pair.Value.OnUpdateBrush(pair.Key, target);

            mTarget = target;
        }

        public SolidBrush this[uint color]
        {
            get
            {
                SolidBrush ret;
                if (mBrushes.TryGetValue(color, out ret))
                    return ret;

                ret = new SolidBrush();
                ret.OnUpdateBrush(color, mTarget);
                mBrushes.Add(color, ret);
                return ret;
            }
        }

        public SolidBrush this[System.Drawing.Color color]
        {
            get
            {
                uint r = color.R;
                uint g = color.G;
                uint b = color.B;
                uint a = color.A;
                return this[(a << 24) | (b << 16) | (g << 8) | r];
            }
        }
    }

    static class Brushes
    {
        public static readonly BrushCollection Solid = new BrushCollection();
        public static SolidBrush White { get; private set; }
        public static SolidBrush Black { get; private set; }
        public static SolidBrush Blue { get; private set; }
        public static SolidBrush Red { get; private set; }
        public static SolidBrush Green { get; private set; }
        public static SolidBrush Yellow { get; private set; }


        public static void Initialize(RenderTarget target)
        {
            Solid.OnUpdateTarget(target);

            if(White == null)
            {
                White = Solid[0xFFFFFFFF];
                Black = Solid[0xFF000000];
                Blue = Solid[0xFFFF0000];
                Red = Solid[0xFF0000FF];
                Green = Solid[0xFF00FF00];
                Yellow = Solid[System.Drawing.Color.Yellow];
            }
        }
    }
}

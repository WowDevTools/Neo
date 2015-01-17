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

        public static void Initialize(RenderTarget target)
        {
            Solid.OnUpdateTarget(target);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2AlphaAnimation
    {
        private M2InterpolateAlpha16AnimationBlock mAlpha;

        public M2AlphaAnimation(M2File file, ref AnimationBlock transBlock, BinaryReader reader)
        {
            mAlpha = new M2InterpolateAlpha16AnimationBlock(file, transBlock, reader, 1.0f);
        }

        public void UpdateValue(int animation, uint time, out float value)
        {
            value = mAlpha.GetValueDefaultLength(animation, time);
        }
    }
}

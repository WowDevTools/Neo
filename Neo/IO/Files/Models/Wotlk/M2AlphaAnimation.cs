using System.IO;

namespace Neo.IO.Files.Models.Wotlk
{
	internal class M2AlphaAnimation
    {
        private readonly M2InterpolateAlpha16AnimationBlock mAlpha;

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

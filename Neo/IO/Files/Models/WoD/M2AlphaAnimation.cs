using System.IO;

namespace Neo.IO.Files.Models.WoD
{
	internal class M2AlphaAnimation
    {
        private readonly M2InterpolateAlpha16AnimationBlock mAlpha;

        public M2AlphaAnimation(M2File file, ref AnimationBlock transBlock, BinaryReader reader)
        {
	        this.mAlpha = new M2InterpolateAlpha16AnimationBlock(file, transBlock, reader, 1.0f);
        }

        public void UpdateValue(int animation, uint time, out float value)
        {
            value = this.mAlpha.GetValueDefaultLength(animation, time);
        }
    }
}

using System.IO;
using OpenTK;

namespace Neo.IO.Files.Models.WoD
{
	internal class M2TexColorAnimation
    {
        private readonly M2Vector3AnimationBlock mColor;
        private readonly M2NoInterpolateAlpha16AnimationBlock mAlpha;

        public M2TexColorAnimation(M2File file, ref M2ColorAnim colorAnim, BinaryReader reader)
        {
	        this.mColor = new M2Vector3AnimationBlock(file, colorAnim.color, reader, Vector3.One);
	        this.mAlpha = new M2NoInterpolateAlpha16AnimationBlock(file, colorAnim.alpha, reader, 1.0f);
        }

        public void UpdateValue(int animation, uint time, out Vector4 value)
        {
            var color = this.mColor.GetValueDefaultLength(animation, time);
            var alpha = this.mAlpha.GetValueDefaultLength(animation, time);

            value = new Vector4(color, alpha);
        }
    }
}

using System.IO;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2TexColorAnimation
    {
        private readonly M2Vector3AnimationBlock mColor;
        private readonly M2NoInterpolateAlpha16AnimationBlock mAlpha;

        public M2TexColorAnimation(M2File file, ref M2ColorAnim colorAnim, BinaryReader reader)
        {
            mColor = new M2Vector3AnimationBlock(file, colorAnim.color, reader, Vector3.One);
            mAlpha = new M2NoInterpolateAlpha16AnimationBlock(file, colorAnim.alpha, reader, 1.0f);
        }

        public void UpdateValue(int animation, uint time, out Vector4 value)
        {
            var color = mColor.GetValueDefaultLength(animation, time);
            var alpha = mAlpha.GetValueDefaultLength(animation, time);

            value = new Vector4(color, alpha);
        }
    }
}

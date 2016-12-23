using System.IO;
using OpenTK;

namespace Neo.IO.Files.Models.Wotlk
{
	internal class M2UVAnimation
    {
        private readonly M2Vector3AnimationBlock mTranslation;
        private readonly M2InvQuaternion16AnimationBlock mRotation;
        private readonly M2Vector3AnimationBlock mScaling;

        public M2UVAnimation(M2File file, ref M2TexAnim texAnim, BinaryReader reader)
        {
            mTranslation = new M2Vector3AnimationBlock(file, texAnim.translation, reader);
            mRotation = new M2InvQuaternion16AnimationBlock(file, texAnim.rotation, reader);
            mScaling = new M2Vector3AnimationBlock(file, texAnim.scaling, reader, Vector3.One);
        }

        public void UpdateMatrix(int animation, uint time, out Matrix4 matrix)
        {
            var position = mTranslation.GetValueDefaultLength(animation, time);
            var scaling = mScaling.GetValueDefaultLength(animation, time);
            var rotation = mRotation.GetValueDefaultLength(animation, time);

            matrix = Matrix4.Rotate(rotation) * Matrix4.Scale(scaling) * Matrix4.Translation(position);
        }
    }
}

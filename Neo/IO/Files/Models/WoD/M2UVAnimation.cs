using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;

namespace Neo.IO.Files.Models.WoD
{
    class M2UVAnimation
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

        public void UpdateMatrix(int animation, uint time, out Matrix matrix)
        {
            var position = mTranslation.GetValueDefaultLength(animation, time);
            var scaling = mScaling.GetValueDefaultLength(animation, time);
            var rotation = mRotation.GetValueDefaultLength(animation, time);

            matrix = Matrix.RotationQuaternion(rotation) * Matrix.Scaling(scaling) * Matrix.Translation(position);
        }
    }
}

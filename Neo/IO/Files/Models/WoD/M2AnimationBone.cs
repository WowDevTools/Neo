using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;

namespace Neo.IO.Files.Models.WoD
{
    class M2AnimationBone
    {
        private readonly M2Bone mBone;
        private readonly Matrix mInvPivot;
        private readonly Matrix mPivot;

        private readonly M2Vector3AnimationBlock mTranslation;
        private readonly M2Quaternion16AnimationBlock mRotation;
        private readonly M2Vector3AnimationBlock mScaling;

        public bool IsBillboarded { get; private set; }

        public bool IsTransformed { get; private set; }

        public M2AnimationBone(M2File file, ref M2Bone bone, BinaryReader reader)
        {
            mBone = bone;
            IsBillboarded = (bone.flags & 0x08) != 0;  // Some billboards have 0x40 for cylindrical?
            IsTransformed = (bone.flags & 0x200) != 0;

            mPivot = Matrix.Translation(bone.pivot);
            mInvPivot = Matrix.Translation(-bone.pivot);

            mTranslation = new M2Vector3AnimationBlock(file, bone.translation, reader);
            mRotation = new M2Quaternion16AnimationBlock(file, bone.rotation, reader, Quaternion.Identity);
            mScaling = new M2Vector3AnimationBlock(file, bone.scaling, reader, Vector3.One);
        }

        public void UpdateMatrix(uint time, int animation, out Matrix matrix,
            M2Animator animator, BillboardParameters billboard)
        {
            var boneMatrix = Matrix.Identity;
            if (IsBillboarded && billboard != null)
            {
                var billboardMatrix = Matrix.Identity;
                billboardMatrix.Row1 = new Vector4(billboard.Forward, 0);
                billboardMatrix.Row2 = new Vector4(billboard.Right, 0);
                billboardMatrix.Row3 = new Vector4(billboard.Up, 0);
                boneMatrix = billboardMatrix * billboard.InverseRotation;
            }

            if (IsTransformed)
            {
                var position = mTranslation.GetValue(animation, time, animator.AnimationLength);
                var scaling = mScaling.GetValue(animation, time, animator.AnimationLength);
                var rotation = mRotation.GetValue(animation, time, animator.AnimationLength);
                boneMatrix *= Matrix.RotationQuaternion(rotation) * Matrix.Scaling(scaling) * Matrix.Translation(position);
            }

            boneMatrix = mInvPivot * boneMatrix * mPivot;

            if (mBone.parentBone >= 0)
                boneMatrix *= animator.GetBoneMatrix(time, mBone.parentBone, billboard);

            matrix = boneMatrix;
        }
    }
}

using System.IO;
using OpenTK;

namespace Neo.IO.Files.Models.WoD
{
	internal class M2AnimationBone
    {
        private readonly M2Bone mBone;
        private readonly Matrix4 mInvPivot;
        private readonly Matrix4 mPivot;

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

            mPivot = Matrix4.Translation(bone.pivot);
            mInvPivot = Matrix4.Translation(-bone.pivot);

            mTranslation = new M2Vector3AnimationBlock(file, bone.translation, reader);
            mRotation = new M2Quaternion16AnimationBlock(file, bone.rotation, reader, Quaternion.Identity);
            mScaling = new M2Vector3AnimationBlock(file, bone.scaling, reader, Vector3.One);
        }

        public void UpdateMatrix(uint time, int animation, out Matrix4 matrix,
            M2Animator animator, BillboardParameters billboard)
        {
            var boneMatrix = Matrix4.Identity;
            if (IsBillboarded && billboard != null)
            {
                var billboardMatrix = Matrix4.Identity;
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
                boneMatrix *= Matrix4.Rotate(rotation) * Matrix4.Scale(scaling) * Matrix4.Translation(position);
            }

            boneMatrix = mInvPivot * boneMatrix * mPivot;

            if (mBone.parentBone >= 0)
            {
	            boneMatrix *= animator.GetBoneMatrix(time, this.mBone.parentBone, billboard);
            }

	        matrix = boneMatrix;
        }
    }
}

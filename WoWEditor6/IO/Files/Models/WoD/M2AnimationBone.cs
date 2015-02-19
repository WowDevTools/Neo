using System.IO;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2AnimationBone
    {
        private readonly Matrix mInvPivot;
        private readonly Matrix mPivot;

        private readonly M2Vector3AnimationBlock mTranslation;
        private readonly M2Quaternion16AnimationBlock mRotation;
        private readonly M2Vector3AnimationBlock mScaling;

        public M2AnimationBone ParentBone { get; set; }

        public M2Bone Bone { get; private set; }

        public bool IsBillboarded { get; private set; }

        public bool IsTransformed { get; private set; }

        public M2AnimationBone(M2File file, ref M2Bone bone, BinaryReader reader)
        {
            Bone = bone;
            IsBillboarded = (bone.flags & 0x08) != 0;
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
            var position = mTranslation.GetValue(animation, time, animator.AnimationLength);
            var scaling = mScaling.GetValue(animation, time, animator.AnimationLength);
            var rotation = mRotation.GetValue(animation, time, animator.AnimationLength);
            var billboardMatrix = Matrix.Identity;

            if (IsBillboarded && billboard != null)
            {
                billboardMatrix.Row1 = new Vector4(billboard.Forward, billboardMatrix.M14);
                billboardMatrix.Row2 = new Vector4(billboard.Right, billboardMatrix.M24);
                billboardMatrix.Row3 = new Vector4(billboard.Up, billboardMatrix.M34);
                billboardMatrix *= billboard.InverseRotation;
            }

            var boneMatrix = mInvPivot * billboardMatrix * Matrix.RotationQuaternion(rotation)
                * Matrix.Scaling(scaling) * Matrix.Translation(position) * mPivot;

            if (IsTransformed && Bone.parentBone >= 0)
                boneMatrix *= animator.GetBoneMatrix(time, Bone.parentBone, billboard);

            matrix = boneMatrix;
        }
    }
}

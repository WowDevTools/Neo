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

        public M2Bone Bone { get; }

        public M2AnimationBone(M2File file, ref M2Bone bone, BinaryReader reader)
        {
            Bone = bone;
            mPivot = Matrix.Translation(bone.pivot);
            mInvPivot = Matrix.Translation(-bone.pivot);

            mTranslation = new M2Vector3AnimationBlock(file, bone.translation, reader);
            mRotation = new M2Quaternion16AnimationBlock(file, bone.rotation, reader, Quaternion.Identity);
            mScaling = new M2Vector3AnimationBlock(file, bone.scaling, reader, Vector3.One);
        }

        public void UpdateMatrix(uint time, int animation, out Matrix matrix, M2Animator animator)
        {
            var position = mTranslation.GetValue(animation, time, animator.AnimationLength);
            var scaling = mScaling.GetValue(animation, time, animator.AnimationLength);
            var rotation = mRotation.GetValue(animation, time, animator.AnimationLength);

            var boneMatrix = mInvPivot * Matrix.RotationQuaternion(rotation) * Matrix.Scaling(scaling) *
                     Matrix.Translation(position) * mPivot;

            if (Bone.parentBone >= 0)
                boneMatrix *= animator.GetBoneMatrix(time, Bone.parentBone);

            matrix = boneMatrix;
        }
    }
}

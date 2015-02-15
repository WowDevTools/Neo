using System.IO;
using SharpDX;
using WoWEditor6.Scene;

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
            M2Animator animator, ref Matrix invRot, ref Matrix view)
        {
            var position = mTranslation.GetValue(animation, time, animator.AnimationLength);
            var scaling = mScaling.GetValue(animation, time, animator.AnimationLength);
            var rotation = mRotation.GetValue(animation, time, animator.AnimationLength);
            var billboardMatrix = Matrix.Identity;

            if (IsBillboarded)
            {
                Vector3 right = new Vector3(view.M11, view.M12, view.M13);
                Vector3 up = new Vector3(view.M21, view.M22, view.M23);
                Vector3 forward = new Vector3(view.M31, view.M32, view.M33);

                billboardMatrix.Row1 = new Vector4(forward, billboardMatrix.M14);
                billboardMatrix.Row2 = new Vector4(right, billboardMatrix.M24);
                billboardMatrix.Row3 = new Vector4(up, billboardMatrix.M34);
                billboardMatrix *= invRot;
            }

            var boneMatrix = mInvPivot * billboardMatrix * Matrix.RotationQuaternion(rotation)
                * Matrix.Scaling(scaling) * Matrix.Translation(position) * mPivot;

            if (IsTransformed && Bone.parentBone >= 0)
                boneMatrix *= animator.GetBoneMatrix(time, Bone.parentBone, ref invRot, ref view);

            matrix = boneMatrix;
        }
    }
}

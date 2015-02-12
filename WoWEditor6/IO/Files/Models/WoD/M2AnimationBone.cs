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

            var boneMatrix = Matrix.RotationQuaternion(rotation) *
                Matrix.Scaling(scaling) * Matrix.Translation(position);

            /*var billboard = (Bone.flags & 0x8) != 0;
            if (billboard)
            {
                Vector3 up = WorldFrame.Instance.ActiveCamera.Up;
                Vector3 right = WorldFrame.Instance.ActiveCamera.Right;

                boneMatrix.M21 = right.X;
                boneMatrix.M22 = right.Y;
                boneMatrix.M23 = right.Z;

                boneMatrix.M31 = up.X;
                boneMatrix.M32 = up.Y;
                boneMatrix.M33 = up.Z;

                // TODO: UpdateMatrix needs to be called every frame to make this work.
                // TODO: Must not rotate this bone in case billboarding was applied
            }*/

            boneMatrix = mInvPivot * boneMatrix * mPivot;

            if (Bone.parentBone >= 0)
                boneMatrix *= animator.GetBoneMatrix(time, Bone.parentBone);

            matrix = boneMatrix;
        }
    }
}

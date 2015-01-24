using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2AnimationBone
    {
        private Matrix mInvPivot;
        private Matrix mPivot;

        private M2Vector3AnimationBlock mTranslation;
        private M2Quaternion16AnimationBlock mRotation;
        private M2Vector3AnimationBlock mScaling;

        public M2AnimationBone ParentBone { get; set; }

        public M2Bone Bone { get; }

        public M2AnimationBone(M2File file, ref M2Bone bone, BinaryReader reader)
        {
            Bone = bone;
            mPivot = Matrix.Translation(bone.pivot);
            mInvPivot = Matrix.Translation(-bone.pivot);

            mTranslation = new M2Vector3AnimationBlock(file, bone.translation, reader);
            mRotation = new M2Quaternion16AnimationBlock(file, bone.rotation, reader);
            mScaling = new M2Vector3AnimationBlock(file, bone.scaling, reader, Vector3.One);
        }

        public void UpdateMatrix(uint time, int animation, out Matrix matrix, M2Animator animator)
        {
            var position = mTranslation.GetValue(animation, time, 0);
            var scaling = mScaling.GetValue(animation, time, 0);
            var rotation = mRotation.GetValue(animation, time, 0);

            var boneMatrix = mInvPivot * Matrix.Scaling(scaling) * Matrix.RotationQuaternion(rotation) *
                     Matrix.Translation(position) * mPivot;

            if (Bone.parentBone >= 0)
                boneMatrix *= animator.GetBoneMatrix(time, Bone.parentBone);

            matrix = boneMatrix;
        }
    }
}

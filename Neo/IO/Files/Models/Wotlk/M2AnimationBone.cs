using System.IO;
using OpenTK;

namespace Neo.IO.Files.Models.Wotlk
{
    public sealed class M2AnimationBone
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
	        this.mBone = bone;
	        this.IsBillboarded = (bone.flags & 0x08) != 0;  // Some billboards have 0x40 for cylindrical?
	        this.IsTransformed = (bone.flags & 0x200) != 0;

            bone.pivot.Y = -bone.pivot.Y;

            this.mPivot = Matrix4.CreateTranslation(bone.pivot);
            this.mInvPivot = Matrix4.CreateTranslation(-bone.pivot);

	        this.mTranslation = new M2Vector3AnimationBlock(file, bone.translation, reader);
	        this.mRotation = new M2Quaternion16AnimationBlock(file, bone.rotation, reader, Quaternion.Identity);
	        this.mScaling = new M2Vector3AnimationBlock(file, bone.scaling, reader, Vector3.One);
        }

        public void UpdateMatrix(uint time, int animation, out Matrix4 matrix,
            M2Animator animator, BillboardParameters billboard)
        {
            var boneMatrix = Matrix4.Identity;
            if (this.IsBillboarded && billboard != null)
            {
                var billboardMatrix = Matrix4.Identity;
                billboardMatrix.Row1 = new Vector4(billboard.Forward, 0);
                billboardMatrix.Row2 = new Vector4(billboard.Right, 0);
                billboardMatrix.Row3 = new Vector4(billboard.Up, 0);
                boneMatrix = billboard.InverseRotation * billboardMatrix;
            }

            if (this.IsTransformed)
            {
                var position = this.mTranslation.GetValue(animation, time, animator.AnimationLength);
                position.Y = -position.Y;

                var scaling = this.mScaling.GetValue(animation, time, animator.AnimationLength);
                var rotation = this.mRotation.GetValue(animation, time, animator.AnimationLength);
                boneMatrix *= Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateScale(scaling) * Matrix4.CreateTranslation(position);
            }

            boneMatrix = this.mInvPivot * boneMatrix * this.mPivot;

            if (this.mBone.parentBone >= 0)
            {
	            boneMatrix *= animator.GetBoneMatrix(time, this.mBone.parentBone, billboard);
            }

	        matrix = boneMatrix;
        }
    }
}

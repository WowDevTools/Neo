using System;
using System.Linq;
using OpenTK;

// ReSharper disable InconsistentlySynchronizedField

namespace Neo.IO.Files.Models.WoD
{
	internal class M2Animator : IM2Animator
    {
        public Matrix4[] BoneMatrices { get; private set; }
        private M2AnimationBone[] mBones;
        private bool[] mBoneCalculated;
        private int mBoneStart;

        public Matrix4[] UvMatrices { get; private set; }
        private M2UVAnimation[] mUvAnimations;
        private int mUvStart;

        public Vector4[] Colors { get; private set; }
        private M2TexColorAnimation[] mTexColorAnimations;
        private int mTexColorStart;

        public float[] Transparencies { get; private set; }
        private M2AlphaAnimation[] mAlphaAnimations;
        private int mAlphaStart;

        private AnimationEntry mAnimation;
        private bool mHasAnimation;
        private bool mIsFinished;
        private int mAnimationId;
        private readonly AnimationEntry[] mAnimations;
        private readonly ushort[] mAnimationIds;
        private readonly short[] mAnimationLookup;
        private bool mIsDirty;

        public uint AnimationLength { get { return this.mAnimation.length; } }

        public M2Animator(M2File file)
        {
	        this.mHasAnimation = false;
	        this.mAnimations = file.Animations;
	        this.mAnimationLookup = file.AnimationLookup;
	        this.mAnimationIds = file.AnimationIds;

            SetBoneData(file.Bones);
            SetUvData(file.UvAnimations);
            SetTexColorData(file.ColorAnimations);
            SetAlphaData(file.Transparencies);
        }

        public bool SetAnimation(uint animation)
        {
            if (this.mAnimations.Length == 0 && this.mAnimationLookup.Length == 0)
            {
	            return false;
            }

	        if (Array.IndexOf(this.mAnimationLookup, (short)animation) >= 0)
            {
	            this.mAnimationId = this.mAnimationLookup[animation];
	            this.mAnimation = this.mAnimations[this.mAnimationId];
	            this.mHasAnimation = true;
                ResetAnimationTimes();
                return true;
            }
            else if (Array.IndexOf(this.mAnimationIds, (ushort)animation) >= 0)
            {
                var anim = this.mAnimations.First(x => x.animationID == animation);
	            this.mAnimationId = anim.animationID;
	            this.mAnimation = anim;
	            this.mHasAnimation = true;
                ResetAnimationTimes();
                return true;
            }
            else
            {
                Log.Warning("Animation not found in model. Skipping");
                return false;
            }
        }

        public bool SetAnimation(Storage.AnimationType animation)
        {
            return SetAnimation((uint) animation);
        }

        public void SetAnimationByIndex(uint index)
        {
            if(index >= this.mAnimations.Length)
            {
                Log.Warning("Tried to access animation by index outside of valid animation range. Ignoring animation");
                return;
            }

	        this.mAnimation = this.mAnimations[index];
	        this.mAnimationId = (int) index;
	        this.mHasAnimation = true;
            ResetAnimationTimes();
        }

        public void Update(BillboardParameters billboard)
        {
            if (this.mHasAnimation == false)
            {
	            return;
            }

	        var now = Environment.TickCount;

            var time = (uint)(now - this.mBoneStart);
            if (time >= this.mAnimation.length && ((this.mAnimation.flags & 0x20) == 0 || this.mAnimation.nextAnimation >= 0))
            {
                if (this.mIsFinished)
                {
                    if (this.mAnimation.nextAnimation < 0 || this.mAnimation.nextAnimation >= this.mAnimations.Length)
                    {
	                    return;
                    }

	                this.mIsFinished = false;
	                this.mBoneStart = now;
	                this.mAnimationId = this.mAnimation.nextAnimation;
	                this.mAnimation = this.mAnimations[this.mAnimationId];
                    return;
                }

                time = this.mAnimation.length;
	            this.mIsFinished = true;
            }
            else
            {
                if (this.mAnimation.length > 0)
                {
	                time %= this.mAnimation.length;
                }
            }

            for (var i = 0; i < this.mBoneCalculated.Length; ++i)
            {
	            this.mBoneCalculated[i] = false;
            }

	        lock(this.mBones)
            {
                for (var i = 0; i < this.mBones.Length; ++i)
                {
                    if (this.mBoneCalculated[i])
                    {
	                    continue;
                    }

	                this.mBones[i].UpdateMatrix(time, this.mAnimationId, out this.BoneMatrices[i], this, billboard);
	                this.mBoneCalculated[i] = true;
                }
            }

            time = (uint)(now - this.mUvStart);
            lock(this.mUvAnimations)
            {
                for (var i = 0; i < this.mUvAnimations.Length; ++i)
                {
	                this.mUvAnimations[i].UpdateMatrix(0, time, out this.UvMatrices[i]);
                }
            }

            time = (uint)(now - this.mTexColorStart);
            lock(this.mTexColorAnimations)
            {
                for (var i = 0; i < this.mTexColorAnimations.Length; ++i)
                {
	                this.mTexColorAnimations[i].UpdateValue(0, time, out this.Colors[i]);
                }
            }

            time = (uint)(now - this.mAlphaStart);
            lock(this.mAlphaAnimations)
            {
                for (var i = 0; i < this.mAlphaAnimations.Length; ++i)
                {
	                this.mAlphaAnimations[i].UpdateValue(0, time, out this.Transparencies[i]);
                }
            }

	        this.mIsDirty = true;
        }

        public Vector4 GetColorValue(int texAnim)
        {
            lock(this.mTexColorAnimations)
            {
                if (texAnim < this.Colors.Length && texAnim >= 0)
                {
	                return this.Colors[texAnim];
                }

	            return Vector4.One;
            }
        }

        public float GetAlphaValue(int alphaAnim)
        {
            lock(this.mAlphaAnimations)
            {
                if (alphaAnim >= 0 && alphaAnim < this.Transparencies.Length)
                {
	                return this.Transparencies[alphaAnim];
                }

	            return 1.0f;
            }
        }

        public bool GetUvAnimMatrix(int animation, out Matrix4 matrix)
        {
            lock(this.mUvAnimations)
            {
                if (animation >= 0 && animation < this.UvMatrices.Length)
                {
                    matrix = this.UvMatrices[animation];
                    return true;
                }

                matrix = Matrix4.Identity;
                return false;
            }
        }

        public bool GetBones(Matrix4[] bones)
        {
            if (this.mIsDirty == false)
            {
	            return false;
            }

	        lock (this.mBones)
            {
                for (var i = 0; i < Math.Min(bones.Length, this.BoneMatrices.Length); ++i)
                {
	                bones[i] = this.BoneMatrices[i];
                }

	            this.mIsDirty = false;
                return true;
            }
        }

        public Matrix4 GetBoneMatrix(int bone, BillboardParameters billboard)
        {
            uint time = (uint)(Environment.TickCount - this.mBoneStart);
            return GetBoneMatrix(time, (short)bone, billboard);
        }

        public Matrix4 GetBoneMatrix(uint time, short bone, BillboardParameters billboard)
        {
            lock(this.mBones)
            {
                if (bone < 0 || bone >= this.mBones.Length)
                {
	                return Matrix4.Identity;
                }

	            if (this.mBoneCalculated[bone])
	            {
		            return this.BoneMatrices[bone];
	            }

	            this.mBones[bone].UpdateMatrix(time, this.mAnimationId, out this.BoneMatrices[bone], this, billboard);
	            this.mBoneCalculated[bone] = true;
                return this.BoneMatrices[bone];
            }
        }

        public void ResetAnimationTimes()
        {
	        this.mBoneStart = Environment.TickCount;
	        this.mUvStart = Environment.TickCount;
	        this.mTexColorStart = Environment.TickCount;
	        this.mAlphaStart = Environment.TickCount;
        }

        private void SetBoneData(M2AnimationBone[] bones)
        {
	        this.mBones = bones;
	        this.mBoneCalculated = new bool[bones.Length];
	        this.BoneMatrices = new Matrix4[bones.Length];
	        this.mBoneStart = Environment.TickCount;
            for (var i = 0; i < bones.Length; ++i)
            {
	            this.BoneMatrices[i] = Matrix4.Identity;
            }

	        this.mIsDirty = true;
        }

        private void SetUvData(M2UVAnimation[] animations)
        {
	        this.mUvAnimations = animations;
	        this.UvMatrices = new Matrix4[animations.Length];
	        this.mUvStart = Environment.TickCount;
        }

        private void SetTexColorData(M2TexColorAnimation[] animations)
        {
	        this.mTexColorAnimations = animations;
	        this.Colors = new Vector4[animations.Length];
	        this.mTexColorStart = Environment.TickCount;
        }

        private void SetAlphaData(M2AlphaAnimation[] animations)
        {
	        this.mAlphaAnimations = animations;
	        this.Transparencies = new float[animations.Length];
	        this.mAlphaStart = Environment.TickCount;
        }
    }
}

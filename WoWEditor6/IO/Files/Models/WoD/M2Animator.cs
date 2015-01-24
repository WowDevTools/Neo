using SharpDX;
using System;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2Animator
    {
        public Matrix[] BoneMatrices { get; private set; }
        private M2AnimationBone[] mBones;
        private bool[] mBoneCalculated;
        private int mBoneStart;

        public Matrix[] UvMatrices { get; private set; }
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
        private readonly short[] mAnimationLookup;

        public uint AnimationLength => mAnimation.length;

        public M2Animator(M2File file)
        {
            mHasAnimation = false;
            mAnimations = file.Animations;
            mAnimationLookup = file.AnimLookup;
        }

        public void SetAnimation(uint animation)
        {
            if(animation >= mAnimationLookup.Length)
            {
                Log.Warning("Tried to access animation by id outside of the lookup array. Ignoring animation");
                return;
            }

            if(mAnimationLookup[animation] < 0)
            {
                Log.Warning("Animation not found in model. Skipping");
                return;
            }

            mAnimationId = mAnimationLookup[animation];
            mAnimation = mAnimations[mAnimationId];
            mHasAnimation = true;
            ResetAnimationTimes();
        }

        public void SetAnimationByIndex(int index)
        {
            if(index >= mAnimations.Length)
            {
                Log.Warning("Tried to access animation by index outside of valid animation range. Ignoring animation");
                return;
            }

            mAnimation = mAnimations[index];
            mAnimationId = index;
            mHasAnimation = true;
            ResetAnimationTimes();
        }

        public Vector4 GetColorValue(int texAnim)
        {
            if (texAnim < Colors.Length && texAnim >= 0)
                return Colors[texAnim];

            return Vector4.One;
        }

        public float GetAlphaValue(int alphaAnim)
        {
            if (alphaAnim >= 0 && alphaAnim < Transparencies.Length)
                return Transparencies[alphaAnim];

            return 1.0f;
        }

        public Matrix GetUvAnimMatrix(int animation)
        {
            if (animation >= 0 && animation < UvMatrices.Length)
                return UvMatrices[animation];

            return Matrix.Identity;
        }

        public Matrix GetBoneMatrix(uint time, short bone)
        {
            if (bone < 0 || bone >= mBones.Length)
                return Matrix.Identity;

            if (mBoneCalculated[bone])
                return BoneMatrices[bone];

            mBones[bone].UpdateMatrix(time, mAnimationId, out BoneMatrices[bone], this);
            mBoneCalculated[bone] = true;
            return BoneMatrices[bone];
        }

        public void SetBoneData(M2AnimationBone[] bones)
        {
            mBones = bones;
            mBoneCalculated = new bool[bones.Length];
            BoneMatrices = new Matrix[bones.Length];
            mBoneStart = Environment.TickCount;
        }

        public void SetUvData(M2UVAnimation[] animations)
        {
            mUvAnimations = animations;
            UvMatrices = new Matrix[animations.Length];
            mUvStart = Environment.TickCount;
        }

        public void SetTexColorData(M2TexColorAnimation[] animations)
        {
            mTexColorAnimations = animations;
            Colors = new Vector4[animations.Length];
            mTexColorStart = Environment.TickCount;
        }

        public void SetAlphaData(M2AlphaAnimation[] animations)
        {
            mAlphaAnimations = animations;
            Transparencies = new float[animations.Length];
            mAlphaStart = Environment.TickCount;
        }

        public void Update()
        {
            if (mHasAnimation == false)
                return;

            var now = Environment.TickCount;

            var time = (uint)(now - mBoneStart);
            if(time >= mAnimation.length && ((mAnimation.flags & 0x20) == 0 || mAnimation.nextAnimation >= 0))
            {
                if(mIsFinished)
                {
                    if (mAnimation.nextAnimation < 0 || mAnimation.nextAnimation >= mAnimations.Length) return;

                    mIsFinished = false;
                    mBoneStart = now;
                    mAnimationId = mAnimation.nextAnimation;
                    mAnimation = mAnimations[mAnimationId];
                    return;
                }

                time = mAnimation.length;
                mIsFinished = true;
            }
            else
                time %= mAnimation.length;

            for (var i = 0; i < mBoneCalculated.Length; ++i)
                mBoneCalculated[i] = false;

            for(var i = 0; i < mBones.Length; ++i)
            {
                if (mBoneCalculated[i])
                    continue;

                mBones[i].UpdateMatrix(time, mAnimationId, out BoneMatrices[i], this);
                mBoneCalculated[i] = true;
            }

            time = (uint) (now - mUvStart);
            for(var i = 0; i < mUvAnimations.Length; ++i)
                mUvAnimations[i].UpdateMatrix(mAnimationId, time, out UvMatrices[i]);

            time = (uint) (now - mTexColorStart);
            for (var i = 0; i < mTexColorAnimations.Length; ++i)
                mTexColorAnimations[i].UpdateValue(mAnimationId, time, out Colors[i]);

            time = (uint) (now - mAlphaStart);
            for (var i = 0; i < mAlphaAnimations.Length; ++i)
                mAlphaAnimations[i].UpdateValue(mAnimationId, time, out Transparencies[i]);
        }

        private void ResetAnimationTimes()
        {
            mBoneStart = Environment.TickCount;
            mUvStart = Environment.TickCount;
            mTexColorStart = Environment.TickCount;
            mAlphaStart = Environment.TickCount;
        }
    }
}

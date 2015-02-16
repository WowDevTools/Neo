using SharpDX;
using System;
// ReSharper disable InconsistentlySynchronizedField

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2Animator : IM2Animator
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
        private bool mIsDirty;

        public uint AnimationLength { get { return mAnimation.length; } }

        public M2Animator(M2File file)
        {
            mHasAnimation = false;
            mAnimations = file.Animations;
            mAnimationLookup = file.AnimLookup;

            SetBoneData(file.Bones);
            SetUvData(file.UvAnimations);
            SetTexColorData(file.ColorAnimations);
            SetAlphaData(file.Transparencies);
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

        public void SetAnimationByIndex(uint index)
        {
            if(index >= mAnimations.Length)
            {
                Log.Warning("Tried to access animation by index outside of valid animation range. Ignoring animation");
                return;
            }

            mAnimation = mAnimations[index];
            mAnimationId = (int) index;
            mHasAnimation = true;
            ResetAnimationTimes();
        }

        public void Update(BillboardParameters billboard)
        {
            if (mHasAnimation == false)
                return;

            var now = Environment.TickCount;

            var time = (uint)(now - mBoneStart);
            if (time >= mAnimation.length && ((mAnimation.flags & 0x20) == 0 || mAnimation.nextAnimation >= 0))
            {
                if (mIsFinished)
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
            {
                if (mAnimation.length > 0)
                    time %= mAnimation.length;
            }

            for (var i = 0; i < mBoneCalculated.Length; ++i)
                mBoneCalculated[i] = false;

            lock(mBones)
            {
                for (var i = 0; i < mBones.Length; ++i)
                {
                    if (mBoneCalculated[i])
                        continue;

                    mBones[i].UpdateMatrix(time, mAnimationId, out BoneMatrices[i], this, billboard);
                    mBoneCalculated[i] = true;
                }
            }

            time = (uint)(now - mUvStart);
            lock(mUvAnimations)
            {
                for (var i = 0; i < mUvAnimations.Length; ++i)
                    mUvAnimations[i].UpdateMatrix(mAnimationId, time, out UvMatrices[i]);
            }

            time = (uint)(now - mTexColorStart);
            lock(mTexColorAnimations)
            {
                for (var i = 0; i < mTexColorAnimations.Length; ++i)
                    mTexColorAnimations[i].UpdateValue(mAnimationId, time, out Colors[i]);
            }

            time = (uint)(now - mAlphaStart);
            lock(mAlphaAnimations)
            {
                for (var i = 0; i < mAlphaAnimations.Length; ++i)
                    mAlphaAnimations[i].UpdateValue(mAnimationId, time, out Transparencies[i]);
            }

            mIsDirty = true;
        }

        public Vector4 GetColorValue(int texAnim)
        {
            lock(mTexColorAnimations)
            {
                if (texAnim < Colors.Length && texAnim >= 0)
                    return Colors[texAnim];

                return Vector4.One;
            }
        }

        public float GetAlphaValue(int alphaAnim)
        {
            lock(mAlphaAnimations)
            {
                if (alphaAnim >= 0 && alphaAnim < Transparencies.Length)
                    return Transparencies[alphaAnim];

                return 1.0f;
            }
        }

        public bool GetUvAnimMatrix(int animation, out Matrix matrix)
        {
            lock(mUvAnimations)
            {
                if (animation >= 0 && animation < UvMatrices.Length)
                {
                    matrix = UvMatrices[animation];
                    return true;
                }

                matrix = Matrix.Identity;
                return false;
            }
        }

        public bool GetBones(Matrix[] bones)
        {
            if (mIsDirty == false)
                return false;

            lock (mBones)
            {
                for (var i = 0; i < Math.Min(bones.Length, BoneMatrices.Length); ++i)
                    bones[i] = BoneMatrices[i];

                mIsDirty = false;
                return true;
            }
        }

        public Matrix GetBoneMatrix(int bone, BillboardParameters billboard)
        {
            uint time = (uint)(Environment.TickCount - mBoneStart);
            return GetBoneMatrix(time, (short)bone, billboard);
        }

        public Matrix GetBoneMatrix(uint time, short bone, BillboardParameters billboard)
        {
            lock(mBones)
            {
                if (bone < 0 || bone >= mBones.Length)
                    return Matrix.Identity;

                if (mBoneCalculated[bone])
                    return BoneMatrices[bone];

                mBones[bone].UpdateMatrix(time, mAnimationId, out BoneMatrices[bone], this, billboard);
                mBoneCalculated[bone] = true;
                return BoneMatrices[bone];
            }
        }

        public void ResetAnimationTimes()
        {
            mBoneStart = Environment.TickCount;
            mUvStart = Environment.TickCount;
            mTexColorStart = Environment.TickCount;
            mAlphaStart = Environment.TickCount;
        }

        private void SetBoneData(M2AnimationBone[] bones)
        {
            mBones = bones;
            mBoneCalculated = new bool[bones.Length];
            BoneMatrices = new Matrix[bones.Length];
            mBoneStart = Environment.TickCount;
            for (var i = 0; i < bones.Length; ++i)
                BoneMatrices[i] = Matrix.Identity;
        }

        private void SetUvData(M2UVAnimation[] animations)
        {
            mUvAnimations = animations;
            UvMatrices = new Matrix[animations.Length];
            mUvStart = Environment.TickCount;
        }

        private void SetTexColorData(M2TexColorAnimation[] animations)
        {
            mTexColorAnimations = animations;
            Colors = new Vector4[animations.Length];
            mTexColorStart = Environment.TickCount;
        }

        private void SetAlphaData(M2AlphaAnimation[] animations)
        {
            mAlphaAnimations = animations;
            Transparencies = new float[animations.Length];
            mAlphaStart = Environment.TickCount;
        }
    }
}

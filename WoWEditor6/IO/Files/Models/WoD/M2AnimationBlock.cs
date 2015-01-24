using System.IO;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    interface IInterpolator<TSource, out TDest>
    {
        TDest Interpolate(float fac, ref TSource v1, ref TSource v2);
    }

    class DefaultInterpolator<TSource, TDest> : IInterpolator<TSource, TDest>
    {
        public TDest Interpolate(float fac, ref TSource v1, ref TSource v2)
        {
            return default(TDest);
        }
    }

    class VectorInterpolator : IInterpolator<Vector3, Vector3>, IInterpolator<Vector4, Vector4>
    {
        public Vector3 Interpolate(float fac, ref Vector3 v1, ref Vector3 v2)
        {
            return v1 * (1.0f - fac) + fac * v2;
        }

        public Vector4 Interpolate(float fac, ref Vector4 v1, ref Vector4 v2)
        {
            return v1 * (1.0f - fac) + fac * v2;
        }
    }

    class M2Vector3AnimationBlock : M2AnimationBlock<Vector3, Vector3, VectorInterpolator>
    {
        public M2Vector3AnimationBlock(M2File file, ref AnimationBlock data, BinaryReader reader, Vector3 defaultValue = default(Vector3))
            : base(file, ref data, reader, defaultValue)
        {

        }
    }

    class M2Vector4AnimationBlock : M2AnimationBlock<Vector4, Vector4, VectorInterpolator>
    {
        public M2Vector4AnimationBlock(M2File file, ref AnimationBlock data, BinaryReader reader, Vector4 defaultValue = default(Vector4))
            : base(file, ref data, reader, defaultValue)
        {

        }
    }

    class M2AnimationBlock<TSource, TDest, TInterpolator> where TInterpolator : IInterpolator<TSource, TDest>, new()
    {
        private static TInterpolator mInterpolator = new TInterpolator();

        private AnimationBlock mFileBlock;
        private TDest mDefaultValue;
        private uint mGlobalSequence = 0;
        private bool mHasGlobalSequence;

        public M2AnimationBlock(M2File file, ref AnimationBlock data, BinaryReader reader, TDest defaultValue = default(TDest))
        {
            if (data.globalSequence >= 0 && data.globalSequence < file.GlobalSequences.Length)
            {
                mGlobalSequence = file.GlobalSequences[data.globalSequence];
                mHasGlobalSequence = true;
            }

            mDefaultValue = defaultValue;
            mFileBlock = data;

            Load(reader);
        }

        private void Load(BinaryReader reader)
        {

        }
    }
}

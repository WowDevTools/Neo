using System;
using System.IO;
using OpenTK;

namespace Neo.IO.Files.Models.WoD
{
    class M2AnimationBlock<TSource, TDest, TInterpolator> where TInterpolator : IInterpolator<TSource, TDest>, new() where TSource : struct
    {
        private static readonly TInterpolator Interpolator = new TInterpolator();

        private AnimationBlock mFileBlock;
        private readonly TDest mDefaultValue;
        private readonly uint mGlobalSequence;
        private readonly bool mHasGlobalSequence;

        private uint[][] mTimestamps = new uint[0][];
        private TSource[][] mValues = new TSource[0][];

        public M2AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, TDest defaultValue = default(TDest))
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

        public TDest GetValue(int timeline, uint time, uint length)
        {
            if (timeline >= mTimestamps.Length || timeline >= mValues.Length)
            {
	            return this.mDefaultValue;
            }

	        var tl = mTimestamps[timeline];
            var values = mValues[timeline];

            if (tl.Length == 0 || values.Length == 0)
            {
	            return this.mDefaultValue;
            }

	        if (mHasGlobalSequence && mGlobalSequence > 0)
	        {
		        time %= this.mGlobalSequence;
	        }
	        else
            {
                var ltime = tl[tl.Length - 1];
                if (ltime != 0)
                {
	                time %= ltime;
                }
            }

            var maxIndex = Math.Min(tl.Length, values.Length);
            if (maxIndex == 1)
            {
	            return Interpolator.Interpolate(ref values[0]);
            }

	        if (time == tl[maxIndex - 1])
	        {
		        return Interpolator.Interpolate(ref values[maxIndex - 1]);
	        }

	        if(time >= tl[maxIndex - 1])
            {
                var te = tl[0] + length;
                var ts = tl[maxIndex - 1];
                var fac = (time - ts) / (float) (te - ts);
                return Interpolator.Interpolate(fac, ref values[maxIndex - 1], ref values[0]);
            }

            if (time <= tl[0])
            {
	            return Interpolator.Interpolate(ref values[0]);
            }

	        return InterpolateValue(time, timeline);
        }

        public TDest GetValueDefaultLength(int timeline, uint timeFull)
        {
            if (timeline >= mTimestamps.Length || timeline >= mValues.Length)
            {
	            return this.mDefaultValue;
            }

	        var tl = mTimestamps[timeline];
            var values = mValues[timeline];

            if (tl.Length == 0 || values.Length == 0)
            {
	            return this.mDefaultValue;
            }

	        if (mHasGlobalSequence && mGlobalSequence > 0)
	        {
		        timeFull %= this.mGlobalSequence;
	        }
	        else
            {
                var ltime = tl[tl.Length - 1];
                if (ltime != 0)
                {
	                timeFull %= ltime;
                }
            }

            return InterpolateValue(timeFull, timeline);
        }

        private TDest InterpolateValue(uint time, int timeline)
        {
            var tl = mTimestamps[timeline];
            var values = mValues[timeline];

            var maxIndex = Math.Min(tl.Length, values.Length);

            int istart;
            var iend = 0;

            var found = false;

            for (istart = 0; istart < maxIndex - 1; ++istart)
            {
                if (tl[istart] <= time && tl[istart + 1] >= time)
                {
                    iend = istart + 1;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
	            return Interpolator.Interpolate(ref values[istart]);
            }

	        var ts = tl[istart];
            var te = tl[iend];

            if(ts > te)
            {
                var tmp = ts;
                ts = te;
                te = tmp;
            }

            if (ts > time)
            {
	            ts = time;
            }
	        if (te < time)
	        {
		        te = time;
	        }

	        var fac = (time - ts) / (float)(te - ts);
            return Interpolator.Interpolate(fac, ref values[istart], ref values[iend]);
        }

        private void Load(BinaryReader reader)
        {
            mTimestamps = new uint[mFileBlock.numTimeStamps][];
            mValues = new TSource[mFileBlock.numValues][];

            reader.BaseStream.Position = mFileBlock.ofsTimeStamps;
            var timeStampEntries = reader.ReadArray<ulong>(mFileBlock.numTimeStamps);

            for (var i = 0; i < mFileBlock.numTimeStamps; ++i)
            {
                var count = timeStampEntries[i] & 0xFFFFFFFF;
                var offset = (uint)(timeStampEntries[i] >> 32);
                reader.BaseStream.Position = offset;
                mTimestamps[i] = reader.ReadArray<uint>((int) count);
            }

            reader.BaseStream.Position = mFileBlock.ofsValues;
            var valueEntries = reader.ReadArray<ulong>(mFileBlock.numValues);

            for(var i = 0; i < mFileBlock.numValues; ++i)
            {
                var count = valueEntries[i] & 0xFFFFFFFF;
                var offset = (uint)(valueEntries[i] >> 32);
                reader.BaseStream.Position = offset;
                mValues[i] = reader.ReadArray<TSource>((int) count);
            }
        }
    }

    class M2Vector2AnimationBlock : M2AnimationBlock<Vector2, Vector2, VectorInterpolator>
    {
        public M2Vector2AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, Vector2 defaultValue = default(Vector2))
            : base(file, data, reader, defaultValue)
        {

        }
    }

    class M2Vector3AnimationBlock : M2AnimationBlock<Vector3, Vector3, VectorInterpolator>
    {
        public M2Vector3AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, Vector3 defaultValue = default(Vector3))
            : base(file, data, reader, defaultValue)
        {

        }
    }

    class M2Vector4AnimationBlock : M2AnimationBlock<Vector4, Vector4, VectorInterpolator>
    {
        public M2Vector4AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, Vector4 defaultValue = default(Vector4))
            : base(file, data, reader, defaultValue)
        {

        }
    }

    class M2Quaternion16AnimationBlock : M2AnimationBlock<Quaternion16, Quaternion, QuaternionInterpolator>
    {
        public M2Quaternion16AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, Quaternion defaultValue)
            : base(file, data, reader, defaultValue)
        {

        }

        public M2Quaternion16AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader)
            : base(file, data, reader, Quaternion.Identity)
        {

        }
    }

    class M2InvQuaternion16AnimationBlock : M2AnimationBlock<InvQuaternion16, Quaternion, QuaternionInterpolator>
    {
        public M2InvQuaternion16AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, Quaternion defaultValue = default(Quaternion))
            : base(file, data, reader, defaultValue)
        {

        }

        public M2InvQuaternion16AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader)
            : base(file, data, reader, Quaternion.Identity)
        {

        }
    }

    class M2NoInterpolateAlpha16AnimationBlock : M2AnimationBlock<short, float, NoInterpolateAlpha16>
    {
        public M2NoInterpolateAlpha16AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, float defaultValue = default(float))
            : base(file, data, reader, defaultValue)
        {

        }
    }

    class M2InterpolateAlpha16AnimationBlock : M2AnimationBlock<short, float, InterpolateAlpha16>
    {
        public M2InterpolateAlpha16AnimationBlock(M2File file, AnimationBlock data, BinaryReader reader, float defaultValue = default(float))
            : base(file, data, reader, defaultValue)
        {

        }
    }
}

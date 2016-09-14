using System;
using System.Collections.Generic;
using SharpDX;

namespace Neo.IO.Files.Sky.WoD
{
    class MapLight
    {
        private readonly LightEntryData mEntry;
        private LightParamsEntry mParams;

        private readonly List<LightDataEntry> mDataEntries = new List<LightDataEntry>();
        private LightDataEntry[] mEntryArray = new LightDataEntry[0];

        public bool IsZoneLight { get; private set; }
        public int SkyId { get { return mParams.Id; } }
        public Vector3 Position { get { return mEntry.Position; } }
        public float OuterRadius { get { return mEntry.OuterRadius; } }
        public float InnerRadius { get { return mEntry.InnerRadius; } }
        public bool IsGlobal { get { return (InnerRadius < 0.01f && OuterRadius < 0.01f); } }
        public int LightId { get { return mEntry.Id; } }

        public MapLight(LightEntryData entry, ref LightParamsEntry paramsEntry)
        {
            IsZoneLight = false;
            mEntry = entry;
            mParams = paramsEntry;
        }

        public bool GetColorForTime(int time, LightColor colorType, ref Vector3 color)
        {
            if (mDataEntries.Count == 0)
                return false;

            if (mEntryArray.Length != mDataEntries.Count)
                mEntryArray = mDataEntries.ToArray();

            if (mEntryArray.Length == 1)
            {
                color = ToRgb(colorType, ref mEntryArray[0]);
                return true;
            }

            var maxTime = mEntryArray[mEntryArray.Length - 1].timeValues;
            if (maxTime == 0 || mEntryArray[0].timeValues > time)
            {
                color = ToRgb(colorType, ref mEntryArray[0]);
                return true;
            }

            time %= 2880;

            var eIndex1 = -1;
            var eIndex2 = -1;
            var hasLight = false;
            var t1 = 1u;
            var t2 = 1u;

            for (var i = 0; i < mEntryArray.Length; ++i)
            {
                if (i + 1 >= mEntryArray.Length)
                {
                    eIndex1 = i;
                    eIndex2 = 0;
                    hasLight = true;
                    t1 = mEntryArray[eIndex1].timeValues;
                    t2 = mEntryArray[eIndex2].timeValues + 2880;
                    break;
                }

                if (mEntryArray[i].timeValues > time || mEntryArray[i + 1].timeValues <= time) continue;

                eIndex1 = i;
                eIndex2 = i + 1;
                hasLight = true;
                t1 = mEntryArray[eIndex1].timeValues;
                t2 = mEntryArray[eIndex2].timeValues;
                break;
            }

            if (hasLight == false)
                return false;

            if (t1 >= t2)
            {
                color = ToRgb(colorType, ref mEntryArray[eIndex1]);
                return true;
            }

            var diff = t2 - t1;
            var sat = (time - t1) / (float)diff;
            var v1 = ToRgb(colorType, ref mEntryArray[eIndex1]);
            var v2 = ToRgb(colorType, ref mEntryArray[eIndex2]);
            color = v2 * sat + v1 * (1 - sat);

            return true;
        }

        public bool GetAllColorsForTime(int time, Color3[] colors)
        {
            if (mDataEntries.Count == 0)
                return false;

            if (mEntryArray.Length != mDataEntries.Count)
                mEntryArray = mDataEntries.ToArray();

            if (mEntryArray.Length == 1)
            {
                for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
                    colors[i] = ToRgb((LightColor)i, ref mEntryArray[0]);

                return true;
            }

            var maxTime = mEntryArray[mEntryArray.Length - 1].timeValues;
            if (maxTime == 0 || mEntryArray[0].timeValues > time)
            {
                for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
                    colors[i] = ToRgb((LightColor)i, ref mEntryArray[0]);

                return true;
            }

            time %= 2880;

            var eIndex1 = -1;
            var eIndex2 = -1;
            var hasLight = false;
            var t1 = 1u;
            var t2 = 1u;

            for (var i = 0; i < mEntryArray.Length; ++i)
            {
                if (i + 1 >= mEntryArray.Length)
                {
                    eIndex1 = i;
                    eIndex2 = 0;
                    hasLight = true;
                    t1 = mEntryArray[eIndex1].timeValues;
                    t2 = mEntryArray[eIndex2].timeValues + 2880;
                    break;
                }

                if (mEntryArray[i].timeValues > time || mEntryArray[i + 1].timeValues <= time) continue;

                eIndex1 = i;
                eIndex2 = i + 1;
                hasLight = true;
                t1 = mEntryArray[eIndex1].timeValues;
                t2 = mEntryArray[eIndex2].timeValues;
                break;
            }

            if (hasLight == false)
                return false;

            if (t1 >= t2)
            {
                for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
                    colors[i] = ToRgb((LightColor)i, ref mEntryArray[eIndex1]);

                return true;
            }

            var diff = t2 - t1;
            var sat = (time - t1) / (float)diff;

            for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
            {
                var v1 = ToRgb((LightColor)i, ref mEntryArray[eIndex1]);
                var v2 = ToRgb((LightColor)i, ref mEntryArray[eIndex2]);
                colors[i] = v2 * sat + v1 * (1 - sat);
            }

            return true;
        }

        public bool GetAllFloatsForTime(int time, float[] floats)
        {
            if (mDataEntries.Count == 0)
                return false;

            if (mEntryArray.Length != mDataEntries.Count)
                mEntryArray = mDataEntries.ToArray();

            if(mEntryArray.Length == 1)
            {
                for (var i = 0; i < (int) LightFloat.MaxLightFloat; ++i)
                    floats[i] = ToFloat((LightFloat) i, ref mEntryArray[0]);

                return true;
            }

            var maxTime = mEntryArray[mEntryArray.Length - 1].timeValues;
            if(maxTime == 0 || mEntryArray[0].timeValues > time)
            {
                for (var i = 0; i < (int)LightFloat.MaxLightFloat; ++i)
                    floats[i] = ToFloat((LightFloat)i, ref mEntryArray[0]);

                return true;
            }

            time %= 2880;

            var eIndex1 = -1;
            var eIndex2 = -1;
            var hasLight = false;
            var t1 = 1u;
            var t2 = 1u;

            for(var i = 0; i < mEntryArray.Length; ++i)
            {
                if(i + 1 >= mEntryArray.Length)
                {
                    eIndex1 = i;
                    eIndex2 = 0;
                    hasLight = true;
                    t1 = mEntryArray[eIndex1].timeValues;
                    t2 = mEntryArray[eIndex2].timeValues + 2880;
                    break;
                }

                if (mEntryArray[i].timeValues > time || mEntryArray[i + 1].timeValues <= time) continue;

                eIndex1 = i;
                eIndex2 = i + 1;
                hasLight = true;
                t1 = mEntryArray[eIndex1].timeValues;
                t2 = mEntryArray[eIndex2].timeValues;
                break;
            }

            if (hasLight == false)
                return false;

            if(t1 >= t2)
            {
                for (var i = 0; i < (int) LightFloat.MaxLightFloat; ++i)
                    floats[i] = ToFloat((LightFloat) i, ref mEntryArray[eIndex1]);

                return true;
            }

            var diff = t2 - t1;
            var sat = (time - t1) / (float) diff;

            for (var i = 0; i < (int)LightFloat.MaxLightFloat; ++i)
            {
                var v1 = ToFloat((LightFloat) i, ref mEntryArray[eIndex1]);
                var v2 = ToFloat((LightFloat) i, ref mEntryArray[eIndex2]);
                floats[i] = v2 * sat + v1 * (1 - sat);
            }

            return true;
        }

        public void AddDataEntry(ref LightDataEntry e)
        {
            mDataEntries.Add(e);
        }

        public void AddAllData(IEnumerable<LightDataEntry> e)
        {
            mDataEntries.AddRange(e);
            mDataEntries.Sort(
                (e1, e2) => (e1.timeValues < e2.timeValues) ? -1 : ((e1.timeValues > e2.timeValues) ? 1 : 0));
        }

        static void ToRgb(uint value, ref Vector3 color)
        {
            color.X = ((value & 0x00FF0000) >> 16) / 255.0f;
            color.Y = ((value & 0x0000FF00) >> 8) / 255.0f;
            color.Z = ((value & 0x000000FF) >> 0) / 255.0f;
        }

        Vector3 ToRgb(LightColor colorType, ref LightDataEntry e)
        {
            var ret = new Vector3();
            switch(colorType)
            {
                case LightColor.Ambient:
                    ToRgb(e.globalAmbient, ref ret);
                    break;

                case LightColor.Diffuse:
                    ToRgb(e.globalDiffuse, ref ret);
                    break;

                case LightColor.Top:
                    ToRgb(e.skyColor0, ref ret);
                    break;

                case LightColor.Middle:
                    ToRgb(e.skyColor1, ref ret);
                    break;

                case LightColor.MiddleLower:
                    ToRgb(e.skyColor2, ref ret);
                    break;

                case LightColor.Lower:
                    ToRgb(e.skyColor3, ref ret);
                    break;

                case LightColor.Horizon:
                    ToRgb(e.skyColor4, ref ret);
                    break;

                case LightColor.Fog:
                    ToRgb(e.fogColor, ref ret);
                    break;

                case LightColor.Sun:
                    ToRgb(e.sunColor, ref ret);
                    break;

                case LightColor.Halo:
                    ToRgb(e.haloColor, ref ret);
                    break;

                case LightColor.Cloud:
                    ToRgb(e.cloudColor, ref ret);
                    break;

                default:
                    throw new ArgumentException("Invalid light type");
            }

            return ret;
        }

        float ToFloat(LightFloat type, ref LightDataEntry e)
        {
            switch(type)
            {
                case LightFloat.FogDensity:
                    return e.fogDensity;

                case LightFloat.FogEnd:
                    return e.fogEnd;

                case LightFloat.FogScale:
                    return e.fogScaler;

                default:
                    throw new ArgumentException("Light type not supported yet or invalid");
            }
        }
    }
}

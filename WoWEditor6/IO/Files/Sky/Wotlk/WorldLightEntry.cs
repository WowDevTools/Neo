using System;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.IO.Files.Sky.Wotlk
{
    class WorldLightEntry
    {
        private readonly DbcRecord mLight;
        private readonly List<Vector3>[] mColorTables = new List<Vector3>[18];
        private readonly List<uint>[] mTimeTables = new List<uint>[18];

        private readonly List<float>[] mFloatTables = new List<float>[2];
        private readonly List<uint>[] mFloatTimes = new List<uint>[2];

        public bool IsGlobal { get; private set; }
        public float InnerRadius { get; private set; }
        public float OuterRadius { get; private set; }
        public Vector3 Position { get; private set; }

        public WorldLightEntry(DbcRecord lightEntry)
        {
            mLight = lightEntry;

            var px = mLight.GetFloat(2);
            var py = mLight.GetFloat(3);
            var pz = mLight.GetFloat(4);
            var ir = mLight.GetFloat(5);
            var or = mLight.GetFloat(6);

            px /= 36.0f;
            py /= 36.0f;
            pz /= 36.0f;
            ir /= 36.0f;
            or /= 36.0f;

            IsGlobal = Math.Abs(px) < 1e-3 && Math.Abs(py) < 1e-3 && Math.Abs(pz) < 1e-3;
            InnerRadius = ir;
            OuterRadius = or;

            Position = new Vector3(pz + Metrics.MapMidPoint, px  + Metrics.MapMidPoint, py);

            for (var i = 0; i < 18; ++i)
            {
                mColorTables[i] = new List<Vector3>();
                mTimeTables[i] = new List<uint>();
            }

            for (var i = 0; i < 2; ++i)
            {
                mFloatTables[i] = new List<float>();
                mFloatTimes[i] = new List<uint>();
            }

            InitTables();
        }

        public float GetFloatForTime(LightFloat table, uint time)
        {
            int idx;
            switch (table)
            {
                case LightFloat.FogEnd:
                    idx = 0;
                    break;

                case LightFloat.FogScale:
                    idx = 1;
                    break;

                default:
                    return 1.0f;
            }

            if (idx < 0 || idx >= 2)
                return 0.0f;

            var timeValues = mFloatTimes[idx];
            var colorValues = mFloatTables[idx];
            if (timeValues.Count == 0)
                return 0.0f;

            if (timeValues[0] > time)
                time = timeValues[0];

            if (timeValues.Count == 1)
                return colorValues[0];

            var v1 = 0.0f;
            var v2 = 0.0f;

            uint t1 = 0;
            uint t2 = 0;

            for (var i = 0; i < timeValues.Count; ++i)
            {
                if (i + 1 >= timeValues.Count)
                {
                    v1 = colorValues[i];
                    v2 = colorValues[0];
                    t1 = timeValues[i];
                    t2 = timeValues[0] + 2880;
                    break;
                }

                var ts = timeValues[i];
                var te = timeValues[i + 1];
                if (ts <= time && te >= time)
                {
                    t1 = ts;
                    t2 = te;
                    v1 = colorValues[i];
                    v2 = colorValues[i + 1];
                    break;
                }
            }

            var diff = t2 - t1;
            if (diff == 0)
                return v1;

            var sat = (time - t1) / (float)diff;
            return (1 - sat) * v1 + sat * v2;
        }

        public Vector3 GetColorForTime(LightColor table, uint time)
        {
            var idx = (int)table;
            time %= 2880;

            if (idx < 0 || idx >= 18)
                return Vector3.Zero;

            var timeValues = mTimeTables[idx];
            var colorValues = mColorTables[idx];
            if (timeValues.Count == 0)
                return Vector3.Zero;

            if (timeValues[0] > time)
                time = timeValues[0];

            if (timeValues.Count == 1)
                return colorValues[0];

            var v1 = Vector3.Zero;
            var v2 = Vector3.Zero;

            uint t1 = 0;
            uint t2 = 0;

            for (var i = 0; i < timeValues.Count; ++i)
            {
                if (i + 1 >= timeValues.Count)
                {
                    v1 = colorValues[i];
                    v2 = colorValues[0];
                    t1 = timeValues[i];
                    t2 = timeValues[0] + 2880;
                    break;
                }

                var ts = timeValues[i];
                var te = timeValues[i + 1];
                if (ts <= time && te >= time)
                {
                    t1 = ts;
                    t2 = te;
                    v1 = colorValues[i];
                    v2 = colorValues[i + 1];
                    break;
                }
            }

            var diff = t2 - t1;
            if (diff == 0)
                return v1;

            var sat = (time - t1) / (float)diff;
            return (1 - sat) * v1 + sat * v2;
        }

        private void InitTables()
        {
            var baseIndex = mLight.GetInt32(7) * 18;
            for (var i = 0; i < 18; ++i)
            {
                var lib = Storage.DbcStorage.LightIntBand.GetRowById(baseIndex + i - 17);
                var numEntries = lib.GetInt32(1);
                for (var j = 0; j < numEntries; ++j)
                {
                    mColorTables[i].Add(ToVector(lib.GetUint32(18 + j)));
                    mTimeTables[i].Add(lib.GetUint32(2 + j));
                }
            }

            baseIndex = mLight.GetInt32(7) * 6;
            for (var i = 0; i < 2; ++i)
            {
                var lfb = Storage.DbcStorage.LightFloatBand.GetRowById(baseIndex + i - 5);
                var numEntries = lfb.GetInt32(1);
                for (var j = 0; j < numEntries; ++j)
                {
                    mFloatTables[i].Add(lfb.GetFloat(18 + j));
                    mFloatTimes[i].Add(lfb.GetUint32(2 + j));
                }
            }
        }

        private static Vector3 ToVector(uint value)
        {
            return new Vector3(((value >> 16) & 0xFF) / 255.0f, ((value >> 8) & 0xFF) / 255.0f, ((value >> 0) & 0xFF) / 255.0f);
        }
    }
}

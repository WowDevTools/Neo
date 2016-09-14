using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.IO.Files.Sky.Wotlk
{
    class MapSky
    {
        private readonly List<WorldLightEntry> mLights = new List<WorldLightEntry>();
        private readonly Vector3[] mColors = new Vector3[18];
        private readonly float[] mFloats = new float[2];

        public MapSky(uint mapId)
        {
            for (var i = 0; i < Storage.DbcStorage.Light.NumRows; ++i)
            {
                var row = Storage.DbcStorage.Light.GetRow(i);
                if (row.GetUint32(1) == mapId)
                    mLights.Add(new WorldLightEntry(row));
            }

            if (mLights.Count == 0 && Storage.DbcStorage.Light.NumRows > 0)
                mLights.Add(new WorldLightEntry(Storage.DbcStorage.Light.GetRow(0)));

            SortLights();
        }

        public Vector3 GetColor(LightColor color)
        {
            var idx = (int)color;
            if (idx < 0 || idx >= 18)
                return Vector3.Zero;

            return mColors[idx];
        }

        public float GetFloat(LightFloat flt)
        {
            var idx = (flt == LightFloat.FogEnd) ? 0 : (flt == LightFloat.FogScale ? 1 : -1);
            return idx < 0 ? 0.0f : mFloats[idx];
        }

        public void Update(Vector3 position, uint time)
        {
            var weights = new float[mLights.Count];
            CalculateWeights(position, weights);

            for (var i = 0; i < 18; ++i)
                mColors[i] = new Vector3(1, 1, 1);

            for (var i = 0; i < 2; ++i)
                mFloats[i] = 0.0f;

            for (var j = 0; j < mLights.Count; ++j)
            {
                if (weights[j] >= 1e-3)
                {
                    for (var k = 0; k < 18; ++k)
                        mColors[k] += mLights[j].GetColorForTime((LightColor)k, time) * weights[j];

                    for (var k = 0; k < 2; ++k)
                        mFloats[k] += mLights[j].GetFloatForTime((LightFloat)k, time) * weights[j];
                }
            }

            for (var i = 0; i < 18; ++i)
                mColors[i] -= new Vector3(1, 1, 1);
        }

        private void CalculateWeights(Vector3 position, float[] w)
        {
            var globals = new List<int>();
            for (var i = w.Length - 1; i >= 0; --i)
            {
                var le = mLights[i];
                if (le.IsGlobal)
                {
                    globals.Add(i);
                    continue;
                }

                var dist = (position - le.Position).Length();
                if (dist < le.InnerRadius)
                {
                    w[i] = 1.0f;
                    for (var j = i + 1; j < w.Length; ++j)
                        w[j] = 0.0f;
                }
                else if (dist < le.OuterRadius)
                {
                    var sat = (dist - le.InnerRadius) / (le.OuterRadius - le.InnerRadius);
                    w[i] = 1 - sat;
                    for (var j = i + 1; j < w.Length; ++j)
                        w[j] *= sat;
                }
                else
                    w[i] = 0;
            }

            var totalW = w.Sum();
            if (totalW >= 1 || globals.Count == 0)
                return;

            var perGlobalW = (1.0f - totalW) / globals.Count;
            foreach (var glob in globals)
                w[glob] = perGlobalW;
        }

        private void SortLights()
        {
            mLights.Sort((l1, l2) =>
            {
                if (l1.IsGlobal && l2.IsGlobal)
                    return 0;

                if (l1.IsGlobal)
                    return -1;

                if (l2.IsGlobal)
                    return 1;

                if (l1.OuterRadius > l2.OuterRadius)
                    return 1;

                if (l2.OuterRadius > l1.OuterRadius)
                    return -1;

                return 0;
            });
        }
    }
}

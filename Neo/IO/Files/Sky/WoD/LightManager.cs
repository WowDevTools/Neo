using System.Collections.Generic;
using System.Linq;
using Neo.Scene;
using OpenTK;

namespace Neo.IO.Files.Sky.WoD
{
    class LightManager : SkyManager
    {
        private readonly MapLightCollection mLightCollection = new MapLightCollection();
        private List<MapLight> mLights = new List<MapLight>();
        private List<ZoneLight> mZoneLights = new List<ZoneLight>();
        private float[] mWeights = new float[0];
        private float[] mZoneWeights = new float[0];

        private readonly object mColorLock = new object();

        private bool mIsDirty;
        private bool mPositionChanged;
        private Vector3 mLastPosition;
        private Vector2 mLastPosition2D;

        private Vector3[] mColorValues = new Vector3[(int)LightColor.MaxLightType];
        private readonly Vector3[] mColorValuesTemp = new Vector3[(int)LightColor.MaxLightType];
        private float[] mFloatValues = new float[(int) LightFloat.MaxLightFloat];
        private readonly float[] mFloatValuesTemp = new float[(int) LightFloat.MaxLightFloat];

        private bool mIsTextureDirty;
        private Graphics.Texture mSkyTexture;
        private readonly uint[] mSkyGraph = new uint[180];

        public override void Initialize()
        {
            mLightCollection.FillLights();
            mSkyTexture = new Graphics.Texture(WorldFrame.Instance.GraphicsContext);
            WorldFrame.Instance.MapManager.SkySphere.UpdateSkyTexture(mSkyTexture);
        }

        public override void OnEnterWorld(int mapId)
        {
            mLights = mLightCollection.GetLightsForMap(mapId);
            if (mLightCollection.HasZoneLights(mapId))
                mZoneLights = mLightCollection.GetZoneLightsForMap(mapId);
            else
            {
                mZoneLights = new List<ZoneLight>();
                mZoneWeights = new float[0];
            }

            mWeights = new float[mLights.Count];
            mZoneWeights = new float[mZoneLights.Count];

            mLights.Sort((l1, l2) =>
            {
                if (l1.IsGlobal && l2.IsGlobal)
                    return 0;

                if (l1.IsGlobal)
                    return -1;

                if (l2.IsGlobal)
                    return 1;

                return (l1.OuterRadius < l2.OuterRadius) ? -1 : 1;
            });
        }

        public override void AsyncUpdate()
        {
            if (mPositionChanged)
            {
                mPositionChanged = false;

                var globalLights = new List<int>();
                for (var i = 0; i < mWeights.Length; ++i)
                    mWeights[i] = 0.0f;

                for(var index = mLights.Count - 1; index >= 0; --index)
                {
                    var light = mLights[index];
                    if(light.IsGlobal)
                    {
                        globalLights.Add(index);
                        continue;
                    }

                    var pos = light.Position;
                    var inner = light.InnerRadius;
                    inner *= inner;
                    var outer = light.OuterRadius;
                    outer *= outer;

                    var dx = pos.X - mLastPosition.X;
                    var dy = pos.Z - mLastPosition.Y;
                    var diff = dx * dx + dy * dy;
                    if(diff <= inner)
                    {
                        mWeights[index] = 1.0f;
                        for (var j = index + 1; j < mWeights.Length; ++j)
                            mWeights[j] = 0.0f;
                    }
                    else if (diff <= outer)
                    {
                        var sat = (diff - inner) / (outer - inner);
                        mWeights[index] = 1.0f - sat;
                        for (var j = index + 1; j < mWeights.Length; ++j)
                            mWeights[j] *= sat;
                    }
                    else
                        mWeights[index] = 0.0f;
                }

                var totalW = mWeights.Sum();
                var fullLight = 0;
                var hasFullLight = false;
                var partialLights = new List<int>();
                for(var i = 0; i < mZoneLights.Count; ++i)
                {
                    var zl = mZoneLights[i];
                    if (zl.GetDistance(ref mLastPosition2D) < 50.0f)
                        partialLights.Add(i);
                    else if (zl.IsInside(ref mLastPosition2D))
                    {
                        fullLight = i;
                        hasFullLight = true;
                    }
                    else
                        mZoneWeights[i] = 0.0f;
                }

                if (hasFullLight)
                    mZoneWeights[fullLight] = 1.0f;

                for(var i = 0; i < mZoneLights.Count; ++i)
                {
                    if (hasFullLight == false || (i != fullLight))
                        mZoneWeights[i] = 0.0f;
                }

                foreach(var i in partialLights)
                {
                    var zl = mZoneLights[i];
                    var dist = zl.GetDistance(ref mLastPosition2D);
                    var inner = zl.IsInside(ref mLastPosition2D);
                    if (inner) dist = 50 - dist;
                    else dist += 50.0f;

                    var sat = dist / 100.0f;
                    mZoneWeights[i] = 1.0f - sat;
                    for (var j = 0; j < i; ++j)
                        mZoneWeights[j] *= sat;

                    if (hasFullLight && fullLight > i)
                        mZoneWeights[fullLight] *= sat;
                }

                var fac = 1.0f - totalW;
                for(var i = 0; i < mZoneLights.Count; ++i)
                {
                    mZoneWeights[i] *= fac;
                    totalW += mZoneWeights[i];
                }

                var remain = 1.0f - totalW;
                if(remain > 1e-5)
                {
                    var perGlobalW = remain / globalLights.Count;
                    foreach (var gl in globalLights)
                        mWeights[gl] = perGlobalW;
                }

                if (mZoneLights.Count > 0 && mWeights.Length == 0 && partialLights.Count == 0 && hasFullLight == false)
                    mZoneWeights[0] = 1.0f;
            }

            LoadColors();
        }

        public override void UpdatePosition(Vector3 position)
        {
            mLastPosition = position;
            mLastPosition2D = new Vector2(position.X, position.Y);
            mPositionChanged = true;
        }

        public override void SyncUpdate()
        {
            if (mLights.Count == 0)
                return;

            if(mIsTextureDirty)
            {
                mIsTextureDirty = false;
                mSkyTexture.UpdateMemory(1, 180, SharpDX.DXGI.Format.B8G8R8A8_UNorm, mSkyGraph, 4);
            }

            if (mIsDirty == false)
                return;

            lock(mColorLock)
            {
                mIsDirty = false;
            }
        }

        private void LoadColors()
        {
            var time = Utils.TimeManager.Instance.GetTime();
            var ms = (int) (time.TotalMilliseconds * Properties.Settings.Default.DayNightScaling / 10.0f);
            if (Properties.Settings.Default.UseDayNightCycle == false)
                ms = Properties.Settings.Default.DefaultDayTime;

            for (var i = 0; i < mColorValuesTemp.Length; ++i)
                mColorValuesTemp[i] = new Vector3(1, 1, 1);

            for (var i = 0; i < mFloatValuesTemp.Length; ++i)
                mFloatValuesTemp[i] = 1.0f;

            var curColors = new Vector3[(int) LightColor.MaxLightType];
            var curFloats = new float[(int) LightFloat.MaxLightFloat];

            for(var j = 0; j < mLights.Count && j < mWeights.Length; ++j)
            {
                if (mWeights[j] <= 1e-5)
                    continue;

                mLights[j].GetAllColorsForTime(ms, curColors);
                mLights[j].GetAllFloatsForTime(ms, curFloats);

                for (var i = 0; i < curColors.Length; ++i)
                {
                    curColors[i] *= mWeights[j];
                    mColorValuesTemp[i] += curColors[i];
                }

                for (var i = 0; i < curFloats.Length; ++i)
                {
                    curFloats[i] *= mWeights[j];
                    mFloatValuesTemp[i] += curFloats[i];
                }
            }

            for (var j = 0; j < mZoneLights.Count && j < mZoneWeights.Length; ++j)
            {
                if (mZoneWeights[j] <= 1e-5)
                    continue;

                mZoneLights[j].Light.GetAllColorsForTime(ms, curColors);
                mZoneLights[j].Light.GetAllFloatsForTime(ms, curFloats);

                for (var i = 0; i < curColors.Length; ++i)
                {
                    curColors[i] *= mZoneWeights[j];
                    mColorValuesTemp[i] += curColors[i];
                }

                for (var i = 0; i < curFloats.Length; ++i)
                {
                    curFloats[i] *= mZoneWeights[j];
                    mFloatValuesTemp[i] += curFloats[i];
                }
            }

            for (var i = 0; i < mColorValuesTemp.Length; ++i)
                mColorValuesTemp[i] -= new Vector3(1, 1, 1);
            for (var i = 0; i < mFloatValuesTemp.Length; ++i)
                mFloatValuesTemp[i] -= 1.0f;

            if(mWeights.Length == 0 && mZoneWeights.Length == 0)
            {
                mColorValuesTemp[(int) LightColor.Top] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.Middle] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.MiddleLower] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.Lower] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.Horizon] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.Fog] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.Diffuse] = new Vector3(0.25f, 0.5f, 1.0f);
                mColorValuesTemp[(uint) LightColor.Ambient] = new Vector3(0.3f, 0.3f, 0.3f);
                mFloatValues[(uint) LightFloat.FogDensity] = 0.66f;
                mFloatValues[(uint) LightFloat.FogEnd] = 200.0f;
            }

            lock(mColorLock)
            {
                mColorValues = mColorValuesTemp;
                mFloatValues = mFloatValuesTemp;
                mIsDirty = true;
            }

            UpdateSkyTexture();

            var fogStart = mFloatValues[(int) LightFloat.FogScale] * mFloatValues[(int) LightFloat.FogEnd];
            fogStart /= 72.0f;

            WorldFrame.Instance.UpdateMapAmbient(mColorValues[(int) LightColor.Ambient]);
            WorldFrame.Instance.UpdateMapDiffuse(mColorValues[(int) LightColor.Diffuse]);
            WorldFrame.Instance.UpdateFogParams(mColorValues[(int) LightColor.Fog], fogStart);
        }

        private void UpdateSkyTexture()
        {
            var top = mColorValues[(int) LightColor.Top];
            var middle = mColorValues[(int) LightColor.Middle];
            var middleLower = mColorValues[(int) LightColor.MiddleLower];
            var lower = mColorValues[(int) LightColor.Lower];
            var horizon = mColorValues[(int) LightColor.Horizon];
            var fog = mColorValues[(int) LightColor.Fog];

            for (var i = 0; i < 80; ++i)
                mSkyGraph[i] = ToRgbx(ref fog);

            for (var i = 80; i < 90; ++i)
            {
                var sat = (i - 80) / 10.0f;
                var clr = fog + (horizon - fog) * sat;
                mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 90; i < 95; ++i)
            {
                var sat = (i - 90) / 5.0f;
                var clr = horizon + (lower - horizon) * sat;
                mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 95; i < 105; ++i)
            {
                var sat = (i - 95) / 10.0f;
                var clr = lower + (middleLower - lower) * sat;
                mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 105; i < 120; ++i)
            {
                var sat = (i - 105) / 15.0f;
                var clr = middleLower + (middle - middleLower) * sat;
                mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 120; i < 180; ++i)
            {
                var sat = (i - 120) / 60.0f;
                var clr = middle + (top - middle) * sat;
                mSkyGraph[i] = ToRgbx(ref clr);
            }

            mIsTextureDirty = true;
        }

        private static uint ToRgbx(ref Vector3 value)
        {
            var r = (uint)(value.X * 255.0f);
            var g = (uint)(value.Y * 255.0f);
            var b = (uint)(value.Z * 255.0f);
            const uint a = 0xFFu;

            return r | (g << 8) | (b << 16) | (a << 24);
        }
    }
}

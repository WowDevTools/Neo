using System.Collections.Generic;
using System.Linq;
using Neo.Scene;
using OpenTK;

namespace Neo.IO.Files.Sky.WoD
{
	internal class LightManager : SkyManager
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
	        this.mLightCollection.FillLights();
	        this.mSkyTexture = new Graphics.Texture();
            WorldFrame.Instance.MapManager.SkySphere.UpdateSkyTexture(this.mSkyTexture);
        }

        public override void OnEnterWorld(int mapId)
        {
	        this.mLights = this.mLightCollection.GetLightsForMap(mapId);
            if (this.mLightCollection.HasZoneLights(mapId))
            {
	            this.mZoneLights = this.mLightCollection.GetZoneLightsForMap(mapId);
            }
            else
            {
	            this.mZoneLights = new List<ZoneLight>();
	            this.mZoneWeights = new float[0];
            }

	        this.mWeights = new float[this.mLights.Count];
	        this.mZoneWeights = new float[this.mZoneLights.Count];

	        this.mLights.Sort((l1, l2) =>
            {
                if (l1.IsGlobal && l2.IsGlobal)
                {
	                return 0;
                }

	            if (l1.IsGlobal)
	            {
		            return -1;
	            }

	            if (l2.IsGlobal)
	            {
		            return 1;
	            }

	            return (l1.OuterRadius < l2.OuterRadius) ? -1 : 1;
            });
        }

        public override void AsyncUpdate()
        {
            if (this.mPositionChanged)
            {
	            this.mPositionChanged = false;

                var globalLights = new List<int>();
                for (var i = 0; i < this.mWeights.Length; ++i)
                {
	                this.mWeights[i] = 0.0f;
                }

	            for(var index = this.mLights.Count - 1; index >= 0; --index)
                {
                    var light = this.mLights[index];
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

                    var dx = pos.X - this.mLastPosition.X;
                    var dy = pos.Z - this.mLastPosition.Y;
                    var diff = dx * dx + dy * dy;
                    if(diff <= inner)
                    {
	                    this.mWeights[index] = 1.0f;
                        for (var j = index + 1; j < this.mWeights.Length; ++j)
                        {
	                        this.mWeights[j] = 0.0f;
                        }
                    }
                    else if (diff <= outer)
                    {
                        var sat = (diff - inner) / (outer - inner);
	                    this.mWeights[index] = 1.0f - sat;
                        for (var j = index + 1; j < this.mWeights.Length; ++j)
                        {
	                        this.mWeights[j] *= sat;
                        }
                    }
                    else
                    {
	                    this.mWeights[index] = 0.0f;
                    }
                }

                var totalW = this.mWeights.Sum();
                var fullLight = 0;
                var hasFullLight = false;
                var partialLights = new List<int>();
                for(var i = 0; i < this.mZoneLights.Count; ++i)
                {
                    var zl = this.mZoneLights[i];
                    if (zl.GetDistance(ref this.mLastPosition2D) < 50.0f)
                    {
	                    partialLights.Add(i);
                    }
                    else if (zl.IsInside(ref this.mLastPosition2D))
                    {
                        fullLight = i;
                        hasFullLight = true;
                    }
                    else
                    {
	                    this.mZoneWeights[i] = 0.0f;
                    }
                }

                if (hasFullLight)
                {
	                this.mZoneWeights[fullLight] = 1.0f;
                }

	            for(var i = 0; i < this.mZoneLights.Count; ++i)
                {
                    if (hasFullLight == false || (i != fullLight))
                    {
	                    this.mZoneWeights[i] = 0.0f;
                    }
                }

                foreach(var i in partialLights)
                {
                    var zl = this.mZoneLights[i];
                    var dist = zl.GetDistance(ref this.mLastPosition2D);
                    var inner = zl.IsInside(ref this.mLastPosition2D);
                    if (inner)
                    {
	                    dist = 50 - dist;
                    }
                    else
                    {
	                    dist += 50.0f;
                    }

	                var sat = dist / 100.0f;
	                this.mZoneWeights[i] = 1.0f - sat;
                    for (var j = 0; j < i; ++j)
                    {
	                    this.mZoneWeights[j] *= sat;
                    }

	                if (hasFullLight && fullLight > i)
	                {
		                this.mZoneWeights[fullLight] *= sat;
	                }
                }

                var fac = 1.0f - totalW;
                for(var i = 0; i < this.mZoneLights.Count; ++i)
                {
	                this.mZoneWeights[i] *= fac;
                    totalW += this.mZoneWeights[i];
                }

                var remain = 1.0f - totalW;
                if(remain > 1e-5)
                {
                    var perGlobalW = remain / globalLights.Count;
                    foreach (var gl in globalLights)
                    {
	                    this.mWeights[gl] = perGlobalW;
                    }
                }

                if (this.mZoneLights.Count > 0 && this.mWeights.Length == 0 && partialLights.Count == 0 && hasFullLight == false)
                {
	                this.mZoneWeights[0] = 1.0f;
                }
            }

            LoadColors();
        }

        public override void UpdatePosition(Vector3 position)
        {
	        this.mLastPosition = position;
	        this.mLastPosition2D = new Vector2(position.X, position.Y);
	        this.mPositionChanged = true;
        }

        public override void SyncUpdate()
        {
            if (this.mLights.Count == 0)
            {
	            return;
            }

	        if(this.mIsTextureDirty)
            {
	            this.mIsTextureDirty = false;
	            // TODO: Recreate texture from bitmap
	            // mSkyTexture.UpdateMemory(1, 180, SharpDX.DXGI.Format.B8G8R8A8_UNorm, mSkyGraph, 4);
            }

            if (this.mIsDirty == false)
            {
	            return;
            }

	        lock(this.mColorLock)
            {
	            this.mIsDirty = false;
            }
        }

        private void LoadColors()
        {
            var time = Utils.TimeManager.Instance.GetTime();
            var ms = (int) (time.TotalMilliseconds * Properties.Settings.Default.DayNightScaling / 10.0f);
            if (Properties.Settings.Default.UseDayNightCycle == false)
            {
	            ms = Properties.Settings.Default.DefaultDayTime;
            }

	        for (var i = 0; i < this.mColorValuesTemp.Length; ++i)
	        {
		        this.mColorValuesTemp[i] = new Vector3(1, 1, 1);
	        }

	        for (var i = 0; i < this.mFloatValuesTemp.Length; ++i)
	        {
		        this.mFloatValuesTemp[i] = 1.0f;
	        }

	        var curColors = new Vector3[(int) LightColor.MaxLightType];
            var curFloats = new float[(int) LightFloat.MaxLightFloat];

            for(var j = 0; j < this.mLights.Count && j < this.mWeights.Length; ++j)
            {
                if (this.mWeights[j] <= 1e-5)
                {
	                continue;
                }

	            this.mLights[j].GetAllColorsForTime(ms, curColors);
	            this.mLights[j].GetAllFloatsForTime(ms, curFloats);

                for (var i = 0; i < curColors.Length; ++i)
                {
                    curColors[i] *= this.mWeights[j];
	                this.mColorValuesTemp[i] += curColors[i];
                }

                for (var i = 0; i < curFloats.Length; ++i)
                {
                    curFloats[i] *= this.mWeights[j];
	                this.mFloatValuesTemp[i] += curFloats[i];
                }
            }

            for (var j = 0; j < this.mZoneLights.Count && j < this.mZoneWeights.Length; ++j)
            {
                if (this.mZoneWeights[j] <= 1e-5)
                {
	                continue;
                }

	            this.mZoneLights[j].Light.GetAllColorsForTime(ms, curColors);
	            this.mZoneLights[j].Light.GetAllFloatsForTime(ms, curFloats);

                for (var i = 0; i < curColors.Length; ++i)
                {
                    curColors[i] *= this.mZoneWeights[j];
	                this.mColorValuesTemp[i] += curColors[i];
                }

                for (var i = 0; i < curFloats.Length; ++i)
                {
                    curFloats[i] *= this.mZoneWeights[j];
	                this.mFloatValuesTemp[i] += curFloats[i];
                }
            }

            for (var i = 0; i < this.mColorValuesTemp.Length; ++i)
            {
	            this.mColorValuesTemp[i] -= new Vector3(1, 1, 1);
            }
	        for (var i = 0; i < this.mFloatValuesTemp.Length; ++i)
	        {
		        this.mFloatValuesTemp[i] -= 1.0f;
	        }

	        if(this.mWeights.Length == 0 && this.mZoneWeights.Length == 0)
            {
	            this.mColorValuesTemp[(int) LightColor.Top] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.Middle] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.MiddleLower] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.Lower] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.Horizon] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.Fog] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.Diffuse] = new Vector3(0.25f, 0.5f, 1.0f);
	            this.mColorValuesTemp[(uint) LightColor.Ambient] = new Vector3(0.3f, 0.3f, 0.3f);
	            this.mFloatValues[(uint) LightFloat.FogDensity] = 0.66f;
	            this.mFloatValues[(uint) LightFloat.FogEnd] = 200.0f;
            }

            lock(this.mColorLock)
            {
	            this.mColorValues = this.mColorValuesTemp;
	            this.mFloatValues = this.mFloatValuesTemp;
	            this.mIsDirty = true;
            }

            UpdateSkyTexture();

            var fogStart = this.mFloatValues[(int) LightFloat.FogScale] * this.mFloatValues[(int) LightFloat.FogEnd];
            fogStart /= 72.0f;

            WorldFrame.Instance.UpdateMapAmbient(this.mColorValues[(int) LightColor.Ambient]);
            WorldFrame.Instance.UpdateMapDiffuse(this.mColorValues[(int) LightColor.Diffuse]);
            WorldFrame.Instance.UpdateFogParams(this.mColorValues[(int) LightColor.Fog], fogStart);
        }

        private void UpdateSkyTexture()
        {
            var top = this.mColorValues[(int) LightColor.Top];
            var middle = this.mColorValues[(int) LightColor.Middle];
            var middleLower = this.mColorValues[(int) LightColor.MiddleLower];
            var lower = this.mColorValues[(int) LightColor.Lower];
            var horizon = this.mColorValues[(int) LightColor.Horizon];
            var fog = this.mColorValues[(int) LightColor.Fog];

            for (var i = 0; i < 80; ++i)
            {
	            this.mSkyGraph[i] = ToRgbx(ref fog);
            }

	        for (var i = 80; i < 90; ++i)
            {
                var sat = (i - 80) / 10.0f;
                var clr = fog + (horizon - fog) * sat;
	            this.mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 90; i < 95; ++i)
            {
                var sat = (i - 90) / 5.0f;
                var clr = horizon + (lower - horizon) * sat;
	            this.mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 95; i < 105; ++i)
            {
                var sat = (i - 95) / 10.0f;
                var clr = lower + (middleLower - lower) * sat;
	            this.mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 105; i < 120; ++i)
            {
                var sat = (i - 105) / 15.0f;
                var clr = middleLower + (middle - middleLower) * sat;
	            this.mSkyGraph[i] = ToRgbx(ref clr);
            }

            for (var i = 120; i < 180; ++i)
            {
                var sat = (i - 120) / 60.0f;
                var clr = middle + (top - middle) * sat;
	            this.mSkyGraph[i] = ToRgbx(ref clr);
            }

	        this.mIsTextureDirty = true;
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

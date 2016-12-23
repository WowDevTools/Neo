using System;
using System.Collections.Generic;
using Neo.Scene;
using OpenTK;

namespace Neo.IO.Files.Sky.Wotlk
{
	internal class LightManager : SkyManager
    {
        private readonly Dictionary<uint, MapSky> mSkies = new Dictionary<uint, MapSky>();
        private readonly Vector3[] mCurColors = new Vector3[18];
        private readonly float[] mCurFloats = new float[2];

        private MapSky mActiveSky;
        private bool mIsTextureDirty;
        private Graphics.Texture mSkyTexture;
        private readonly uint[] mSkyGraph = new uint[180];

        private Vector3 mLastPosition;

        public override void OnEnterWorld(int mapId)
        {
            mSkies.TryGetValue((uint)mapId, out mActiveSky);
        }

        public override void AsyncUpdate()
        {
            if (mActiveSky == null)
            {
	            return;
            }

	        var time = Utils.TimeManager.Instance.GetTime();
            var ms = (uint)(time.TotalMilliseconds * Properties.Settings.Default.DayNightScaling / 10.0f);
            if (Properties.Settings.Default.UseDayNightCycle == false)
            {
	            ms = (uint)Properties.Settings.Default.DefaultDayTime;
            }

	        mActiveSky.Update(mLastPosition, ms);
            for (var i = 0; i < 18; ++i)
            {
	            this.mCurColors[i] = this.mActiveSky.GetColor((LightColor)i);
            }

	        mCurFloats[0] = mActiveSky.GetFloat(LightFloat.FogEnd);
            mCurFloats[1] = mActiveSky.GetFloat(LightFloat.FogScale);

            var fogStart = mCurFloats[1] * mCurFloats[0];
            fogStart /= 72.0f;
            fogStart = Math.Max(fogStart, 0);

            WorldFrame.Instance.UpdateFogParams(mCurColors[(int)LightColor.Fog], fogStart);
            WorldFrame.Instance.UpdateMapAmbient(mCurColors[(int)LightColor.Ambient]);
            WorldFrame.Instance.UpdateMapDiffuse(mCurColors[(int)LightColor.Diffuse]);

            UpdateSkyTexture();
        }

        public override void UpdatePosition(Vector3 position)
        {
            mLastPosition = position;
        }

        public override void SyncUpdate()
        {
            if (mActiveSky == null)
            {
	            return;
            }

	        if (mIsTextureDirty)
            {
                mIsTextureDirty = false;
	            // TODO: Recreate texture from bitmap
	            // mSkyTexture.UpdateMemory(1, 180, SharpDX.DXGI.Format.B8G8R8A8_UNorm, mSkyGraph, 4);
            }
        }

        public override void Initialize()
        {
            mSkyTexture = new Graphics.Texture();
            WorldFrame.Instance.MapManager.SkySphere.UpdateSkyTexture(mSkyTexture);

            for (var i = 0; i < Storage.DbcStorage.Map.NumRows; ++i)
            {
                var id = Storage.DbcStorage.Map.GetRow(i).GetUint32(0);
                mSkies.Add(id, new MapSky(id));
            }
        }

        private void UpdateSkyTexture()
        {
            var top = mCurColors[(int)LightColor.Top];
            var middle = mCurColors[(int)LightColor.Middle];
            var middleLower = mCurColors[(int)LightColor.MiddleLower];
            var lower = mCurColors[(int)LightColor.Lower];
            var horizon = mCurColors[(int)LightColor.Horizon];
            var fog = mCurColors[(int)LightColor.Fog];

            for (var i = 0; i < 80; ++i)
            {
	            this.mSkyGraph[i] = ToRgbx(ref fog);
            }

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

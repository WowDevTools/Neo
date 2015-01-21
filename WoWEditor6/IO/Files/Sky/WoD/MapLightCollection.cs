using System.Collections.Generic;

namespace WoWEditor6.IO.Files.Sky.WoD
{
    class MapLightCollection
    {
        private readonly Dictionary<int, List<MapLight>> mLights = new Dictionary<int, List<MapLight>>();
        private readonly Dictionary<int, List<ZoneLight>> mZoneLights = new Dictionary<int, List<ZoneLight>>();

        public List<MapLight> GetLightsForMap(int mapid)
        {
            List<MapLight> ret;
            return mLights.TryGetValue(mapid, out ret) ? ret : new List<MapLight>();
        }

        public List<ZoneLight> GetZoneLightsForMap(int mapid)
        {
            List<ZoneLight> ret;
            return mZoneLights.TryGetValue(mapid, out ret) ? ret : new List<ZoneLight>();
        }

        public bool HasZoneLights(int mapid)
        {
            return mZoneLights.ContainsKey(mapid);
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void FillLights()
        {
            var dataEntries = new Dictionary<uint, List<LightDataEntry>>();
            for(var i = 0; i < Storage.DbcStorage.LightData.NumRows; ++i)
            {
                var row = Storage.DbcStorage.LightData.GetRow(i);
                var entry = row.Get<LightDataEntry>(0);
                List<LightDataEntry> l;
                if (dataEntries.TryGetValue(entry.skyId, out l))
                    l.Add(entry);
                else
                    dataEntries.Add(entry.skyId, new List<LightDataEntry> {entry});
            }

            var lightMap = new Dictionary<int, MapLight>();
            for(var i = 0; i < Storage.DbcStorage.Light.NumRows; ++i)
            {
                var row = Storage.DbcStorage.Light.GetRow(i);
                var lightElem = row.Get<LightEntry>(0);
                var light = new LightEntryData(lightElem);

                light.Position = new SharpDX.Vector3(light.Position.X / 36.0f, light.Position.Y / 36.0f,
                    light.Position.Z / 36.0f);
                light.Position.Z = 64.0f * Metrics.TileSize - light.Position.Z;
                light.InnerRadius /= 36.0f;
                light.OuterRadius /= 36.0f;

                var paramsData = Storage.DbcStorage.LightParams.GetRowById(light.RefParams).Get<LightParamsEntry>(0);
                var l = new MapLight(light, ref paramsData);
                List<LightDataEntry> elems;
                if (dataEntries.TryGetValue((uint) light.RefParams, out elems))
                    l.AddAllData(elems);

                List<MapLight> lightList;
                if (mLights.TryGetValue(light.MapId, out lightList))
                    lightList.Add(l);
                else
                    mLights.Add(light.MapId, new List<MapLight> {l});

                lightMap[light.Id] = l;
            }

            var zoneLightMap = new Dictionary<int, ZoneLight>();
            for(var i = 0; i < Storage.DbcStorage.ZoneLight.NumRows; ++i)
            {
                var light = new ZoneLight();
                var zl = Storage.DbcStorage.ZoneLight.GetRow(i).Get<DbcZoneLight>(0);
                light.SetDbcZoneLight(ref zl);
                var le = Storage.DbcStorage.Light.GetRowById(zl.RefLight).Get<LightEntry>(0);

                var dq = mLights[le.MapId];
                dq.RemoveAll(m => m.LightId == le.Id);

                var lp = lightMap[le.Id];
                light.Light = lp;
                List<ZoneLight> zlList;
                if (mZoneLights.TryGetValue(zl.MapId, out zlList))
                    zlList.Add(light);
                else
                    mZoneLights.Add(zl.MapId, new List<ZoneLight> {light});

                zoneLightMap[zl.Id] = light;
            }

            for (var i = 0; i < Storage.DbcStorage.ZoneLightPoint.NumRows; ++i)
            {
                var zp = Storage.DbcStorage.ZoneLightPoint.GetRow(i).Get<ZoneLightPoint>(0);
                ZoneLight zl;
                if (zoneLightMap.TryGetValue(zp.RefZoneLight, out zl) == false)
                    continue;

                zl.AddPolygonPoint(ref zp);
            }

            foreach (var pair in zoneLightMap)
                pair.Value.CreatePolygon();
        }
    }
}

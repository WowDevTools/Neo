using System;
using System.Collections.Generic;
using System.Linq;
using WoWEditor6.IO;

namespace WoWEditor6.Storage
{
    static class MapFormatGuess
    {  
        public static int FieldMapName { get; private set; }
        public static int FieldMapTitle { get; private set; }
        public static int FieldLoadingScreenPath { get; private set; }
        public static int FieldLoadingScreenHasWidescreen { get; private set; }
        public static int FieldMapLoadingScreen { get; private set; }

        public static void Initialize()
        {
            var map = DbcStorage.Map;
            var mapNameFields = new List<int>();
            var mapTitleFields = new List<int>();

            for (var i = 0; i < map.NumFields; ++i)
            {
                mapNameFields.Add(i != 0 ? 0 : -1);
                mapTitleFields.Add(i != 0 ? 0 : -1);
            }

            for(var i = 0; i < Math.Min(50, map.NumRows); ++i)
            {
                var record = map.GetRow(i);
                // start at field 1, ID cant be any string
                for(var j = 1; j < map.NumFields; ++j)
                {
                    if (mapNameFields.Count(f => f >= 0) == 1)
                        break;

                    if (mapNameFields[j] < 0)
                        continue;

                    var name = record.GetString(j);
                    if (name == null)
                    {
                        mapNameFields[j] = -1;
                        mapTitleFields[j] = -1;
                        continue;
                    }

                    mapTitleFields[j] += 1;

                    if (FileManager.Instance.Provider.Exists(string.Format(@"World\Maps\{0}\{0}.wdt", name)))
                        mapNameFields[j] += 1;
                }
            }

            var idx = -1;
            var maxValue = mapNameFields.Max();
            for(var i = 0; i < mapNameFields.Count; ++i)
            {
                if (mapNameFields[i] != maxValue) continue;

                idx = i;
                break;
            }

            if (idx < 0)
                throw new InvalidOperationException("Unable to find the internal name field in Map.dbc");

            FieldMapName = idx;

            mapTitleFields[idx] = -1;

            idx = -1;
            maxValue = mapTitleFields.Max();
            for (var i = 0; i < mapTitleFields.Count; ++i)
            {
                if (mapTitleFields[i] != maxValue) continue;

                idx = i;
                break;
            }

            if (idx < 0)
                throw new InvalidOperationException("Unable to find the title field in Map.dbc");

            FieldMapTitle = idx;

            InitLoadingScreen();
            FindLoadingScreenLink();
        }

        private static void InitLoadingScreen()
        {
            var loadingScreenFields = new List<bool>();
            for (var i = 0; i < DbcStorage.LoadingScreen.NumFields; ++i)
                loadingScreenFields.Add(i != 0);

            for(var i = 0; i < DbcStorage.LoadingScreen.NumRows; ++i)
            {
                var row = DbcStorage.LoadingScreen.GetRow(i);
                for(var j = 0; j < loadingScreenFields.Count; ++j)
                {
                    if (loadingScreenFields[j] == false)
                        continue;

                    var str = row.GetString(j);
                    if(str == null)
                    {
                        loadingScreenFields[j] = false;
                        continue;
                    }

                    if (FileManager.Instance.Provider.Exists(str) == false)
                        loadingScreenFields[j] = false;
                }

                if (loadingScreenFields.Count(f => f) == 1)
                    break;
            }

            FieldLoadingScreenPath = -1;
            FieldLoadingScreenHasWidescreen = -1;
            for (var i = 0; i < loadingScreenFields.Count; ++i)
            {
                if (!loadingScreenFields[i]) continue;

                FieldLoadingScreenPath = i;
                FieldLoadingScreenHasWidescreen = i + 1;
                break;
            }

            if (FieldLoadingScreenHasWidescreen >= DbcStorage.LoadingScreen.NumFields)
                FieldLoadingScreenHasWidescreen = -1;

            if (FieldLoadingScreenPath < 0)
                throw new InvalidOperationException("Unable to find the loading screen asset path");
        }

        private static void FindLoadingScreenLink()
        {
            var mapLoadScreenFields = new List<bool>();
            var mapLoadNumZeros = new List<int>();
            for (var i = 0; i < DbcStorage.Map.NumFields; ++i)
            {
                mapLoadNumZeros.Add(0);
                mapLoadScreenFields.Add(i != 0);
            }

            for(var i = 0; i < Math.Min(100, DbcStorage.Map.NumRows); ++i)
            {
                var row = DbcStorage.Map.GetRow(i);

                for(var j = 0; j < mapLoadScreenFields.Count; ++j)
                {
                    if (mapLoadScreenFields[j] == false)
                        continue;

                    if(j == FieldMapName || j == FieldMapTitle)
                    {
                        mapLoadScreenFields[j] = false;
                        continue;
                    }

                    var id = row.GetInt32(j);
                    if (id == 0)
                        mapLoadNumZeros[j]++;

                    if (DbcStorage.LoadingScreen.GetRowById(id) != null || id == 0) continue;

                    mapLoadScreenFields[j] = false;
                }
            }

            FieldMapLoadingScreen = -1;
            var minZeros = int.MaxValue;

            for(var i = 0; i < mapLoadScreenFields.Count; ++i)
            {
                if (!mapLoadScreenFields[i]) continue;

                if (mapLoadNumZeros[i] >= minZeros)
                    continue;

                minZeros = mapLoadNumZeros[i];
                FieldMapLoadingScreen = i;
            }
        }
    }
}

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
        }
    }
}

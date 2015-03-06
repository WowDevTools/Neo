using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Terrain
{
    class MapAreaGroundEffects
    {
        private int mIndexX, mIndexY;
        private static readonly Dictionary<int, string[]> GroundEffectCache = new Dictionary<int, string[]>(); 

        public void ProcessChunk(MapChunk chunk)
        {
            var layerDoodads = new List<string[]>();
            var groundEffects = new List<DbcRecord>();
            for (var i = 0; i < 4; ++i)
            {
                if (i >= chunk.Layers.Length || chunk.Layers[i].EffectId < 0)
                {
                    layerDoodads.Add(new string[0]);
                    groundEffects.Add(null);
                    continue;
                }

                var row = Storage.DbcStorage.GroundEffectTexture.GetRowById(chunk.Layers[i].EffectId);
                if (row == null)
                {
                    layerDoodads.Add(new string[0]);
                    groundEffects.Add(null);
                    continue;
                }

                layerDoodads.Add(GetDoodads(chunk.Layers[i].EffectId, row));
                groundEffects.Add(row);
            }

            var step = Metrics.ChunkSize / 24.0f;
            var hasDoodadsList = layerDoodads.Select(s => s.Any(d => d != null)).ToList();
            for (var i = 0; i < 64; ++i)
            {
                var layer = chunk.GroundEffectLayer[i];
                if (layer < 0 || layer >= 4)
                    continue;

                if (hasDoodadsList[layer] == false)
                    continue;

                var texRow = groundEffects[layer];

                var ix = i / 64;
                var iy = i % 64;
            }
        }

        private string[] GetDoodads(int effect, DbcRecord textureRow)
        {
            if (GroundEffectCache.ContainsKey(effect))
                return GroundEffectCache[effect];


            var ret = new string[4];
            for (var i = 0; i < 4; ++i)
            {
                var doodadRef = textureRow.GetInt32(1 + i);
                if (doodadRef < 0)
                    continue;

                ret[i] = GetDoodadString(doodadRef);
            }

            GroundEffectCache.Add(effect, ret);
            return ret;
        }

        private string GetDoodadString(int id)
        {
            var row = Storage.DbcStorage.GroundEffectDoodad.GetRowById(id);
            if (row == null)
                return null;

            if (FileManager.Instance.Version > FileDataVersion.Warlords)
                return row.GetString(1);

            var fileRef = row.GetInt32(1);
            if (fileRef <= 0)
                return null;

            var fileRow = Storage.DbcStorage.FileData.GetRowById(fileRef);
            return Path.Combine(fileRow.GetString(2), fileRow.GetString(1));
        }
    }
}

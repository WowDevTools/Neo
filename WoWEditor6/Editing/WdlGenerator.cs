using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WoWEditor6.IO;

namespace WoWEditor6.Editing
{
    class WdlGenerator
    {
        class TerrainTileEntry
        {
            public byte[] MaocData;
            public byte[] MahoData;
        }

        public static async void Generate(string continent, Action<string, float> progressCallback, Action completeCallback)
        {
            await Task.Factory.StartNew(() =>
            {
                var wdlFile = string.Format(@"World\Maps\{0}\{0}.wdl", continent);

                if (!FileManager.Instance.Provider.Exists(wdlFile))
                    CreateNewWdlFile(continent, progressCallback);
                else
                    UpdateWdl(continent, progressCallback);

                completeCallback();
            });
        }

        private static void CreateNewWdlFile(string continent, Action<string, float> progressCallback)
        {
            var maofValues = new int[4096];
            var heightValues = new List<short[]>();
            var curOffset = 0;

            for (var i = 0; i < 64 * 64; ++i)
            {
                progressCallback(string.Format("Processing {0}_{1}_{2}.adt...", continent, i % 64, i / 64), i / 4096.0f);
                var ix = i % 16;
                var iy = i / 16;
                var heights = GetMareEntry(continent, ix, iy);
                if (heights == null)
                    maofValues[i] = 0;
                else
                {
                    maofValues[i] = curOffset;
                    // MARE + size + shorts + MVER chunk
                    curOffset += heights.Length * 2 + 8 + 12;
                    heightValues.Add(heights);
                }
            }

            var wdlPath = string.Format(@"World\Maps\{0}\{0}.wdl", continent);
            using (var strm = FileManager.Instance.GetOutputStream(wdlPath))
            {
                var writer = new BinaryWriter(strm);
                writer.Write(0x4D564552);
                writer.Write(4);
                writer.Write(18);

                writer.Write(0x4D414F46);
                writer.Write(maofValues.Length * 4);
                writer.WriteArray(maofValues);

                foreach (var heights in heightValues)
                {
                    writer.Write(0x4D415245);
                    writer.Write(heights.Length * 2);
                    writer.WriteArray(heights);
                }
            }
        }

        private static unsafe void UpdateWdl(string continent, Action<string, float> progressCallback)
        {
            var preChunks = new Dictionary<int, byte[]>();
            var wdlPath = string.Format(@"World\Maps\{0}\{0}.wdl", continent);
            List<TerrainTileEntry> existingTiles;
            var mareOffsets = new uint[64 * 64];
            using (var strm = FileManager.Instance.Provider.OpenFile(wdlPath))
            {
                var reader = new BinaryReader(strm);
                while ((strm.Length - strm.Position) > 8)
                {
                    var signature = reader.ReadInt32();
                    var size = reader.ReadInt32();
                    var data = reader.ReadBytes(size);
                    // skip the per ADT stuff like MARE, MAOC and MAHO
                    if (signature == 0x4D415245 || signature == 0x4D414F43
                        || signature == 0x4D41484F)
                        continue;

                    if (preChunks.ContainsKey(signature))
                        continue;

                    preChunks.Add(signature, data);
                }

                byte[] offsetBytes;
                preChunks.TryGetValue(0x4D414F46, out offsetBytes);
                if (offsetBytes != null)
                {
                    fixed (uint* ptr = mareOffsets)
                    {
                        fixed (byte* bptr = offsetBytes)
                        {
                            UnsafeNativeMethods.CopyMemory((byte*)ptr, bptr, 64 * 64 * 4);
                        }
                    }
                }

                existingTiles = ParseTiles(mareOffsets, reader);
            }

            using (var strm = FileManager.Instance.GetOutputStream(wdlPath))
            {
                var writer = new BinaryWriter(strm);
                foreach (var pair in preChunks.Where(pair => pair.Key != 0x4D414F46))
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value.Length);
                    writer.Write(pair.Value);
                }

                var marePos = writer.BaseStream.Position;
                writer.Write(0x4D414F46);
                writer.Write(mareOffsets.Length * 4);
                writer.WriteArray(mareOffsets);

                for (var i = 0; i < 64 * 64; ++i)
                {
                    progressCallback(string.Format("Processing {0}_{1}_{2}.adt...", continent, i % 64, i / 64), i / 4096.0f);
                    var existing = existingTiles[i];
                    var heights = GetMareEntry(continent, i % 64, i / 64);
                    if (heights == null)
                    {
                        mareOffsets[i] = 0;
                        continue;
                    }

                    if (existing == null)
                        existing = new TerrainTileEntry();

                    mareOffsets[i] = (uint) writer.BaseStream.Position;
                    writer.Write(0x4D415245);
                    writer.Write(heights.Length * 2);
                    writer.WriteArray(heights);

                    if (existing.MaocData != null)
                    {
                        writer.Write(0x4D414F43);
                        writer.Write(existing.MaocData.Length);
                        writer.Write(existing.MaocData);
                    }

                    if (existing.MahoData != null)
                    {
                        writer.Write(0x4D41484F);
                        writer.Write(existing.MahoData.Length);
                        writer.Write(existing.MahoData);
                    }
                }

                writer.BaseStream.Position = marePos + 8;
                writer.WriteArray(mareOffsets);
            }
        }

        private static List<TerrainTileEntry> ParseTiles(IEnumerable<uint> offsets, BinaryReader reader)
        {
            var ret = new List<TerrainTileEntry>();
            var strm = reader.BaseStream;
            foreach (var t in offsets)
            {
                strm.Position = t;
                if (t == 0)
                {
                    ret.Add(null);
                    continue;
                }

                var entry = new TerrainTileEntry();
                var signature = reader.ReadInt32();
                reader.ReadInt32();
                if (signature != 0x4D415245)
                {
                    ret.Add(null);
                    continue;
                }

                reader.ReadArray<short>(17 * 17 + 16 * 16);

                if (FileManager.Instance.Version <= FileDataVersion.Lichking)
                {
                    signature = reader.ReadInt32();
                    switch (signature)
                    {
                        case 0x4D414F43:
                            entry.MaocData = reader.ReadBytes(reader.ReadInt32());
                            break;
                        case 0x4D41484F:
                            entry.MahoData = reader.ReadBytes(reader.ReadInt32());
                            break;
                        default:
                            strm.Position -= 4;
                            break;
                    }
                }

                ret.Add(entry);
            }

            return ret;
        } 

        private static short[] GetMareEntry(string continent, int ax, int ay)
        {
            var retValue = new short[17 * 17 + 16 * 16];
            var filePath = string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", continent, ax, ay);
            if (FileManager.Instance.Provider.Exists(filePath) == false)
                return null;

            using (var strm = FileManager.Instance.Provider.OpenFile(filePath))
            {
                var reader = new BinaryReader(strm);
                var heights = new float[256][];

                for (var i = 0; i < 16; ++i)
                {
                    for (var j = 0; j < 16; ++j)
                    {
                        float baseHeight;
                        int chunkSize;
                        // if there ain't no MCNK entry for a chunk in an ADT somethings
                        // gone really wrong. Not having an MCVT in an MCNK is different,
                        // but when there isn't any MCNK skip the ADT.
                        if (!SeekNextMcnk(reader, out baseHeight, out chunkSize))
                            return null;

                        var curPos = reader.BaseStream.Position;

                        if (!MoveToMcvt(reader))
                            continue;

                        heights[i * 16 + j] = reader.ReadArray<float>(145);
                        for (var k = 0; k < heights[i * 16 + j].Length; ++k)
                            heights[i * 16 + j][k] += baseHeight;

                        reader.BaseStream.Position = curPos + chunkSize;
                    }
                }

                const float stepSize = Metrics.TileSize / 16.0f;
                for (var i = 0; i < 17; ++i)
                {
                    for (var j = 0; j < 17; ++j)
                    {
                        var posx = j * stepSize;
                        var posy = i * stepSize;
                        
                        retValue[i * 17 + j] = (short) 
                            Math.Min(
                                Math.Max(
                                    Math.Round(GetLandHeight(heights, posx, posy)), 
                                    short.MinValue), 
                                short.MaxValue);
                    }
                }

                for (var i = 0; i < 16; ++i)
                {
                    for (var j = 0; j < 16; ++j)
                    {
                        var posx = j * stepSize;
                        var posy = i * stepSize;
                        posx += stepSize / 2.0f;
                        posy += stepSize / 2.0f;

                        retValue[17 * 17 + i * 16 + j] = (short)
                            Math.Min(
                                Math.Max(
                                    Math.Round(GetLandHeight(heights, posx, posy)),
                                    short.MinValue),
                                short.MaxValue);
                    }
                }
            }

            return retValue;
        }

        private static float GetLandHeight(IReadOnlyList<float[]> heights, float x, float y)
        {
            var cx = (int)Math.Floor(x / Metrics.ChunkSize);
            var cy = (int)Math.Floor(y / Metrics.ChunkSize);
            cx = Math.Min(Math.Max(cx, 0), 15);
            cy = Math.Min(Math.Max(cy, 0), 15);

            if (heights[cy * 16 + cx] == null)
                return 0;

            x -= cx * Metrics.ChunkSize;
            y -= cy * Metrics.ChunkSize;

            var row = (int)(y / (Metrics.UnitSize * 0.5f) + 0.5f);
            var col = (int)((x - Metrics.UnitSize * 0.5f * (row % 2)) / Metrics.UnitSize + 0.5f);

            if (row < 0 || col < 0 || row > 16 || col > (((row % 2) != 0) ? 8 : 9))
                return 0;

            return heights[cy * 16 + cx][17 * (row / 2) + (((row % 2) != 0) ? 9 : 0) + col];
        }

        private static bool SeekNextMcnk(BinaryReader reader, out float baseHeight, out int chunkSize)
        {
            chunkSize = 0;
            baseHeight = 0;
            try
            {
                var signature = reader.ReadUInt32();
                var size = reader.ReadInt32();

                while (signature != 0x4D434E4B)
                {
                    reader.ReadBytes(size);
                    signature = reader.ReadUInt32();
                    size = reader.ReadInt32();
                }

                if (FileManager.Instance.Version < FileDataVersion.Lichking)
                {
                    var mcnk = reader.Read<IO.Files.Terrain.Wotlk.Mcnk>();
                    baseHeight = mcnk.Position.Z;
                    reader.BaseStream.Position -= SizeCache<IO.Files.Terrain.Wotlk.Mcnk>.Size;
                }
                else
                {
                    var mcnk = reader.Read<IO.Files.Terrain.WoD.Mcnk>();
                    baseHeight = mcnk.Position.Z;
                    reader.BaseStream.Position -= SizeCache<IO.Files.Terrain.WoD.Mcnk>.Size;
                }

                chunkSize = size;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool MoveToMcvt(BinaryReader reader)
        {
            reader.BaseStream.Position -= 4;
            var size = reader.ReadInt32();
            if (FileManager.Instance.Version >= FileDataVersion.Cataclysm)
            {
                var mcnk = reader.Read<IO.Files.Terrain.Wotlk.Mcnk>();
                if (mcnk.Mcvt < 8 + SizeCache<IO.Files.Terrain.Wotlk.Mcnk>.Size)
                    return false;

                var toRead = mcnk.Mcvt - SizeCache<IO.Files.Terrain.Wotlk.Mcnk>.Size;
                reader.ReadBytes(toRead);
                return true;
            }

            var basePos = reader.BaseStream.Position;
            reader.Read<IO.Files.Terrain.WoD.Mcnk>();
            while ((basePos - reader.BaseStream.Position + 8) < size)
            {
                var signature = reader.ReadUInt32();
                var csize = reader.ReadInt32();
                if (signature == 0x4D435654)
                    return true;

                reader.ReadBytes(csize);
            }

            return false;
        }
    }
}

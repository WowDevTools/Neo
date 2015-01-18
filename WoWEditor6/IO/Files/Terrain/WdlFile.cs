using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace WoWEditor6.IO.Files.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    struct MareEntry
    {
        public unsafe fixed short outer [17 * 17];
        public unsafe fixed short inner [16 * 16];
    }

    class WdlFile
    {
        private readonly Dictionary<int, MareEntry> mEntries = new Dictionary<int, MareEntry>();

        public void Load(string continent)
        {
            var wdlPath = string.Format(@"World\Maps\{0}\{0}.wdl", continent);
            using (var strm = FileManager.Instance.Provider.OpenFile(wdlPath))
            {
                if (strm == null)
                    throw new FileNotFoundException(continent);

                var reader = new BinaryReader(strm);
                var signature = 0u;
                while (signature != 0x4D414F46)
                {
                    signature = reader.ReadUInt32();
                    var size = reader.ReadInt32();
                    if (signature == 0x4D414F46)
                        break;

                    reader.ReadBytes(size);
                }

                var tileOffsets = reader.ReadArray<int>(4096);
                for(var i = 0; i < 4096; ++i)
                {
                    if (tileOffsets[i] <= 0)
                        continue;

                    strm.Position = tileOffsets[i] + 8;
                    mEntries.Add(i, reader.Read<MareEntry>());
                }
            }
        }

        public bool HasEntry(int x, int y)
        {
            if (x < 0 || y < 0 || x > 63 || y > 63)
                return false;

            var index = x + y * 64;
            return mEntries.ContainsKey(index);

        }

        public MareEntry GetEntry(int x, int y)
        {
            if (x < 0 || y < 0 || x > 63 || y > 63)
                throw new ArgumentException();

            var index = x + y * 64;
            MareEntry ret;
            mEntries.TryGetValue(index, out ret);
            return ret;
        }
    }
}

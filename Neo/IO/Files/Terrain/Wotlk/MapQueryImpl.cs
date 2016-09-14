using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Terrain.Wotlk
{
    class MapQueryImpl : IMapAreaQueryImplementation
    {
        public void Execute(MapAreaQuery query)
        {
            if (query.RequestedChunks.Count == 0)
                return;

            var fileName = string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", query.Continent, query.IndexX, query.IndexY);
            using (var strm = FileManager.Instance.Provider.OpenFile(fileName))
            {
                if (strm == null)
                    return;

                var reader = new BinaryReader(strm);
                while (strm.Position + 8 <= strm.Length)
                {
                    var signature = reader.ReadUInt32();
                    var size = reader.ReadInt32();
                    if (size + strm.Position > strm.Length)
                        break;

                    var data = reader.ReadBytes(size);

                    if (query.RequestedChunks.Contains(signature))
                        query.AddResult(signature, new DataChunk {Data = data, Signature = signature, Size = size});
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Neo.IO.Files.Terrain
{
	public class MapAreaQuery
    {
        private readonly Dictionary<uint, DataChunk> mResults = new Dictionary<uint, DataChunk>();

        public DataChunk this[uint signature]
        {
            get
            {
                return mResults[signature];
            }
        }

        public int IndexX { get; private set; }
        public int IndexY { get; private set; }
        public string Continent { get; private set; }

        public List<uint> RequestedChunks { get; private set; }

        public MapAreaQuery(string continent, int indexX, int indexY, params uint[] chunks)
        {
            IndexX = indexX;
            IndexY = indexY;
            Continent = continent;
            RequestedChunks = chunks.ToList();
        }

        public void AddResult(uint key, DataChunk chunk)
        {
            if (mResults.ContainsKey(key))
                return;

            mResults.Add(key, chunk);
        }
    }
}

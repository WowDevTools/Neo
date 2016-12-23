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
                return this.mResults[signature];
            }
        }

        public int IndexX { get; private set; }
        public int IndexY { get; private set; }
        public string Continent { get; private set; }

        public List<uint> RequestedChunks { get; private set; }

        public MapAreaQuery(string continent, int indexX, int indexY, params uint[] chunks)
        {
	        this.IndexX = indexX;
	        this.IndexY = indexY;
	        this.Continent = continent;
	        this.RequestedChunks = chunks.ToList();
        }

        public void AddResult(uint key, DataChunk chunk)
        {
            if (this.mResults.ContainsKey(key))
            {
	            return;
            }

	        this.mResults.Add(key, chunk);
        }
    }
}

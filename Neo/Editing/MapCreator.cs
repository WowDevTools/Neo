using Neo.Storage;

namespace Neo.Editing
{
	internal class MapCreator
    {
        private readonly string mInternalName;

        public MapCreator(string internalName)
        {
	        this.mInternalName = internalName;
        }

        public bool CreateNew(string mapName)
        {
            if (Exists())
            {
	            return false;
            }

	        return false;
        }

        private bool Exists()
        {
            for (var i = 0; i < DbcStorage.Map.NumRows; ++i)
            {
                var row = DbcStorage.Map.GetRow(i);
                var internalName = row.GetString(MapFormatGuess.FieldMapName);
                if (internalName.ToLowerInvariant().Equals(this.mInternalName.ToLowerInvariant()))
                {
	                return true;
                }
            }

            return false;
        }
    }
}

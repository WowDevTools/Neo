using WoWEditor6.Storage;

namespace WoWEditor6.Editing
{
    class MapCreator
    {
        private readonly string mInternalName;

        public MapCreator(string internalName)
        {
            mInternalName = internalName;
        }

        public bool CreateNew(string mapName)
        {
            if (Exists())
                return false;

            return false;
        }

        private bool Exists()
        {
            for (var i = 0; i < DbcStorage.Map.NumRows; ++i)
            {
                var row = DbcStorage.Map.GetRow(i);
                var internalName = row.GetString(MapFormatGuess.FieldMapName);
                if (internalName.ToLowerInvariant().Equals(mInternalName.ToLowerInvariant()))
                    return true;
            }

            return false;
        }
    }
}

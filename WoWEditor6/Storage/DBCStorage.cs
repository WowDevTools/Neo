using WoWEditor6.IO.Files;

namespace WoWEditor6.Storage
{
    static class DbcStorage
    {
        public static DbcFile Map { get; } = new DbcFile();

        public static void Initialize()
        {
            Map.Load(@"DBFilesClient\Map.dbc");

            MapFormatGuess.Initialize();
        }
    }
}

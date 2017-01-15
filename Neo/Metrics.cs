namespace Neo
{
	internal static class Metrics
    {
        public const float TileSize = 533.0f + 1.0f / 3.0f;
        public const float ChunkSize = TileSize / 16.0f;
        public const float UnitSize = ChunkSize / 8.0f;
        public const float MapMidPoint = 32.0f * TileSize;
        public const float ChunkRadius = 1.4142135f * ChunkSize;
    }
}

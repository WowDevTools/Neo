
namespace Neo.IO.Files
{
    public struct DataChunk
    {
        public uint Signature;
        public int Size;
        public byte[] Data;
    }
}

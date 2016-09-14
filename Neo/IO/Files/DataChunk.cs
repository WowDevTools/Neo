
namespace Neo.IO.Files
{
    struct DataChunk
    {
        public uint Signature;
        public int Size;
        public byte[] Data;
    }
}

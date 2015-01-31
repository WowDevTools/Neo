using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files
{
    struct DataChunk
    {
        public uint Signature;
        public int Size;
        public byte[] Data;
    }
}

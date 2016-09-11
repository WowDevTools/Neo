using System.IO;

namespace WDBXLib.FileTypes
{
    public class WDBC : DBHeader
    {
        public override void ReadHeader(ref BinaryReader dbReader, string signature)
        {
            base.ReadHeader(ref dbReader, signature);
        }
    }
}

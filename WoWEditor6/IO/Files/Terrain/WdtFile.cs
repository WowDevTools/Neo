using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Terrain
{
    class WdtFile
    {
        public uint Flags { get; private set; }

        public void Load(string continent)
        {
            var fileName = string.Format(@"World\Maps\{0}\{0}.wdt", continent);
            using (var strm = FileManager.Instance.Provider.OpenFile(fileName))
            {
                if (strm == null)
                {
                    Flags = 0;
                    return;
                }

                var reader = new BinaryReader(strm);
                while(true)
                {
                    try
                    {
                        var id = reader.ReadUInt32();
                        var size = reader.ReadInt32();
                        if(id == 0x4D504844)
                        {
                            Flags = reader.ReadUInt32();
                            return;
                        }

                        reader.ReadBytes(size);
                    }
                    catch(Exception)
                    {
                        Flags = 0;
                        return;
                    }
                }
            }
        }
    }
}

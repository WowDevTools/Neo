using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.IO.Files.Terrain
{
	public class WdtFile
    {
        public uint Flags { get; set; }

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

        public void Save(string continent)
        {
            var hasWdt = FileManager.Instance.Provider.Exists(string.Format(@"World\Maps\{0}\{0}.wdt", continent));
            var chunks = new Dictionary<uint, DataChunk>();
            if(hasWdt)
            {
                using(var inFile = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}.wdt", continent)))
                {
                    var reader = new BinaryReader(inFile);
                    while(reader.BaseStream.Position + 8 < reader.BaseStream.Length)
                    {
                        var signature = reader.ReadUInt32();
                        var size = reader.ReadInt32();
                        if (reader.BaseStream.Position + size > reader.BaseStream.Length)
                            break;

                        var data = reader.ReadBytes(size);
                        if(chunks.ContainsKey(signature))
                        {
                            Log.Warning("Duplicate chunk in WDT file, this is unexpected!");
                            continue;
                        }

                        chunks.Add(signature, new DataChunk
                        {
                            Data = data,
                            Signature = signature,
                            Size = size
                        });

                    }
                }
            }

            using(var outStream = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}.wdt", continent)))
            {
                var writer = new BinaryWriter(outStream);
                if (hasWdt == false)
                    CreateEmptyWdt(writer, continent);
                else
                {
                    if(chunks.ContainsKey(0x4D504844) == false)
                    {
                        var dch = new DataChunk
                        {
                            Signature = 0x4D504844,
                            Size = 32,
                            Data =
                                BitConverter.GetBytes(Flags)
                                    .Concat(new byte[]
                                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})
                                    .ToArray()
                        };

                        chunks.Add(dch.Signature, dch);
                    }
                    else
                    {
                        var cnk = chunks[0x4D504844];
                        Buffer.BlockCopy(BitConverter.GetBytes(Flags), 0, cnk.Data, 0, 4);
                    }

                    foreach(var pair in chunks)
                    {
                        writer.Write(pair.Value.Signature);
                        writer.Write(pair.Value.Size);
                        writer.Write(pair.Value.Data);
                    }
                }
            }
        }

        private void CreateEmptyWdt(BinaryWriter writer, string continent)
        {
            writer.Write(0x4D564552); // MVER
            writer.Write(4);
            writer.Write(18);
            writer.Write(0x4D484452);

            writer.Write(0x4D504844);
            writer.Write(32);
            writer.Write(Flags);
            writer.Write(0);
            writer.WriteArray(new[] {0, 0, 0, 0, 0, 0});

            writer.Write(0x4D41494E);
            writer.Write(4096 * 8);

            for (var i = 0; i < 64; ++i)
            {
                for(var j = 0; j < 64; ++j)
                {
                    if(FileManager.Instance.Provider.Exists(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", continent, j, i)))
                    {
                        writer.Write(1);
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(0);
                        writer.Write(0);
                    }
                }
            }
        }
    }
}

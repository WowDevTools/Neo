using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.CASC
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileKey
    {
        public byte v1, v2, v3, v4, v5, v6, v7, v8, v9;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileKeyMD5
    {
        public byte v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15, v16;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct IndexFileEntry
    {
        public FileKey key;
        public byte indexHigh;
        public uint indexLowBE;
        public uint size;
    }

    class IndexEntry
    {
        public uint index;
        public uint offset;
        public uint size;
    }

    class BLTEChunk
    {
        public long sizeCompressed;
        public long sizeUncompressed;
    }

    class EncodingEntry
    {
        public long size;
        public Binary[] keys;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RootEntryFile
    {
        public FileKeyMD5 md5;
        public ulong hash;
    }

    class RootEntry
    {
        public Binary MD5;
        public ulong hash;
    }

    class FileManager : IFileProvider
    {
        private string mDataDir;
        private readonly Dictionary<Binary, IndexEntry> mIndexData = new Dictionary<Binary, IndexEntry>();
        private readonly KeyValueConfig mBuildConfig = new KeyValueConfig();
        private readonly TokenConfig mBuildInfo = new TokenConfig();
        private readonly Dictionary<uint, DataStream> mDataStreams = new Dictionary<uint, DataStream>();
        private readonly Dictionary<Binary, EncodingEntry> mEncodingData = new Dictionary<Binary, EncodingEntry>();
        private readonly Dictionary<ulong, List<RootEntry>> mRootData = new Dictionary<ulong, List<RootEntry>>();

        public void Initialize(string dataDirectory)
        {
            mDataDir = dataDirectory;
            mDataDir = Path.Combine(mDataDir, "Data\\data");

            InitConfigs();

            FindIdexFiles().ForEach(LoadIndexFile);

            LoadEncodingFile();
            LoadRootFile();
        }

        public Stream OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        private void InitConfigs()
        {
            using (var buildStrm = new StreamReader(File.OpenRead(Path.Combine(mDataDir, "..\\..\\.build.info"))))
                mBuildInfo.Load(buildStrm);

            var buildKeys = mBuildInfo["Build Key"];
            var buildKey = buildKeys.FirstOrDefault();
            if (buildKey == default(string)) throw new InvalidOperationException(".build.info is missing a build key");

            var buildCfgPath = Path.Combine(mDataDir, "..\\..\\Data\\config", buildKey.Substring(0, 2), buildKey.Substring(2, 2), buildKey);
            using (var buildCfg = new StreamReader(File.OpenRead(buildCfgPath)))
                mBuildConfig.Load(buildCfg);
        }

        private void LoadIndexFile(string file)
        {
            using (var strm = File.OpenRead(file))
            {
                using (var reader = new BinaryReader(strm))
                {
                    var h2Len = reader.ReadInt32();
                    reader.ReadInt32();
                    var padded = (8 + h2Len + 0x0F) & 0xFFFFFFF0;
                    strm.Position = padded;
                    var dataLen = reader.ReadInt32();
                    reader.ReadInt32();
                    var numBlocks = dataLen / 18;
                    var blocks = reader.ReadArray<IndexFileEntry>(numBlocks);
                    foreach(var block in blocks)
                    {
                        var key = new Binary(new[]
                        {
                            block.key.v1, block.key.v2, block.key.v3, block.key.v4, block.key.v5,
                            block.key.v6, block.key.v7, block.key.v8, block.key.v9
                        });

                        if (mIndexData.ContainsKey(key))
                            continue;

                        var idxLowBe = block.indexLowBE;
                        var idxHigh = block.indexHigh;
                        var idxLow = idxLowBe >> 24;
                        idxLow |= ((idxLowBe >> 16) & 0xFF) << 8;
                        idxLow |= ((idxLowBe >> 8) & 0xFF) << 16;
                        idxLow |= ((idxLowBe >> 0) & 0xFF) << 24;

                        mIndexData.Add(key, new IndexEntry()
                        {
                            index = (((byte)(idxHigh << 2)) | ((idxLow & 0xC0000000) >> 30)),
                            offset = (idxLow & 0x3FFFFFFF),
                            size = block.size
                        });
                    }
                }
            }
        }

        private void LoadRootFile()
        {
            var encKeyStr = mBuildConfig["root"].FirstOrDefault();
            if (encKeyStr == null) throw new InvalidOperationException("Build config is missing root key");
            var encodingKey = encKeyStr.HexToBytes().ToArray();

            EncodingEntry encEntry;
            if (mEncodingData.TryGetValue(new Binary(encodingKey), out encEntry) == false || encEntry.keys.Length == 0)
                throw new InvalidOperationException("Unable to find encoding value for root file");

            IndexEntry entry;
            if (mIndexData.TryGetValue(new Binary(encEntry.keys[0].ToArray().Take(9).ToArray()), out entry) == false)
                throw new InvalidOperationException("Unable to locate root file in index table");

            var strm = GetDataStream(entry.index);
            using (var fileReader = new BinaryReader(strm.Stream, Encoding.UTF8, true))
            {
                fileReader.BaseStream.Position = entry.offset + 30;
                using (var reader = new BinaryReader(BlteGetData(fileReader, entry.size)))
                {
                    try
                    {
                        while(true)
                        {
                            var count = reader.ReadInt32();
                            reader.ReadBytes(8 + count * 4);
                            var entries = reader.ReadArray<RootEntryFile>(count);
                            foreach (var e in entries)
                            {
                                var b = e.md5;
                                var rootEntry = new RootEntry
                                {
                                    hash = e.hash,
                                    MD5 = new Binary(new[]
                                    {
                                        b.v1, b.v2, b.v3, b.v4, b.v5, b.v6, b.v7, b.v8, b.v9, b.v10, b.v11,
                                        b.v12, b.v13, b.v14, b.v15, b.v16
                                    })
                                };
                                if (mRootData.ContainsKey(e.hash))
                                    mRootData[e.hash].Add(rootEntry);
                                else
                                    mRootData.Add(e.hash, new List<RootEntry> {rootEntry});
                            }
                        }
                    }
                    catch(EndOfStreamException)
                    {

                    }
                }
            }
        }

        private void LoadEncodingFile()
        {
            var encodingKeyStr = mBuildConfig["encoding"].ElementAtOrDefault(1);
            if (encodingKeyStr == null) throw new InvalidOperationException("Build config is missing encoding key");

            var encodingKey = new Binary(encodingKeyStr.HexToBytes().Take(9).ToArray());
            if (mIndexData.ContainsKey(encodingKey) == false)
                throw new InvalidOperationException("Encoding file not found");

            var entry = mIndexData[encodingKey];
            var strm = GetDataStream(entry.index);
            using (var fileReader = new BinaryReader(strm.Stream, Encoding.UTF8, true))
            {
                fileReader.BaseStream.Position = entry.offset + 30;
                using (var reader = new BinaryReader(BlteGetData(fileReader, entry.size)))
                {
                    reader.BaseStream.Position = 9;
                    var numEntries = reader.ReadUInt32BE();
                    reader.BaseStream.Position += 5;
                    var ofsEntries = reader.ReadUInt32BE();
                    reader.BaseStream.Position += ofsEntries + 32 * numEntries;

                    for (var i = 0; i < numEntries; ++i)
                    {
                        var keys = reader.ReadUInt16();
                        while (keys != 0)
                        {
                            var size = reader.ReadUInt32BE();
                            var md5 = reader.ReadBytes(16);
                            var keyValues = reader.ReadArray<FileKeyMD5>(keys);
                            var e = new EncodingEntry()
                            {
                                size = size,
                                keys =
                                    keyValues.Select(
                                        b =>
                                            new Binary(new[]
                                            {
                                                b.v1, b.v2, b.v3, b.v4, b.v5, b.v6, b.v7, b.v8, b.v9, b.v10, b.v11,
                                                b.v12, b.v13, b.v14, b.v15, b.v16
                                            })).ToArray()
                            };
                            mEncodingData.Add(new Binary(md5), e);
                            keys = reader.ReadUInt16();
                        }

                        try
                        {
                            var curb = reader.BaseStream.ReadByte();
                            while (curb == 0) curb = reader.BaseStream.ReadByte();

                            reader.BaseStream.Position -= 1;
                        }
                        catch (EndOfStreamException) { }
                    }
                }
            }
        }

        private List<string> FindIdexFiles()
        {
            var ret = new List<string>();

            for(var i = 0; i < 0x10; ++i)
            {
                var files = Directory.GetFiles(mDataDir, i.ToString("X2") + "*.idx");
                ret.Add(files.Last());
            }

            return ret;
        }

        private DataStream GetDataStream(uint index)
        {
            lock(mDataStreams)
            {
                DataStream ret;
                if (mDataStreams.TryGetValue(index, out ret))
                    return ret;

                var path = Path.Combine(mDataDir, "data." + index.ToString("D3"));
                ret = new DataStream(path);
                mDataStreams.Add(index, ret);
                return ret;
            }
        }

        private MemoryStream BlteGetData(BinaryReader reader, long size)
        {
            if (reader.ReadUInt32() != 0x45544C42)
                throw new InvalidOperationException("Invalid file in archive. Invalid BLTE header");

            var sizeFrameHeader = reader.ReadUInt32BE();
            uint numChunks;
            var totalSize = 0L;
            if(sizeFrameHeader == 0)
            {
                numChunks = 1;
                totalSize = size - 8;
            }
            else
            {
                if (reader.ReadByte() != 0x0F)
                    throw new InvalidOperationException("Unknown error in BLTE: unk1 != 0x0F. This is not good im told.");

                var sizes = reader.ReadBytes(3);
                numChunks = (uint) ((sizes[0] << 16) | (sizes[1] >> 8) | sizes[2]);
            }

            if (numChunks == 0)
                return new MemoryStream();

            var chunks = new BLTEChunk[numChunks];
            for(var i = 0; i < numChunks; ++i)
            {
                var chunk = new BLTEChunk();
                chunks[i] = chunk;
                if(sizeFrameHeader != 0)
                {
                    chunk.sizeCompressed = reader.ReadUInt32BE();
                    chunk.sizeUncompressed = reader.ReadUInt32BE();
                    reader.BaseStream.Position += 16;
                }
                else
                {
                    chunk.sizeCompressed = totalSize;
                    chunk.sizeUncompressed = totalSize - 1;
                }
            }

            var data = new List<byte>();

            for(var i = 0; i < numChunks; ++i)
            {
                var code = reader.ReadByte();
                switch (code)
                {
                    case 0x4E:
                        data.AddRange(reader.ReadBytes((int)(chunks[i].sizeCompressed - 1)));
                        break;

                    case 0x5A:
                        {
                            if (sizeFrameHeader != 0)
                            {
                                var curPos = reader.BaseStream.Position;
                                reader.BaseStream.Position += 2;
                                using (var strm = new DeflateStream(reader.BaseStream, CompressionMode.Decompress, true))
                                {
                                    var content = new byte[(int)chunks[i].sizeUncompressed];
                                    var numRead = 0;
                                    while (numRead < content.Length)
                                    {
                                        numRead += strm.Read(content, numRead, content.Length - numRead);
                                    }

                                    data.AddRange(content);
                                }
                                reader.BaseStream.Position = curPos + (chunks[i].sizeCompressed - 1);
                            }
                            else
                            {
                                var content = reader.ReadBytes((int)(chunks[i].sizeCompressed - 1));
                                using (var inStrm = new MemoryStream(content))
                                {
                                    inStrm.Position = 2;
                                    using (var strm = new DeflateStream(inStrm, CompressionMode.Decompress))
                                    {
                                        using (var outStrm = new MemoryStream())
                                        {
                                            strm.CopyTo(outStrm);
                                            data.AddRange(outStrm.ToArray());
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            return new MemoryStream(data.ToArray());
        }
    }
}

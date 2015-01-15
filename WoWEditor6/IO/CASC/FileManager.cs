using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WoWEditor6.IO.CASC
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileKey
    {
        public readonly byte v1, v2, v3, v4, v5, v6, v7, v8, v9;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileKeyMd5
    {
        public readonly byte v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15, v16;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct IndexFileEntry
    {
        public FileKey key;
        public readonly byte indexHigh;
        public readonly uint indexLowBE;
        public readonly uint size;
    }

    class IndexEntry
    {
        public uint mIndex;
        public uint mOffset;
        public uint mSize;
    }

    class BlteChunk
    {
        public long mSizeCompressed;
        public long mSizeUncompressed;
    }

    class EncodingEntry
    {
#pragma warning disable 414
        public long mSize;
#pragma warning restore 414
        public Binary[] mKeys;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RootEntryFile
    {
        public FileKeyMd5 md5;
        public readonly ulong hash;
    }

    class RootEntry
    {
#pragma warning disable 414
        internal Binary mMd5;
        internal ulong mHash;
#pragma warning restore 414
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
            path = path.ToUpperInvariant();
            var hash = (new JenkinsHash()).Compute(path);
            List<RootEntry> roots;
            if (mRootData.TryGetValue(hash, out roots) == false)
                return null;

            foreach(var root in roots)
            {
                EncodingEntry enc;
                if (mEncodingData.TryGetValue(root.mMd5, out enc) == false)
                    continue;

                if (enc.mKeys.Length == 0)
                    continue;

                IndexEntry indexKey = null;
                var found = enc.mKeys.Any(key => mIndexData.TryGetValue(new Binary(key.ToArray().Take(9).ToArray()), out indexKey));

                if (found == false)
                    continue;

                var strm = GetDataStream(indexKey.mIndex);
                lock(strm)
                {
                    using (var reader = new BinaryReader(strm.Stream, Encoding.UTF8, true))
                    {
                        strm.Stream.Position = 30;
                        return BlteGetData(reader, indexKey.mSize);
                    }
                }
            }

            return null;
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
                            mIndex = (((byte)(idxHigh << 2)) | ((idxLow & 0xC0000000) >> 30)),
                            mOffset = (idxLow & 0x3FFFFFFF),
                            mSize = block.size
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
            if (mEncodingData.TryGetValue(new Binary(encodingKey), out encEntry) == false || encEntry.mKeys.Length == 0)
                throw new InvalidOperationException("Unable to find encoding value for root file");

            IndexEntry entry;
            if (mIndexData.TryGetValue(new Binary(encEntry.mKeys[0].ToArray().Take(9).ToArray()), out entry) == false)
                throw new InvalidOperationException("Unable to locate root file in index table");

            var strm = GetDataStream(entry.mIndex);
            using (var fileReader = new BinaryReader(strm.Stream, Encoding.UTF8, true))
            {
                fileReader.BaseStream.Position = entry.mOffset + 30;
                using (var reader = new BinaryReader(BlteGetData(fileReader, entry.mSize)))
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
                                    mHash = e.hash,
                                    mMd5 = new Binary(new[]
                                    {
                                        b.v1, b.v2, b.v3, b.v4, b.v5, b.v6, b.v7, b.v8, b.v9, b.v10, b.v11,
                                        b.v12, b.v13, b.v14, b.v15, b.v16
                                    })
                                };
                                if (mRootData.ContainsKey(e.hash))
                                    mRootData[e.hash].Add(rootEntry);
                                else
                                    mRootData.Add(e.hash, new List<RootEntry>(20) {rootEntry});
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
            var strm = GetDataStream(entry.mIndex);
            using (var fileReader = new BinaryReader(strm.Stream, Encoding.UTF8, true))
            {
                fileReader.BaseStream.Position = entry.mOffset + 30;
                using (var reader = new BinaryReader(BlteGetData(fileReader, entry.mSize)))
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
                            var keyValues = reader.ReadBytes(keys * 16);
                            var e = new EncodingEntry()
                            {
                                mSize = size,
                                mKeys = new Binary[keys]
                            };

                            for (var j = 0; j < keys; ++j)
                            {
                                var b = j * 16;
                                e.mKeys[j] = new Binary(new[]
                                {
                                    keyValues[b], keyValues[b + 1], keyValues[b + 2], keyValues[b + 3],keyValues[b + 4],keyValues[b + 5],keyValues[b + 6],keyValues[b + 7],
                                    keyValues[b + 8],keyValues[b + 9],keyValues[b + 10],keyValues[b + 11],keyValues[b + 12],keyValues[b + 13],keyValues[b + 14],keyValues[b + 15]
                                });
                            }
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

        private static MemoryStream BlteGetData(BinaryReader reader, long size)
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

            var chunks = new BlteChunk[numChunks];
            for(var i = 0; i < numChunks; ++i)
            {
                var chunk = new BlteChunk();
                chunks[i] = chunk;
                if(sizeFrameHeader != 0)
                {
                    chunk.mSizeCompressed = reader.ReadUInt32BE();
                    chunk.mSizeUncompressed = reader.ReadUInt32BE();
                    reader.BaseStream.Position += 16;
                }
                else
                {
                    chunk.mSizeCompressed = totalSize;
                    chunk.mSizeUncompressed = totalSize - 1;
                }
            }

            var data = new List<byte>();

            for(var i = 0; i < numChunks; ++i)
            {
                var code = reader.ReadByte();
                switch (code)
                {
                    case 0x4E:
                        data.AddRange(reader.ReadBytes((int)(chunks[i].mSizeCompressed - 1)));
                        break;

                    case 0x5A:
                        {
                            if (sizeFrameHeader != 0)
                            {
                                var curPos = reader.BaseStream.Position;
                                reader.BaseStream.Position += 2;
                                using (var strm = new DeflateStream(reader.BaseStream, CompressionMode.Decompress, true))
                                {
                                    var content = new byte[(int)chunks[i].mSizeUncompressed];
                                    var numRead = 0;
                                    while (numRead < content.Length)
                                    {
                                        numRead += strm.Read(content, numRead, content.Length - numRead);
                                    }

                                    data.AddRange(content);
                                }
                                reader.BaseStream.Position = curPos + (chunks[i].mSizeCompressed - 1);
                            }
                            else
                            {
                                var content = reader.ReadBytes((int)(chunks[i].mSizeCompressed - 1));
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

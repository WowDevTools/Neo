using System.IO;
using System.IO.Compression;

namespace Neo.IO.CASC
{
    class FileListing : IFileListing
    {
        public DirectoryEntry RootEntry { get; private set; }

        public FileListing()
        {
            RootEntry = new DirectoryEntry {Name = "Files"};

            if (File.Exists("listfile.bin") == false)
            {
                Log.Warning("Missing Listfile for Warlords of Draenor files. Asset browser wont work.");
                return;
            }

            Init();
        }

        private void Init()
        {
            using (var strm = File.OpenRead("listfile.bin"))
            {
                using (var memStrm = new MemoryStream())
                {
                    using (var zstrm = new DeflateStream(strm, CompressionMode.Decompress))
                    {
                        zstrm.CopyTo(memStrm);
                        memStrm.Position = 0;

                        var reader = new BinaryReader(memStrm);
                        reader.ReadString(); // empty string of the root entry
                        var numDirs = reader.ReadInt32();
                        var numFiles = reader.ReadInt32();

                        for (var i = 0; i < numDirs; ++i)
                        {
                            var dir = ReadDirectory(reader);
                            RootEntry.Children.Add(dir.Name.ToLowerInvariant(), dir);
                        }

                        for (var i = 0; i < numFiles; ++i)
                        {
                            var name = reader.ReadString();
                            RootEntry.Children.Add(name.ToLowerInvariant(), new FileEntry {Name = name});
                        }
                    }
                }
            }
        }

        private static DirectoryEntry ReadDirectory(BinaryReader reader)
        {
            var name = reader.ReadString();
            var ret = new DirectoryEntry {Name = name};

            var numDirs = reader.ReadInt32();
            var numFiles = reader.ReadInt32();

            for (var i = 0; i < numDirs; ++i)
            {
                var dir = ReadDirectory(reader);
                ret.Children.Add(dir.Name.ToLowerInvariant(), dir);
            }

            for (var i = 0; i < numFiles; ++i)
            {
                var fname = reader.ReadString();
                ret.Children.Add(fname.ToLowerInvariant(), new FileEntry { Name = fname });
            }

            return ret;
        }
    }
}

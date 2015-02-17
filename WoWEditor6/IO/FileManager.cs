using System;
using System.Diagnostics;
using System.IO;

namespace WoWEditor6.IO
{
    class FileManager
    {
        public static FileManager Instance { get; private set; }

        public IFileProvider Provider { get; private set; }
        public IFileListing FileListing { get; set; }
        public string DataPath { get; set; }
        public bool Initialized { get; private set; }

        public event Action LoadComplete;
        public FileDataVersion Version { get; private set; }

        static FileManager()
        {
            Instance = new FileManager();
        }

        public Stream GetOutputStream(string path)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Output", path);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? ".");
                return File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public Stream GetExistingFile(string path)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Output", path);
                using (var strm = File.OpenRead(fullPath))
                {
                    var retStream = new MemoryStream();
                    strm.CopyTo(retStream);
                    retStream.Position = 0;
                    return retStream;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void InitFromPath()
        {
            if(string.IsNullOrEmpty(DataPath))
                throw new InvalidOperationException("Cannot initialize file system without a path");

            if (File.Exists(Path.Combine(DataPath, ".build.info")))
            {
                Files.Terrain.AdtFactory.Instance.Version = FileDataVersion.Warlords;
                Files.Models.ModelFactory.Instance.Version = FileDataVersion.Warlords;
                Files.Sky.SkyManager.InitVersion(FileDataVersion.Warlords);
                Version = FileDataVersion.Warlords;

                var mgr = new CASC.FileManager();
                mgr.LoadComplete += () =>
                {
                    Initialized = true;
                    Storage.DbcStorage.Initialize();
                    if (LoadComplete != null)
                        LoadComplete();
                };

                Provider = mgr;
                mgr.Initialize(DataPath);
            }
            else
                InitMpq();
        }

        private void InitMpq()
        {
            var version = FileVersionInfo.GetVersionInfo(Path.Combine(DataPath, "Wow.exe"));
            if (version.FilePrivatePart > 13000 || version.FilePrivatePart < 9000)
                throw new NotImplementedException("MPQ is only implemented for WOTLK (builds 9000 - 13000)");

            Files.Terrain.AdtFactory.Instance.Version = FileDataVersion.Lichking;
            Files.Models.ModelFactory.Instance.Version = FileDataVersion.Lichking;
            Files.Sky.SkyManager.InitVersion(FileDataVersion.Lichking);
            Version = FileDataVersion.Lichking;

            var mgr = new MPQ.FileManager();
            mgr.LoadComplete += () =>
            {
                Initialized = true;
                Storage.DbcStorage.Initialize();
                if (LoadComplete != null)
                    LoadComplete();
            };

            Provider = mgr;
            mgr.InitFromPath(DataPath);
        }
    }
}

using System;
using System.IO;

namespace WoWEditor6.IO
{
    class FileManager
    {
        public static FileManager Instance { get; } = new FileManager();

        public IFileProvider Provider { get; private set; }
        public string DataPath { get; set; }
        public bool Initialized { get; private set; }

        public event Action LoadComplete;

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

        public void InitFromPath()
        {
            if(string.IsNullOrEmpty(DataPath))
                throw new InvalidOperationException("Cannot initialize file system without a path");

            if (File.Exists(Path.Combine(DataPath, ".build.info")))
            {
                Files.Terrain.AdtFactory.Instance.Version = FileDataVersion.Warlords;
                Files.Models.ModelFactory.Instance.Version = FileDataVersion.Warlords;
                Files.Sky.SkyManager.InitVersion(FileDataVersion.Warlords);

                var mgr = new CASC.FileManager();
                mgr.LoadComplete += () =>
                {
                    Initialized = true;
                    LoadComplete?.Invoke();
                };

                mgr.Initialize(DataPath);
                Provider = mgr;
            }
            else
                throw new NotSupportedException("Only CASC based installations are currently supported.");
        }
    }
}

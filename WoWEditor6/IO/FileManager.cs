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

        public void InitFromPath()
        {
            if(string.IsNullOrEmpty(DataPath))
                throw new InvalidOperationException("Cannot initialize file system without a path");

            if (File.Exists(Path.Combine(DataPath, ".build.info")))
            {
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

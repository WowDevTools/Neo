using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO
{
    class FileManager
    {
        public static FileManager Instance { get; } = new FileManager();

        public IFileProvider Provider { get; private set; }

        public void InitFromPath(string wowDir)
        {
            if (File.Exists(Path.Combine(wowDir, ".build.info")))
            {
                var mgr = new CASC.FileManager();
                mgr.Initialize(wowDir);
                Provider = mgr;
            }
            else
                throw new NotSupportedException("Only CASC based installations are currently supported.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.CASC
{
    class DataStream : IDisposable
    {
        public FileStream Stream { get; }

        public DataStream(string file)
        {
            Stream = File.OpenRead(file);
        }

        public void Dispose()
        {
            Stream?.Close();
        }
    }
}

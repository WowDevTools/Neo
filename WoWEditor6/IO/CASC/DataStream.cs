using System;
using System.IO;

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

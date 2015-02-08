using System;
using System.IO;

namespace WoWEditor6.IO.CASC
{
    class DataStream : IDisposable
    {
        public FileStream Stream { get; private set; }

        public DataStream(string file)
        {
            Stream = File.OpenRead(file);
        }

        public void Dispose()
        {
            if (Stream != null)
                Stream.Close();
        }
    }
}

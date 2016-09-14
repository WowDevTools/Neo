using System;
using System.IO;

namespace Neo.IO.CASC
{
    class DataStream : IDisposable
    {
        public FileStream Stream { get; private set; }

        public DataStream(string file)
        {
            Stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        ~DataStream()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

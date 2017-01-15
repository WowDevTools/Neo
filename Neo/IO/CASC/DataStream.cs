using System;
using System.IO;

namespace Neo.IO.CASC
{
	internal class DataStream : IDisposable
    {
        public FileStream Stream { get; private set; }

        public DataStream(string file)
        {
	        this.Stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        ~DataStream()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this.Stream != null)
            {
	            this.Stream.Close();
	            this.Stream = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

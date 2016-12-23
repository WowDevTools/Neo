using System;
using System.IO;

namespace Neo.IO.MPQ
{
    public class Archive
    {
        private readonly IntPtr mHandle;

        public string Name { get; private set; }

        public Archive(IntPtr handle, string name)
        {
            mHandle = handle;
            Name = name;
        }

        public bool Contains(string file)
        {
            return Imports.SFileHasFile(mHandle, file);
        }

        public Stream Open(string file)
        {
            IntPtr handle;
            if (Imports.SFileOpenFileEx(mHandle, file, 0, out handle) == false)
            {
	            return null;
            }

	        uint sizeHigh;
            var sizeLow = Imports.SFileGetFileSize(handle, out sizeHigh);
            var totalSize = ((ulong)sizeHigh << 32) | sizeLow;
            if (totalSize > int.MaxValue)
            {
	            return null;
            }

	        var buffer = new byte[(int) totalSize];
            int numRead;
            if(Imports.SFileReadFile(handle, buffer, (int)totalSize, out numRead, IntPtr.Zero) == false || numRead != (int)totalSize)
            {
                Imports.SFileCloseFile(handle);
                return null;
            }

            Imports.SFileCloseFile(handle);
            return new MemoryStream(buffer);
        }
    }
}

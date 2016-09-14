using System;
using System.Runtime.InteropServices;

namespace Neo.IO.MPQ
{
    static class Imports
    {
        [DllImport("Stormlib.dll", CharSet = CharSet.Ansi)]
        public static extern bool SFileOpenArchive(string mpqName, uint priority, uint flags, out IntPtr handle);

        [DllImport("Stormlib.dll", CharSet = CharSet.Ansi)]
        public static extern bool SFileHasFile(IntPtr mpq, string fileName);

        [DllImport("Stormlib.dll", CharSet = CharSet.Ansi)]
        public static extern bool SFileOpenFileEx(IntPtr mpq, string fileName, uint scope, out IntPtr handle);

        [DllImport("Stormlib.dll", CharSet = CharSet.Ansi)]
        public static extern bool SFileReadFile(IntPtr file, byte[] buffer, int toRead, out int numRead, IntPtr lpOverlapped);

        [DllImport("Stormlib.dll", CharSet = CharSet.Ansi)]
        public static extern uint SFileGetFileSize(IntPtr file, out uint fileSizeHigh);

        [DllImport("Stormlib.dll", CharSet = CharSet.Ansi)]
        public static extern void SFileCloseFile(IntPtr file);
    }
}

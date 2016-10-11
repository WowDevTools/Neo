using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Neo
{
    internal static unsafe class UnsafeNativeMethods
    {
	    [Obsolete("Windows-only P/Invoke call", true)]
        [DllImport("Kernel32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr CopyMemory(byte* dest, byte* src, int count);
    }
}

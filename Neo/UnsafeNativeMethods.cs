using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Neo
{
    internal static unsafe class UnsafeNativeMethods
    {
        [DllImport("Kernel32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr CopyMemory(byte* dest, byte* src, int count);

        [DllImport("User32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void GetKeyboardState(byte[] keyboard);
    }
}

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WoWEditor6
{
    internal static unsafe class UnsafeNativeMethods
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr MoveMemory(byte* dest, byte* src, int count);

        [DllImport("Kernel32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr CopyMemory(byte* dest, byte* src, int count);
    }
}

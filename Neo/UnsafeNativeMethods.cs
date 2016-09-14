using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Neo
{
    internal static unsafe class UnsafeNativeMethods
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr MoveMemory(byte* dest, byte* src, int count);

        [DllImport("Kernel32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr CopyMemory(byte* dest, byte* src, int count);

        [DllImport("User32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern int ToUnicode(int wVirtKey, int scanCode, byte[] keyState, StringBuilder outBuffer,
            int numBUffer, int flags);

        [DllImport("User32.dll", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void GetKeyboardState(byte[] keyboard);
    }
}

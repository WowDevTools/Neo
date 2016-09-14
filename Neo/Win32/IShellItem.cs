using System;
using System.Runtime.InteropServices;

namespace Neo.Win32
{
    public enum Sigdn : uint
    {
        Normaldisplay = 0,
        Parentrelativeparsing = 0x80018001,
        Parentrelativeforaddressbar = 0x8001c001,
        Desktopabsoluteparsing = 0x80028000,
        Parentrelativeediting = 0x80031001,
        Desktopabsoluteediting = 0x8004c000,
        Filesyspath = 0x80058000,
        Url = 0x80068000
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    public interface IShellItem
    {
        void BindToHandler(IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)]Guid bhid,
            [MarshalAs(UnmanagedType.LPStruct)]Guid riid,
            out IntPtr ppv);

        void GetParent(out IShellItem ppsi);

        void GetDisplayName(Sigdn sigdnName, out IntPtr ppszName);

        void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

        void Compare(IShellItem psi, uint hint, out int piOrder);
    };
}

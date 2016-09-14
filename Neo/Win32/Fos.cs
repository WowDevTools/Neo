using System;

namespace Neo.Win32
{
    [Flags]
    enum Fos : uint
    {
        FosOverwriteprompt = 0x00000002,
        FosStrictfiletypes = 0x00000004,
        FosNochangedir = 0x00000008,
        FosPickfolders = 0x00000020,
        FosForcefilesystem = 0x00000040, // Ensure that items returned are filesystem items.
        FosAllnonstorageitems = 0x00000080, // Allow choosing items that have no storage.
        FosNovalidate = 0x00000100,
        FosAllowmultiselect = 0x00000200,
        FosPathmustexist = 0x00000800,
        FosFilemustexist = 0x00001000,
        FosCreateprompt = 0x00002000,
        FosShareaware = 0x00004000,
        FosNoreadonlyreturn = 0x00008000,
        FosNotestfilecreate = 0x00010000,
        FosHidemruplaces = 0x00020000,
        FosHidepinnedplaces = 0x00040000,
        FosNodereferencelinks = 0x00100000,
        FosDontaddtorecent = 0x02000000,
        FosForceshowhidden = 0x10000000,
        FosDefaultnominimode = 0x20000000
    }
}

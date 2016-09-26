using System;

namespace WoWEditor6.Editing
{
    [Flags]
    enum EditMode
    {
        Sculpting = 1,
        Texturing = 2,
        Chunk = 4,
    }
}

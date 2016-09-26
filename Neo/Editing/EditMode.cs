using System;

namespace Neo.Editing
{
    [Flags]
    enum EditMode
    {
        Sculpting = 1,
        Texturing = 2,
        Chunk = 3,
    }
}

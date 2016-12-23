using System;

namespace Neo.Editing
{
    [Flags]
    internal enum EditMode
    {
        Sculpting = 1,
        Texturing = 2,
        Chunk = 4,
    }
}

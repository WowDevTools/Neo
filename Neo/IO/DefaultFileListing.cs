using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.IO
{
    class DefaultFileListing : IFileListing
    {
        public DirectoryEntry RootEntry { get; private set; }

        public DefaultFileListing() { RootEntry = new DirectoryEntry(); }
    }
}

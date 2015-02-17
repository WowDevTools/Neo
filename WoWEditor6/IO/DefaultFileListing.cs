using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO
{
    class DefaultFileListing : IFileListing
    {
        public DirectoryEntry RootEntry { get; private set; }

        public DefaultFileListing() { RootEntry = new DirectoryEntry(); }
    }
}

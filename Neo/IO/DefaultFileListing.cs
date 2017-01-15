using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.IO
{
	internal class DefaultFileListing : IFileListing
    {
        public DirectoryEntry RootEntry { get; private set; }

        public DefaultFileListing() {
	        this.RootEntry = new DirectoryEntry(); }
    }
}

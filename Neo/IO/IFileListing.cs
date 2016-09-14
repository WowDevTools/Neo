using System.Collections.Generic;
using System.Linq;

namespace Neo.IO
{
    public class FileSystemEntry
    {
        public Dictionary<string, FileSystemEntry> Children { get; private set; }

        public string Name { get; set; }

        protected FileSystemEntry()
        {
            Children = new Dictionary<string, FileSystemEntry>();
        }
    }

    public class DirectoryEntry : FileSystemEntry
    {

    }

    public class FileEntry : FileSystemEntry
    {
        
    }

    interface IFileListing
    {
        DirectoryEntry RootEntry { get; }
    }
}

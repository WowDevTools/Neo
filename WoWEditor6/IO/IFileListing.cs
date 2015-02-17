using System.Collections.Generic;

namespace WoWEditor6.IO
{
    class FileSystemEntry
    {
        public string Name { get; set; }
    }

    class DirectoryEntry : FileSystemEntry
    {
        public readonly Dictionary<string, FileSystemEntry> Children = new Dictionary<string, FileSystemEntry>(); 
    }

    class FileEntry : FileSystemEntry
    {
        
    }

    interface IFileListing
    {
        DirectoryEntry RootEntry { get; }
    }
}

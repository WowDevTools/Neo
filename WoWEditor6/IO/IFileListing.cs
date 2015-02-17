using System.Collections.Generic;
using System.Linq;

namespace WoWEditor6.IO
{
    public class FileSystemEntry
    {
        public Dictionary<string, FileSystemEntry> Children { get; private set; }

        public IEnumerable<FileSystemEntry> ChildElements
        {
            get
            {
                return
                    Children.Values.OfType<DirectoryEntry>()
                        .OrderBy(d => d.Name)
                        .Concat(Children.Values.OfType<FileEntry>().OrderBy(f => f.Name).Cast<FileSystemEntry>());
            }
        }

        public IEnumerable<DirectoryEntry> Directories
        {
            get { return Children.Values.OfType<DirectoryEntry>().OrderBy(d => d.Name); }
        }

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

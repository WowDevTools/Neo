using System;
using System.IO;

namespace WoWEditor6.IO.MPQ
{
    class FileListing : IFileListing
    {
        public DirectoryEntry RootEntry { get; private set; }

        public FileListing(FileManager fileMgr)
        {
            RootEntry = new DirectoryEntry {Name = "Files"};
            Init(fileMgr);
        }

        private void Init(FileManager mgr)
        {
            foreach (var archive in mgr.Archives)
            {
                using (var list = archive.Open("(listfile)"))
                {
                    if (list == null)
                        continue;

                    var reader = new StreamReader(list);
                    var fullText = reader.ReadToEnd();
                    var lines = fullText.Split('\r');
                    Array.ForEach(lines, AddFile);
                }
            }
        }

        private void AddFile(string file)
        {
            file = file.Trim();
            if (string.IsNullOrEmpty(file))
                return;

            if (file.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return;

            var dir = Path.GetDirectoryName(file) ?? "";
            var paths = dir.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
            var curDir = RootEntry;
            foreach (var path in paths)
            {
                if (curDir.Children.ContainsKey(path.ToLowerInvariant()))
                    curDir = curDir.Children[path.ToLowerInvariant()] as DirectoryEntry;
                else
                {
                    var dirEnt = new DirectoryEntry {Name = path};
                    curDir.Children.Add(path.ToLowerInvariant(), dirEnt);
                    curDir = dirEnt;
                }

                if (curDir == null)
                    return;
            }

            if (curDir == null)
                return;

            var fileName = Path.GetFileName(file);
            if (curDir.Children.ContainsKey(fileName.ToLowerInvariant()))
                return;

            curDir.Children.Add(fileName.ToLowerInvariant(), new FileEntry { Name = fileName });
        }
    }
}

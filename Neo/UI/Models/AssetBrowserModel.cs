using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IO;

namespace Neo.UI.Models
{
    public class AssetBrowserFile
    {
        public string Name { get { return mEntry.Name; } }

        public string Extension { get { return (Path.GetExtension(Name) ?? "").ToLowerInvariant(); } }
        public string FullPath { get { return mFullPath; } }

        private readonly AssetBrowserViewModel mModel;
        private readonly FileEntry mEntry;
        private readonly string mFullPath;

        public AssetBrowserFile(AssetBrowserViewModel viewModel, FileEntry entry, AssetBrowserDirectory parent)
        {
            mEntry = entry;
            mModel = viewModel;
            mFullPath = Name;
            var cur = parent;
            while (cur != null && cur.Parent != null)
            {
                mFullPath = cur.Name + "\\" + mFullPath;
                cur = cur.Parent;
            }
        }
    }

    public class AssetBrowserDirectory
    {
        public string Name { get { return mEntry.Name; } }
        public string FullPath { get { return mFullPath; } }
        public IEnumerable<AssetBrowserDirectory> Directories { get { return GetDirectories(); } }
        public IEnumerable<AssetBrowserFile> Files { get { return GetFiles(); } }

        public AssetBrowserDirectory Parent { get { return mParent; } }

        private readonly AssetBrowserViewModel mModel;
        private readonly FileSystemEntry mEntry;
        private IEnumerable<AssetBrowserDirectory> mDirectories;
        private IEnumerable<AssetBrowserFile> mFiles;
        private readonly AssetBrowserDirectory mParent;
        private readonly string mFullPath;

        public AssetBrowserDirectory(AssetBrowserViewModel viewModel, FileSystemEntry entry, AssetBrowserDirectory parent)
        {
            mParent = parent;
            mModel = viewModel;
            mEntry = entry;
            mFullPath = Name;
            var cur = parent;
            while (cur != null && cur.Parent != null)
            {
                mFullPath = cur.Name + "\\" + mFullPath;
                cur = cur.Parent;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private IEnumerable<AssetBrowserFile> GetFiles()
        {
            if (mFiles != null)
                return mFiles;

            if (mEntry is FileEntry)
                mFiles = new AssetBrowserFile[0];
            else
            {
                mFiles = mEntry.Children.Values.OfType<FileEntry>()
                    .OrderBy(f => f.Name).Select(f => new AssetBrowserFile(mModel, f, this));
            }

            return mFiles;
        }

        private IEnumerable<AssetBrowserDirectory> GetDirectories()
        {
            if(mDirectories != null)
                return mDirectories;
            
            // TODO: AssetBrowserViewModel should offer filters
            if (mEntry is FileEntry)
                mDirectories = new AssetBrowserDirectory[0];
            else
                mDirectories = mEntry.Children.Values.OfType<DirectoryEntry>()
                    .Where(d => d.Children.Count > 0)
                    .OrderBy(d => d.Name)
                    .Select(d => new AssetBrowserDirectory(mModel, d, this));

            return mDirectories;
        }
    }
}

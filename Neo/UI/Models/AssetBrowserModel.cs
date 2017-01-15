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
        public string Name { get { return this.mEntry.Name; } }

        public string Extension { get { return (Path.GetExtension(this.Name) ?? "").ToLowerInvariant(); } }
        public string FullPath { get { return this.mFullPath; } }

        private readonly AssetBrowserViewModel mModel;
        private readonly FileEntry mEntry;
        private readonly string mFullPath;

        public AssetBrowserFile(AssetBrowserViewModel viewModel, FileEntry entry, AssetBrowserDirectory parent)
        {
	        this.mEntry = entry;
	        this.mModel = viewModel;
	        this.mFullPath = this.Name;
            var cur = parent;
            while (cur != null && cur.Parent != null)
            {
	            this.mFullPath = cur.Name + "\\" + this.mFullPath;
                cur = cur.Parent;
            }
        }
    }

    public class AssetBrowserDirectory
    {
        public string Name { get { return this.mEntry.Name; } }
        public string FullPath { get { return this.mFullPath; } }
        public IEnumerable<AssetBrowserDirectory> Directories { get { return GetDirectories(); } }
        public IEnumerable<AssetBrowserFile> Files { get { return GetFiles(); } }

        public AssetBrowserDirectory Parent { get { return this.mParent; } }

        private readonly AssetBrowserViewModel mModel;
        private readonly FileSystemEntry mEntry;
        private IEnumerable<AssetBrowserDirectory> mDirectories;
        private IEnumerable<AssetBrowserFile> mFiles;
        private readonly AssetBrowserDirectory mParent;
        private readonly string mFullPath;

        public AssetBrowserDirectory(AssetBrowserViewModel viewModel, FileSystemEntry entry, AssetBrowserDirectory parent)
        {
	        this.mParent = parent;
	        this.mModel = viewModel;
	        this.mEntry = entry;
	        this.mFullPath = this.Name;
            var cur = parent;
            while (cur != null && cur.Parent != null)
            {
	            this.mFullPath = cur.Name + "\\" + this.mFullPath;
                cur = cur.Parent;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        private IEnumerable<AssetBrowserFile> GetFiles()
        {
            if (!(this.mEntry is DirectoryEntry) && this.mFiles != null)
            {
	            return this.mFiles;
            }

	        if (this.mEntry is FileEntry)
	        {
		        this.mFiles = new AssetBrowserFile[0];
	        }
	        else
            {
	            this.mFiles = this.mEntry.Children.Values.OfType<FileEntry>()
                    .OrderBy(f => f.Name).Select(f => new AssetBrowserFile(this.mModel, f, this));
            }

            return this.mFiles;
        }

        private IEnumerable<AssetBrowserDirectory> GetDirectories()
        {
            if(this.mDirectories != null)
            {
	            return this.mDirectories;
            }

	        // TODO: AssetBrowserViewModel should offer filters
            if (this.mEntry is FileEntry)
            {
	            this.mDirectories = new AssetBrowserDirectory[0];
            }
            else
            {
	            this.mDirectories = this.mEntry.Children.Values.OfType<DirectoryEntry>()
		            .Where(d => d.Children.Count > 0)
		            .OrderBy(d => d.Name)
		            .Select(d => new AssetBrowserDirectory(this.mModel, d, this));
            }

	        return this.mDirectories;
        }
    }
}

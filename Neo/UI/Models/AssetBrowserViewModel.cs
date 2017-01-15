using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Neo.Annotations;
using Neo.IO;
using Neo.UI.Components;

namespace Neo.UI.Models
{
    public class AssetBrowserFilePreviewElement
    {
        public AssetBrowserFilePreview View { get; set; }
    }

    public class AssetBrowserViewModel : INotifyPropertyChanged
    {
        private readonly Dialogs.AssetBrowser mBrowser;
        private AssetBrowserDirectory mRootDiretory;
        private bool mShowTextures = true;
        private bool mShowModels = true;
        private bool mHideUnknown = true;
        private bool mShowSpecularTextures;
        private AssetBrowserDirectory mCurrentDirectory;
        private readonly ObservableCollection<AssetBrowserFilePreviewElement> mCurFiles = new ObservableCollection<AssetBrowserFilePreviewElement>();
        private IEnumerable<AssetBrowserFilePreviewElement> mFullElements = new List<AssetBrowserFilePreviewElement>();
        private ThumbnailCapture mThumbCapture;

        public bool ShowTextures { get { return this.mShowTextures; } set {
	        this.mShowTextures = value; UpdateShowTextures(value); } }

        public bool ShowModels { get { return this.mShowModels; } set {
	        this.mShowModels = value; UpdateShowModels(value); } }
        public bool HideUnknownFiles { get { return this.mHideUnknown; } set {
	        this.mHideUnknown = value; UpdateHideUnknownFiles(value); } }
        public bool ShowSpecularTextures { get { return this.mShowSpecularTextures; } set { UpdateShowSpecularTextures(value); } }

        public IEnumerable<AssetBrowserDirectory> AssetBrowserRoot { get { return new[] {this.mRootDiretory }; } }


        public AssetBrowserViewModel(Dialogs.AssetBrowser browser)
        {
	        this.mBrowser = browser;
            browser.Loaded += OnInitialized;
            FileManager.Instance.LoadComplete += OnFilesLoaded;
	        this.mRootDiretory = new AssetBrowserDirectory(this, new DirectoryEntry {Name = ""}, null);
            EditorWindowController.Instance.AssetBrowserModel = this;

            if (ThumbnailCache.ThumnailAdded == null)
            {
	            ThumbnailCache.ThumnailAdded += UpdateThumbnail;
            }
        }

        public void HandleImportFile()
        {
            var model = new ImportFileViewModel();
            model.ShowModal();
        }

        public async void HandleExportSelectedFile()
        {
            var selected = this.mBrowser.SelectedFilesListView.SelectedItem as AssetBrowserFilePreviewElement;
            if (selected == null)
            {
	            return;
            }

	        this.mBrowser.ExportFolderLink.IsEnabled = false;
	        this.mBrowser.ExportOneFileLink.IsEnabled = false;
	        this.mBrowser.BusyOverlayGrid.Visibility = Visibility.Visible;
	        this.mBrowser.SelectedFilesListView.Visibility = Visibility.Hidden;

            await Task.Factory.StartNew(() => FileManager.Instance.ExportFile(selected.View.FileEntry.FullPath));

	        this.mBrowser.ExportFolderLink.IsEnabled = true;
	        this.mBrowser.ExportOneFileLink.IsEnabled = true;
	        this.mBrowser.BusyOverlayGrid.Visibility = Visibility.Hidden;
	        this.mBrowser.SelectedFilesListView.Visibility = Visibility.Visible;

            var exportPath = Path.Combine(Path.GetFullPath(Properties.Settings.Default.ExportPath ?? ".\\Export"), selected.View.FileEntry.FullPath);
            MessageBox.Show(string.Format("The selected file has been successfully exported to:\n{0}", exportPath), "Neo - Export File");
        }

        public async void HandleExportSelectedFolder()
        {
            var selected = this.mBrowser.AssetTreeView.SelectedItem as AssetBrowserDirectory;
            if (selected == null)
            {
	            return;
            }

	        this.mBrowser.ExportFolderLink.IsEnabled = false;
	        this.mBrowser.ExportOneFileLink.IsEnabled = false;
	        this.mBrowser.BusyOverlayGrid.Visibility = Visibility.Visible;
	        this.mBrowser.SelectedFilesListView.Visibility = Visibility.Hidden;

            await Task.Factory.StartNew(() =>
            {
                foreach (var child in selected.Files)
                {
	                FileManager.Instance.ExportFile(child.FullPath);
                }
            });

	        this.mBrowser.ExportFolderLink.IsEnabled = true;
	        this.mBrowser.ExportOneFileLink.IsEnabled = true;
	        this.mBrowser.BusyOverlayGrid.Visibility = Visibility.Hidden;
	        this.mBrowser.SelectedFilesListView.Visibility = Visibility.Visible;

            var exportPath = Path.Combine(Path.GetFullPath(Properties.Settings.Default.ExportPath ?? ".\\Export"), selected.FullPath);
            MessageBox.Show(string.Format("The selected folder has been successfully exported to:\n{0}", exportPath), "Neo - Export Folder");
        }

        public void Handle_FileClicked(AssetBrowserFilePreviewElement element)
        {
            if (element.View.FileEntry.Extension.Contains("blp"))
            {
	            this.mBrowser.TexturePreviewImage.Source = element.View.PreviewImage.Source;
	            this.mBrowser.ModelPreviewControl.Visibility = Visibility.Hidden;
	            this.mBrowser.WmoPreviewControl.Visibility = Visibility.Hidden;
	            this.mBrowser.TexturePreviewImage.Visibility = Visibility.Visible;
            }
            else if (element.View.FileEntry.Extension.Contains("m2"))
            {
	            this.mBrowser.TexturePreviewImage.Visibility = Visibility.Hidden;
	            this.mBrowser.WmoPreviewControl.Visibility = Visibility.Hidden;
	            this.mBrowser.ModelPreviewRender.SetModel(element.View.FileEntry.FullPath);
	            this.mBrowser.ModelPreviewControl.Visibility = Visibility.Visible;
            }
            else if (element.View.FileEntry.Extension.Contains("wmo"))
            {
	            this.mBrowser.ModelPreviewControl.Visibility = Visibility.Hidden;
	            this.mBrowser.TexturePreviewImage.Visibility = Visibility.Hidden;
	            this.mBrowser.WmoPreviewControl.Visibility = Visibility.Visible;
	            this.mBrowser.WmoPreviewRender.SetModel(element.View.FileEntry.FullPath);
            }
            else
            {
	            this.mBrowser.ModelPreviewControl.Visibility = Visibility.Hidden;
	            this.mBrowser.TexturePreviewImage.Visibility = Visibility.Hidden;
	            this.mBrowser.WmoPreviewControl.Visibility = Visibility.Hidden;
	            this.mBrowser.TexturePreviewImage.Source = null;
            }
        }

        public void Handle_BrowserSelectionChanged(AssetBrowserDirectory newItem)
        {
	        this.mCurrentDirectory = newItem;

            if (newItem == null)
            {
	            return;
            }

	        this.mCurrentDirectory = newItem;
            UpdateItems(true);
        }

        private void UpdateShowSpecularTextures(bool value)
        {
	        this.mShowSpecularTextures = value;

            var textures = this.mFullElements.Where(f => f.View.FileEntry.Name.ToLowerInvariant().Contains("_s.blp") || f.View.FileEntry.Name.ToLowerInvariant().Contains("_h.blp"));
            if (value)
            {
                foreach (var elem in textures.Where(elem => this.mCurFiles.Contains(elem) == false))
                {
	                this.mCurFiles.Add(elem);
                }
            }
            else
            {
                foreach (var elem in textures.Where(elem => this.mCurFiles.Contains(elem)))
                {
	                this.mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateShowModels(bool value)
        {
	        this.mShowModels = value;

            var models = this.mFullElements.Where(f => f.View.FileEntry.Extension.Contains("m2"));
            if (value)
            {
                foreach (var elem in models.Where(elem => this.mCurFiles.Contains(elem) == false))
                {
	                this.mCurFiles.Add(elem);
                }
            }
            else
            {
                foreach (var elem in models.Where(elem => this.mCurFiles.Contains(elem)))
                {
	                this.mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateShowTextures(bool value)
        {
	        this.mShowTextures = value;

            var textures = this.mFullElements.Where(f => f.View.FileEntry.Extension.Contains("blp"));
            if (value)
            {
                foreach (var elem in textures.Where(elem => this.mCurFiles.Contains(elem) == false))
                {
	                this.mCurFiles.Add(elem);
                }
            }
            else
            {
                foreach (var elem in textures.Where(elem => this.mCurFiles.Contains(elem)))
                {
	                this.mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateHideUnknownFiles(bool value)
        {
	        this.mHideUnknown = value;
            if (value == false)
            {
                foreach (var elem in this.mFullElements.Where(elem => this.mCurFiles.Contains(elem) == false))
                {
                    if ((elem.View.FileEntry.Name.ToLowerInvariant().Contains("_h.blp") ||
                         elem.View.FileEntry.Name.ToLowerInvariant().Contains("_s.blp")) && this.mShowSpecularTextures == false)
                    {
                        continue;
                    }

	                this.mCurFiles.Add(elem);
                }
            }
            else
            {
                var toRemove = this.mFullElements.Where(
                        f =>
                            f.View.FileEntry.Extension.Contains("m2") == false &&
                            f.View.FileEntry.Extension.Contains("blp") == false
                );

                foreach (var elem in toRemove)
                {
	                this.mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateItems(bool directoryChanged = false)
        {
            if (this.mCurrentDirectory == null)
            {
	            this.mBrowser.SelectedFilesListView.ItemsSource = new object[0];
                return;
            }

            var files = this.mCurrentDirectory.Files.Select(
                    f => new AssetBrowserFilePreviewElement {View = new AssetBrowserFilePreview(f)}).ToList();
            var elements = files.Where(f =>
            {
                string[] known = new string[] { "m2", "blp", "wmo" };

                var ext = f.View.FileEntry.Extension.ToLowerInvariant();
                if (ext.Contains("blp") && this.mShowTextures == false)
                {
	                return false;
                }

	            if (ext.Contains("m2") && this.mShowModels == false)
	            {
		            return false;
	            }

	            if (f.View.FileEntry.Name.ToLowerInvariant().Contains("_s.blp") && this.mShowSpecularTextures == false)
	            {
		            return false;
	            }

	            if (f.View.FileEntry.Name.ToLowerInvariant().Contains("_h.blp") && this.mShowSpecularTextures == false)
	            {
		            return false;
	            }

	            if (!known.Contains(ext.TrimStart('.')) && this.mHideUnknown)
	            {
		            return false;
	            }

	            return true;
            }).ToList();

            if (directoryChanged == false && this.mCurFiles.Count() == elements.Count())
            {
	            return;
            }

	        this.mFullElements = files;

	        this.mCurFiles.Clear();
            foreach(var elem in elements)
            {
	            this.mCurFiles.Add(elem);

                if (elem.View.FileEntry.Extension.Contains("m2"))
                {
	                if (!ThumbnailCache.IsCached(elem.View.FileEntry.FullPath))
	                {
		                this.mThumbCapture.AddModel(elem.View.FileEntry.FullPath);
	                }
                }
            }
        }

        private void UpdateThumbnail(string filename)
        {
	        this.mBrowser.Dispatcher.Invoke(() => this.mCurFiles.FirstOrDefault(x => x.View.FileEntry.FullPath == filename)?.View.ReloadImage());
        }

        private void OnInitialized(object sender, EventArgs args)
        {
	        this.mBrowser.SelectedFilesListView.ItemsSource = this.mCurFiles;
	        this.mThumbCapture = new ThumbnailCapture(114, 114);
        }

        private void OnFilesLoaded()
        {
	        this.mRootDiretory = new AssetBrowserDirectory(this, FileManager.Instance.FileListing.RootEntry, null);
	        this.mBrowser.Dispatcher.Invoke(() =>
                OnPropertyChanged("AssetBrowserRoot"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
	            handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

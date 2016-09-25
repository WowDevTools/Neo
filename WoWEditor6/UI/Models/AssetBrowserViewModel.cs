using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using WoWEditor6.Annotations;
using WoWEditor6.IO;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Models
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

        public bool ShowTextures { get { return mShowTextures; } set { mShowTextures = value; UpdateShowTextures(value); } }

        public bool ShowModels { get { return mShowModels; } set { mShowModels = value; UpdateShowModels(value); } }
        public bool HideUnknownFiles { get { return mHideUnknown; } set { mHideUnknown = value; UpdateHideUnknownFiles(value); } }
        public bool ShowSpecularTextures { get { return mShowSpecularTextures; } set { UpdateShowSpecularTextures(value); } }

        public IEnumerable<AssetBrowserDirectory> AssetBrowserRoot { get { return new[] { mRootDiretory }; } }


        public AssetBrowserViewModel(Dialogs.AssetBrowser browser)
        {
            mBrowser = browser;
            browser.Loaded += OnInitialized;
            FileManager.Instance.LoadComplete += OnFilesLoaded;
            mRootDiretory = new AssetBrowserDirectory(this, new DirectoryEntry {Name = ""}, null);
            EditorWindowController.Instance.AssetBrowserModel = this;

            if (ThumbnailCache.ThumnailAdded == null)
                ThumbnailCache.ThumnailAdded += UpdateThumbnail;
        }

        public void HandleImportFile()
        {
            var model = new ImportFileViewModel();
            model.ShowModal();
        }

        public async void HandleExportSelectedFile()
        {
            var selected = mBrowser.SelectedFilesListView.SelectedItem as AssetBrowserFilePreviewElement;
            if (selected == null)
                return;

            mBrowser.ExportFolderLink.IsEnabled = false;
            mBrowser.ExportOneFileLink.IsEnabled = false;
            mBrowser.BusyOverlayGrid.Visibility = Visibility.Visible;
            mBrowser.SelectedFilesListView.Visibility = Visibility.Hidden;

            await Task.Factory.StartNew(() => FileManager.Instance.ExportFile(selected.View.FileEntry.FullPath));

            mBrowser.ExportFolderLink.IsEnabled = true;
            mBrowser.ExportOneFileLink.IsEnabled = true;
            mBrowser.BusyOverlayGrid.Visibility = Visibility.Hidden;
            mBrowser.SelectedFilesListView.Visibility = Visibility.Visible;

            var exportPath = Path.Combine(Path.GetFullPath(Properties.Settings.Default.ExportPath ?? ".\\Export"), selected.View.FileEntry.FullPath);
            MessageBox.Show(String.Format("The selected file has been successfully exported to:\n{0}", exportPath), "Neo - Export File");
        }

        public async void HandleExportSelectedFolder()
        {
            var selected = mBrowser.AssetTreeView.SelectedItem as AssetBrowserDirectory;
            if (selected == null)
                return;

            mBrowser.ExportFolderLink.IsEnabled = false;
            mBrowser.ExportOneFileLink.IsEnabled = false;
            mBrowser.BusyOverlayGrid.Visibility = Visibility.Visible;
            mBrowser.SelectedFilesListView.Visibility = Visibility.Hidden;

            await Task.Factory.StartNew(() =>
            {
                foreach (var child in selected.Files)
                    FileManager.Instance.ExportFile(child.FullPath);
            });

            mBrowser.ExportFolderLink.IsEnabled = true;
            mBrowser.ExportOneFileLink.IsEnabled = true;
            mBrowser.BusyOverlayGrid.Visibility = Visibility.Hidden;
            mBrowser.SelectedFilesListView.Visibility = Visibility.Visible;

            var exportPath = Path.Combine(Path.GetFullPath(Properties.Settings.Default.ExportPath ?? ".\\Export"), selected.FullPath);
            MessageBox.Show(String.Format("The selected folder has been successfully exported to:\n{0}", exportPath), "Neo - Export Folder");
        }

        public void Handle_FileClicked(AssetBrowserFilePreviewElement element)
        {
            if (element.View.FileEntry.Extension.Contains("blp"))
            {
                mBrowser.TexturePreviewImage.Source = element.View.PreviewImage.Source;
                mBrowser.ModelPreviewControl.Visibility = Visibility.Hidden;
                mBrowser.WmoPreviewControl.Visibility = Visibility.Hidden;
                mBrowser.TexturePreviewImage.Visibility = Visibility.Visible;
            }
            else if (element.View.FileEntry.Extension.Contains("m2"))
            {
                mBrowser.TexturePreviewImage.Visibility = Visibility.Hidden;
                mBrowser.WmoPreviewControl.Visibility = Visibility.Hidden;
                mBrowser.ModelPreviewRender.SetModel(element.View.FileEntry.FullPath);
                mBrowser.ModelPreviewControl.Visibility = Visibility.Visible;
            }
            else if (element.View.FileEntry.Extension.Contains("wmo"))
            {
                mBrowser.ModelPreviewControl.Visibility = Visibility.Hidden;
                mBrowser.TexturePreviewImage.Visibility = Visibility.Hidden;
                mBrowser.WmoPreviewControl.Visibility = Visibility.Visible;
                mBrowser.WmoPreviewRender.SetModel(element.View.FileEntry.FullPath);
            }
            else
            {
                mBrowser.ModelPreviewControl.Visibility = Visibility.Hidden;
                mBrowser.TexturePreviewImage.Visibility = Visibility.Hidden;
                mBrowser.WmoPreviewControl.Visibility = Visibility.Hidden;
                mBrowser.TexturePreviewImage.Source = null;
            }
        }

        public void Handle_BrowserSelectionChanged(AssetBrowserDirectory newItem)
        {
            mCurrentDirectory = newItem;

            if (newItem == null)
                return;

            mCurrentDirectory = newItem;
            UpdateItems(true);
        }

        private void UpdateShowSpecularTextures(bool value)
        {
            mShowSpecularTextures = value;

            var textures = mFullElements.Where(f => f.View.FileEntry.Name.ToLowerInvariant().Contains("_s.blp") || f.View.FileEntry.Name.ToLowerInvariant().Contains("_h.blp"));
            if (value)
            {
                foreach (var elem in textures.Where(elem => mCurFiles.Contains(elem) == false))
                {
                    mCurFiles.Add(elem);
                }
            }
            else
            {
                foreach (var elem in textures.Where(elem => mCurFiles.Contains(elem)))
                {
                    mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateShowModels(bool value)
        {
            mShowModels = value;

            var models = mFullElements.Where(f => f.View.FileEntry.Extension.Contains("m2"));
            if (value)
            {
                foreach (var elem in models.Where(elem => mCurFiles.Contains(elem) == false))
                {
                    mCurFiles.Add(elem);
                }
            }
            else
            {
                foreach (var elem in models.Where(elem => mCurFiles.Contains(elem)))
                {
                    mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateShowTextures(bool value)
        {
            mShowTextures = value;

            var textures = mFullElements.Where(f => f.View.FileEntry.Extension.Contains("blp"));
            if (value)
            {
                foreach (var elem in textures.Where(elem => mCurFiles.Contains(elem) == false))
                {
                    mCurFiles.Add(elem);
                }
            }
            else
            {
                foreach (var elem in textures.Where(elem => mCurFiles.Contains(elem)))
                {
                    mCurFiles.Remove(elem);
                }
            }
        }

        private void UpdateHideUnknownFiles(bool value)
        {
            mHideUnknown = value;
            if (value == false)
            {
                foreach (var elem in mFullElements.Where(elem => mCurFiles.Contains(elem) == false))
                {
                    if ((elem.View.FileEntry.Name.ToLowerInvariant().Contains("_h.blp") ||
                         elem.View.FileEntry.Name.ToLowerInvariant().Contains("_s.blp")) &&
                        mShowSpecularTextures == false)
                    {
                        continue;
                    }

                    mCurFiles.Add(elem);
                }
            }
            else
            {
                var toRemove =
                    mFullElements.Where(
                        f =>
                            f.View.FileEntry.Extension.Contains("m2") == false &&
                            f.View.FileEntry.Extension.Contains("blp") == false
                );

                foreach (var elem in toRemove)
                    mCurFiles.Remove(elem);
            }
        }

        private void UpdateItems(bool directoryChanged = false)
        {
            if (mCurrentDirectory == null)
            {
                mBrowser.SelectedFilesListView.ItemsSource = new object[0];
                return;
            }

            var files =
                mCurrentDirectory.Files.Select(
                    f => new AssetBrowserFilePreviewElement {View = new AssetBrowserFilePreview(f)}).ToList();
            var elements = files.Where(f =>
            {
                string[] known = new string[] { "m2", "blp", "wmo" };

                var ext = f.View.FileEntry.Extension.ToLowerInvariant();
                if (ext.Contains("blp") && mShowTextures == false)
                    return false;

                if (ext.Contains("m2") && mShowModels == false)
                    return false;

                if (f.View.FileEntry.Name.ToLowerInvariant().Contains("_s.blp") && mShowSpecularTextures == false)
                    return false;

                if (f.View.FileEntry.Name.ToLowerInvariant().Contains("_h.blp") && mShowSpecularTextures == false)
                    return false;

                if (!known.Contains(ext.TrimStart('.')) && mHideUnknown)
                    return false;

                return true;
            }).ToList();

            if (directoryChanged == false && mCurFiles.Count() == elements.Count())
                return;

            mFullElements = files;

            mCurFiles.Clear();
            foreach(var elem in elements)
            {
                mCurFiles.Add(elem);

                if (elem.View.FileEntry.Extension.Contains("m2"))
                    if (!ThumbnailCache.IsCached(elem.View.FileEntry.FullPath))
                        mThumbCapture.AddModel(elem.View.FileEntry.FullPath);
            }
        }

        private void UpdateThumbnail(string filename)
        {
            mBrowser.Dispatcher.Invoke(() => mCurFiles.FirstOrDefault(x => x.View.FileEntry.FullPath == filename)?.View.ReloadImage());
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            mBrowser.SelectedFilesListView.ItemsSource = mCurFiles;
            mThumbCapture = new ThumbnailCapture(114, 114);
        }

        private void OnFilesLoaded()
        {
            mRootDiretory = new AssetBrowserDirectory(this, FileManager.Instance.FileListing.RootEntry, null);
            mBrowser.Dispatcher.Invoke(() =>
                OnPropertyChanged("AssetBrowserRoot"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

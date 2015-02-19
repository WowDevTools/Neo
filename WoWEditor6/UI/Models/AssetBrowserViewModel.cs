using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WoWEditor6.Annotations;
using WoWEditor6.IO;

namespace WoWEditor6.UI.Models
{
    public class AssetBrowserViewModel : INotifyPropertyChanged
    {
        private readonly Dialogs.AssetBrowser mBrowser;
        private AssetBrowserDirectory mRootDiretory;
        private bool mShowTextures = true;
        private bool mShowModels = true;
        private bool mHideUnknown = true;
        private bool mHideKnownFileNames = true;
        private AssetBrowserDirectory mCurrentDirectory;

        public bool ShowTextures { get { return mShowTextures; } set { mShowTextures = value; UpdateItems(); } }

        public bool ShowModels { get { return mShowModels; } set { mShowModels = value; UpdateItems(); } }

        public IEnumerable<AssetBrowserDirectory> AssetBrowserRoot { get { return new []{mRootDiretory}; } }

        public AssetBrowserViewModel(Dialogs.AssetBrowser browser)
        {
            mBrowser = browser;
            browser.Initialized += OnInitialized;
            FileManager.Instance.LoadComplete += OnFilesLoaded;
            mRootDiretory = new AssetBrowserDirectory(this, new DirectoryEntry {Name = ""}, null);
        }

        public void Handle_BrowserSelectionChanged(AssetBrowserDirectory newItem)
        {
            mCurrentDirectory = newItem;

            if (newItem == null)
                return;

            mCurrentDirectory = newItem;
            UpdateItems();
        }

        private void UpdateItems()
        {
            if (mCurrentDirectory == null)
            {
                mBrowser.SelectedFilesListView.ItemsSource = new object[0];
                return;
            }


            var files = mCurrentDirectory.Files;
            mBrowser.SelectedFilesListView.ItemsSource = files.Where(f =>
            {
                var ext = f.Extension.ToLowerInvariant();
                if (ext.Contains("blp") && mShowTextures == false)
                    return false;

                if (ext.Contains("m2") && mShowModels == false)
                    return false;

                if (ext.Contains("m2") == false && ext.Contains("blp") == false && mHideUnknown)
                    return false;

                return true;
            }).Select(f => new { View = new Components.AssetBrowserFilePreview(f) });
        }

        private void OnInitialized(object sender, EventArgs args)
        {
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

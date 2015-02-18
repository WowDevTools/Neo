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
            if (newItem == null)
                return;

            var files = newItem.Files;
            mBrowser.SelectedFilesListView.ItemsSource = files.Select(f => new { View = new Components.AssetBrowserFilePreview(f) });
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

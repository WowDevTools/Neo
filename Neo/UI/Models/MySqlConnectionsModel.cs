using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Neo.UI.Models
{
    public class MySqlConnectionsModel : BindableBase
    {
        public ObservableCollection<string> Connections { get; set; }    

        public bool Default { get; set; }

        private string mVisibility;

        public string Visibility
        {
            get { return this.mVisibility; }
            set { SetProperty(ref this.mVisibility, value); }
        }

        private string mSaveAs;

        public string SaveAs
        {
            get { return this.mSaveAs; }
            set { SetProperty(ref this.mSaveAs, value); }
        }

        private bool mSaveIsEnabled;

        public bool SaveIsEnabled
        {
            get { return this.mSaveIsEnabled; }
            set { SetProperty(ref this.mSaveIsEnabled, value); }
        }

        private bool mNewIsEnabled;

        public bool NewIsEnabled
        {
            get { return this.mNewIsEnabled; }
            set { SetProperty(ref this.mNewIsEnabled, value); }
        }

        private bool mDeleteIsEnabled;

        public bool DeleteIsEnabled
        {
            get { return this.mDeleteIsEnabled; }
            set { SetProperty(ref this.mDeleteIsEnabled, value); }
        }

        private string mAddress;

        public string Address
        {
            get { return this.mAddress; }
            set { SetProperty(ref this.mAddress, value); }
        }
        private string mUsername;

        public string Username
        {
            get { return this.mUsername; }
            set { SetProperty(ref this.mUsername, value); }
        }

        private string mPassword;

        public string Password
        {
            get { return this.mPassword; }
            set { SetProperty(ref this.mPassword, value); }
        }

        private string mDatabase;

        public string Database
        {
            get { return this.mDatabase; }
            set { SetProperty(ref this.mDatabase, value); }
        }

        private bool mLoginIsEnabled;

        public bool LoginIsEnabled
        {
            get { return this.mLoginIsEnabled; }
            set { SetProperty(ref this.mLoginIsEnabled, value); }
        }

        private string mLoginContent;

        public string LoginContent
        {
            get { return this.mLoginContent; }
            set { SetProperty(ref this.mLoginContent, value); }
        }

        private string mSelectedItem;

        public string SelectedItem
        {
            get { return this.mSelectedItem; }
            set { SetProperty(ref this.mSelectedItem, value); }
        }
    }
}

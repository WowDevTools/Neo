using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace WoWEditor6.UI.Models
{
    public class MySqlConnectionsModel : BindableBase
    {
        public ObservableCollection<string> Connections { get; set; }    

        public bool Default { get; set; }

        private string mVisibility;

        public string Visibility
        {
            get { return mVisibility; }
            set { SetProperty(ref mVisibility, value); }
        }

        private string mSaveAs;

        public string SaveAs
        {
            get { return mSaveAs; }
            set { SetProperty(ref mSaveAs, value); }
        }

        private bool mSaveIsEnabled;

        public bool SaveIsEnabled
        {
            get { return mSaveIsEnabled; }
            set { SetProperty(ref mSaveIsEnabled, value); }
        }

        private bool mNewIsEnabled;

        public bool NewIsEnabled
        {
            get { return mNewIsEnabled; }
            set { SetProperty(ref mNewIsEnabled, value); }
        }

        private bool mDeleteIsEnabled;

        public bool DeleteIsEnabled
        {
            get { return mDeleteIsEnabled; }
            set { SetProperty(ref mDeleteIsEnabled, value); }
        }

        private string mAddress;

        public string Address
        {
            get { return mAddress; }
            set { SetProperty(ref mAddress, value); }
        }
        private string mUsername;

        public string Username
        {
            get { return mUsername; }
            set { SetProperty(ref mUsername, value); }
        }

        private string mPassword;

        public string Password
        {
            get { return mPassword; }
            set { SetProperty(ref mPassword, value); }
        }

        private string mDatabase;

        public string Database
        {
            get { return mDatabase; }
            set { SetProperty(ref mDatabase, value); }
        }

        private bool mLoginIsEnabled;

        public bool LoginIsEnabled
        {
            get { return mLoginIsEnabled; }
            set { SetProperty(ref mLoginIsEnabled, value); }
        }

        private string mLoginContent;

        public string LoginContent
        {
            get { return mLoginContent; }
            set { SetProperty(ref mLoginContent, value); }
        }

        private string mSelectedItem;

        public string SelectedItem
        {
            get { return mSelectedItem; }
            set { SetProperty(ref mSelectedItem, value); }
        }
    }
}

using System;
using Microsoft.Win32;

namespace Neo.UI.Components
{
    /// <summary>
    /// Interaction logic for CheckedMessageBox.xaml
    /// </summary>
    public partial class CheckedMessageBox
    {
        public const int TagSaveOnWdlGeneration = 1;

        private bool mResult;

        private CheckedMessageBox()
        {
            InitializeComponent();
        }

        public static bool Show(string title, string message, int tag)
        {
            var regKey = Registry.CurrentUser.OpenSubKey("Software\\Neo\\MessageBoxes\\States") ??
                         Registry.CurrentUser.CreateSubKey("Software\\Neo\\MessageBoxes\\States");

            if (regKey != null)
            {
                var curValue = regKey.GetValue("State_" + tag, null);
                if (curValue != null)
                {
                    try
                    {
                        return Convert.ToBoolean(curValue);
                    }
                    catch (FormatException)
                    {
                        
                    }
                }
            }

            var box = new CheckedMessageBox
            {
                Title = title,
                MessageTextBlock = {Text = message}
            };

            box.YesButton.Click += (sender, args) =>
            {
                if(box.ToggleBox.IsChecked ?? false)
                {
	                SetMessageBoxToValue(tag, title, true);
                }
	            box.mResult = true;
                box.Close();
            };

            box.NoButton.Click += (sender, args) =>
            {
                if (box.ToggleBox.IsChecked ?? false)
                {
	                SetMessageBoxToValue(tag, title, false);
                }
	            box.mResult = false;
                box.Close();
            };

            box.ShowDialog();
            return box.mResult;
        }

        private static void SetMessageBoxToValue(int tag, string title, bool result)
        {
            var regKey =
                Registry.CurrentUser.OpenSubKey("Software\\Neo\\MessageBoxes\\States",
                    RegistryKeyPermissionCheck.ReadWriteSubTree) ??
                Registry.CurrentUser.CreateSubKey("Software\\Neo\\MessageBoxes\\States",
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

            if (regKey == null)
            {
	            return;
            }

	        regKey.SetValue("State_" + tag, result ? "true" : "false");
            regKey.SetValue("Desc_" + tag, title);
        }
    }
}

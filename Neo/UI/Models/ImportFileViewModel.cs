using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using SharpDX.DXGI;
using Neo.IO;
using Neo.UI.Dialogs;
using Neo.Win32;
using Brushes = System.Windows.Media.Brushes;

namespace Neo.UI.Models
{
	internal enum ImportType
    {
        Texture,
        Raw,
        NotSupported
    }

    public class ImportFileViewModel
    {
        private readonly ImportFileDialog mDialog;

        public ImportFileViewModel()
        {
	        this.mDialog = new ImportFileDialog(this);
        }

        public void ShowModal()
        {
	        this.mDialog.ShowDialog();
        }

        public void Show()
        {
	        this.mDialog.Show();
        }

        public void HandleFileImport()
        {
            var importType = IsFileSupported();
            var sourceName = this.mDialog.PathTextBox.Text;
            var targetName = this.mDialog.TargetNameBox.Text;

            if (importType == ImportType.Texture)
            {
                using (var img = Image.FromFile(sourceName) as Bitmap)
                {
                    if (img == null)
                    {
	                    return;
                    }

	                using (var output = FileManager.Instance.GetOutputStream(targetName))
                    {
                        var texType = this.mDialog.TextureTypeBox.SelectedIndex;
                        var format = Format.BC1_UNorm;
                        var hasMips = true;
                        if (texType == 1)
                        {
	                        format = Format.BC3_UNorm;
                        }
                        else if (texType == 2)
                        {
                            format = Format.BC2_UNorm;
                            hasMips = false;
                        }

                        IO.Files.Texture.BlpWriter.Write(output, img, format, hasMips);
                    }
                }
            }
            else
            {
                using (var output = FileManager.Instance.GetOutputStream(targetName))
                {
                    using (var input = File.OpenRead(sourceName))
                    {
                        input.CopyTo(output);
                    }
                }
            }

	        this.mDialog.Close();
        }

        public void HandleFileImportSettings()
        {
            var importType = IsFileSupported();
            switch (importType)
            {
                case ImportType.NotSupported:
	                this.mDialog.PathErrorLabel.Text = "Sorry, this file cannot be imported";
	                this.mDialog.PathErrorLabel.Foreground = Brushes.Red;
                    return;

                case ImportType.Raw:
	                this.mDialog.PathErrorLabel.Text = "Info: File will be imported raw, no conversion";
	                this.mDialog.PathErrorLabel.Foreground = Brushes.Coral;
                    break;

                case ImportType.Texture:
	                this.mDialog.Height = 300;
	                this.mDialog.TextureSettingsPanel.Visibility = Visibility.Visible;
                    break;
            }

	        this.mDialog.PathErrorLabel.Text = "";
        }

        public unsafe void BrowseForFile()
        {
	        // TODO: why
            var dlg = (IFileOpenDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("{DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7}")));
            var data = new IntPtr[4];
            data[0] = Marshal.StringToBSTR("Images (jpg, gif, png, bmp, exif, tiffs)");
            data[1] = Marshal.StringToBSTR("*.jpg;*.jpeg;*.gif;*.png;*.bmp;*.exif;*.tiff");
            data[2] = Marshal.StringToBSTR("All Files (*.*)");
            data[3] = Marshal.StringToBSTR("*.*");

            fixed (IntPtr* filters = data)
            {
	            dlg.SetFileTypes(2, new IntPtr(filters));
            }

	        for (var i = 0; i < 4; ++i)
	        {
		        Marshal.FreeBSTR(data[i]);
	        }

	        if (dlg.Show(new WindowInteropHelper(this.mDialog).Handle) != 0)
	        {
		        return;
	        }

	        IShellItem item;
            dlg.GetResult(out item);
            if (item == null)
            {
	            return;
            }

	        var ptrOut = IntPtr.Zero;
            try
            {
                item.GetDisplayName(Sigdn.Filesyspath, out ptrOut);
	            this.mDialog.PathTextBox.Text = Marshal.PtrToStringUni(ptrOut);
            }
            catch (Exception)
            {
                item.GetDisplayName(Sigdn.Normaldisplay, out ptrOut);
	            this.mDialog.PathTextBox.Text = Marshal.PtrToStringUni(ptrOut);
            }
            finally
            {
                if (ptrOut != IntPtr.Zero)
                {
	                Marshal.FreeCoTaskMem(ptrOut);
                }
            }

	        this.mDialog.TargetNamePanel.Visibility = Visibility.Visible;
	        this.mDialog.TargetNameBox.Text = Path.GetFileName(this.mDialog.PathTextBox.Text) ?? "";
	        this.mDialog.Height = 200;
        }

        private ImportType IsFileSupported()
        {
            var file = this.mDialog.PathTextBox.Text;
            var extension = Path.GetExtension(file);
            if(string.IsNullOrEmpty(extension))
            {
	            return ImportType.NotSupported;
            }

	        extension = extension.ToLowerInvariant();

            var imageExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".gif",
                ".exif",
                ".png",
                ".bmp"
            };

            return imageExtensions.Any(ext => string.Equals(ext, extension)) ? ImportType.Texture : ImportType.Raw;
        }
    }
}

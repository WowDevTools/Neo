using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Resources;
using WoWEditor6.UI.Components;
using WoWEditor6.Win32;

namespace WoWEditor6.UI.Views
{
    class SplashView : IView
    {
        private Vector2 mSize;

        private readonly Label mAppTitle;
        private readonly Label mDescription;
        private readonly EditBox mInputPath;
        private readonly Button mBrowseButton;
        private readonly Button mRegistryButton;
        private readonly Button mConfirmButton;

        public SplashView()
        {
            mAppTitle = new Label
            {
                Color = Brushes.White,
                Position = new Vector2(30, 30),
                Text = Strings.SplashView_AppTitle,
                Size = new Vector2(float.MaxValue, 40.0f),
                FontSize = 35.0f
            };

            mDescription = new Label
            {
                Color = Brushes.White,
                Position = new Vector2(30, 110),
                Text = Strings.SplashView_AppDescription,
                FontSize = 20.0f,
                Multiline = true
            };

            mInputPath = new EditBox
            {
                Width = 500.0f
            };

            mBrowseButton = new Button
            {
                Text = "...",
                Size = new Vector2(50, 30)
            };

            mRegistryButton = new Button
            {
                Text = "From Registry",
                Size = new Vector2(150, 30)
            };

            mConfirmButton = new Button
            {
                Text = "Load!",
                Size = new Vector2(100, 30)
            };

            mBrowseButton.OnClick += BrowseFolder;
            mRegistryButton.OnClick += LoadFromRegistry;
            mConfirmButton.OnClick += InitFilesystem;
        }

        public void OnShow()
        {

        }

        public void OnRender(RenderTarget target)
        {
            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);
            mAppTitle.OnRender(target);
            mDescription.OnRender(target);
            mInputPath.OnRender(target);
            mBrowseButton.OnRender(target);
            mRegistryButton.OnRender(target);
            mConfirmButton.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            mInputPath.OnMessage(message);
            mBrowseButton.OnMessage(message);
            mRegistryButton.OnMessage(message);
            mConfirmButton.OnMessage(message);
        }

        public void OnResize(Vector2 newSize)
        {
            mSize = newSize;
            mDescription.Size = new Vector2(newSize.X - 60, float.MaxValue);
            mInputPath.Position = new Vector2(30.0f, 100 + mDescription.TextHeight + 60);
            mBrowseButton.Position = new Vector2(535.0f, 100 + mDescription.TextHeight + 60);
            mRegistryButton.Position = new Vector2(590.0f, 100 + mDescription.TextHeight + 60);
            mConfirmButton.Position = new Vector2(30.0f, 100 + mDescription.TextHeight + 60 + 40);
        }

        private void BrowseFolder(Button button)
        {
            var fd = (IFileOpenDialog)
                Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("{DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7}")));

            Fos options;
            fd.GetOptions(out options);
            options |= Fos.FosPickfolders;
            fd.SetOptions(options);
           

            var wnd = InterfaceManager.Instance.Window;
            var result = 0;
            if (wnd.InvokeRequired)
                wnd.Invoke(new Action(() => result = fd.Show(wnd.Handle)));
            else
                result = fd.Show(IntPtr.Zero);

            if (result != 0)
                return;

            IShellItem item;
            fd.GetResult(out item);
            if (item == null)
                return;

            var ptrOut = IntPtr.Zero;
            try
            {
                item.GetDisplayName(Sigdn.Filesyspath, out ptrOut);
                mInputPath.Text = Marshal.PtrToStringUni(ptrOut);
            }
            catch(Exception)
            {
                item.GetDisplayName(Sigdn.Normaldisplay, out ptrOut);
                mInputPath.Text = Marshal.PtrToStringUni(ptrOut);
            }
            finally
            {
                if (ptrOut != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(ptrOut);
            }
        }

        private void LoadFromRegistry(Button button)
        {
            var result = LoadFromKey(Registry.CurrentUser) ?? LoadFromKey(Registry.LocalMachine);

            if (result == null)
                return;

            mInputPath.Text = result;
        }

        private static string LoadFromKey(RegistryKey baseKey)
        {
            var rootKey = IntPtr.Size == 8
                ? "Software\\WoW6432Node\\Blizzard Entertainment\\World of Warcraft"
                : "Software\\Blizzard Entertainment\\World of Warcraft";

            try
            {
                var wowKey = baseKey.OpenSubKey(rootKey);
                return wowKey?.GetValue("InstallPath") as string;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private void InitFilesystem(Button button)
        {
            if (mInputPath.Text.Length == 0 || Directory.Exists(mInputPath.Text) == false)
                return;

            IO.FileManager.Instance.DataPath = mInputPath.Text;
            InterfaceManager.Instance.UpdateState(Scene.AppState.FileSystemInit);
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WoWEditor6.IO.CASC;

namespace WoWEditor6.IO
{
    class FileManager
    {
        public static FileManager Instance { get; private set; }

        public IFileProvider Provider { get; private set; }
        public IFileListing FileListing { get; set; }
        public string DataPath { get; set; }

        public event Action LoadComplete;
        public FileDataVersion Version { get; private set; }

        static FileManager()
        {
            Instance = new FileManager();
        }

        FileManager()
        {
            FileListing = new DefaultFileListing();
        }

        public void ExportFile(string path)
        {
            using (var file = Provider.OpenFile(path))
            {
                if (file == null)
                    return;

                var extension = Path.GetExtension(path);
                if (string.IsNullOrEmpty(extension))
                    return;

                if (extension.StartsWith("."))
                    extension = extension.Substring(1);

                extension = extension.ToLowerInvariant();

                switch (extension)
                {
                    case "blp":
                        ExportTexture(file, path);
                        break;

                    default:
                        DefaultExport(file, path);
                        break;
                }
            }
        }

        public Stream GetExportStream(string path)
        {
            
            var fullPath = Path.Combine(Properties.Settings.Default.ExportPath ?? ".\\Export", path);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? ".");
                return File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Stream GetOutputStream(string path)
        {
            var fullPath = Path.Combine(Properties.Settings.Default.OutputPath ?? ".\\Output", path);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? ".");
                return File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public Stream GetExistingFile(string path)
        {
            try
            {
                var fullPath = Path.Combine(Properties.Settings.Default.OutputPath ?? ".\\Output", path);
                if (!File.Exists(fullPath))
                    return null;

                using (var strm = File.OpenRead(fullPath))
                {
                    var retStream = new MemoryStream();
                    strm.CopyTo(retStream);
                    retStream.Position = 0;
                    return retStream;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void InitFromPath()
        {
            if(string.IsNullOrEmpty(DataPath))
                throw new InvalidOperationException("Cannot initialize file system without a path");

            if (File.Exists(Path.Combine(DataPath, ".build.info")))
            {
                Files.Terrain.AdtFactory.Instance.Version = FileDataVersion.Warlords;
                Files.Models.ModelFactory.Instance.Version = FileDataVersion.Warlords;
                Files.Sky.SkyManager.InitVersion(FileDataVersion.Warlords);
                Version = FileDataVersion.Warlords;

                var mgr = new CASC.FileManager();
                mgr.LoadComplete += () =>
                {
                    Storage.DbcStorage.Initialize();
                    if (LoadComplete != null)
                        LoadComplete();
                };

                Provider = mgr;
                mgr.Initialize(DataPath);
            }
            else
                InitMpq();

            Storage.DbcStorage.BuildModelCache();
            UI.ThumbnailCache.Reload(); //Load thumbnails of models
        }

        private void InitMpq()
        {
            var version = FileVersionInfo.GetVersionInfo(Path.Combine(DataPath, "Wow.exe"));
            if (version.FilePrivatePart > 13000 || version.FilePrivatePart < 9000)
                throw new NotImplementedException("MPQ is only implemented for WOTLK (builds 9000 - 13000)");

            Files.Terrain.AdtFactory.Instance.Version = FileDataVersion.Lichking;
            Files.Models.ModelFactory.Instance.Version = FileDataVersion.Lichking;
            Files.Sky.SkyManager.InitVersion(FileDataVersion.Lichking);
            Version = FileDataVersion.Lichking;

            var mgr = new MPQ.FileManager();
            mgr.LoadComplete += () =>
            {
                Storage.DbcStorage.Initialize();
                if (LoadComplete != null)
                    LoadComplete();
            };

            Provider = mgr;
            mgr.InitFromPath(DataPath);
        }

        private void DefaultExport(Stream input, string path)
        {
            using (var output = GetExportStream(path))
                input.CopyTo(output);
        }

        private unsafe void ExportTexture(Stream input, string path)
        {
            var origPath = path;
            path = Path.ChangeExtension(path, "png");
            var loadInfo = Files.Texture.TextureLoader.LoadToArgbImage(input);
            if (loadInfo == null)
            {
                input.Position = 0;
                using (var output = GetExportStream(origPath))
                {
                    if (output == null)
                        return;

                    input.CopyTo(output);
                }

                return;
            }

            using (var output = GetExportStream(path))
            {
                if (output == null)
                    return;

                using (var bmp = new Bitmap(loadInfo.Width, loadInfo.Height, PixelFormat.Format32bppArgb))
                {
                    var bmpd = bmp.LockBits(new Rectangle(0, 0, loadInfo.Width, loadInfo.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);

                    fixed (byte* srcData = loadInfo.Layers[0])
                        UnsafeNativeMethods.CopyMemory((byte*) bmpd.Scan0.ToPointer(), srcData,
                            loadInfo.Width * loadInfo.Height * 4);

                    bmp.UnlockBits(bmpd);
                    bmp.Save(output, ImageFormat.Png);
                }
            }
        }
    }
}

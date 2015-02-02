using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WoWEditor6.IO.MPQ
{
	enum Locale
	{
		frFR,
		deDE,
		enGB,
		enUS,
		itIT,
		koKR,
		zhCN,
		zhTW,
		ruRU,
		esES,
		esMX,
		ptBR
	}

	class FileManager : IFileProvider
	{
		public event Action LoadComplete;

		private Locale mLocale;
		private List<Archive> mArchives = new List<Archive>();

		public async void InitFromPath(string dataPath)
		{
			await Task.Factory.StartNew(() =>
			{
				var baseArchives = GetAllArchives(Path.Combine(dataPath, "Data"));
				if (GetLocale(Path.Combine(dataPath, "Data")))
					baseArchives = baseArchives.Union(GetAllArchives(Path.Combine(dataPath, "Data", mLocale.ToString())));

				var archives = baseArchives.ToList();
				archives.Sort(Compare);
				mArchives = archives;

				LoadComplete?.Invoke();
			});
		}

		public Stream OpenFile(string path)
		{
			return mArchives.Select(archive => archive.Open(path)).FirstOrDefault(ret => ret != null);
		}

		public bool Exists(string path)
		{
			return mArchives.Any(archive => archive.Contains(path));
		}

		private static int Compare(Archive a1, Archive a2)
		{
			var name1 = Path.GetFileName(a1.Name)?.ToLowerInvariant();
			var name2 = Path.GetFileName(a2.Name)?.ToLowerInvariant();

			if (name1 == null && name2 != null)
				return 1;

			if (name1 != null && name2 == null)
				return -1;

			if (name1 == null)
				return 0;

			var isLocale1 = name1.IndexOf("locale", StringComparison.Ordinal) >= 0 ||
			                name1.IndexOf("speech", StringComparison.Ordinal) >= 0 ||
			                name1.IndexOf("base", StringComparison.Ordinal) >= 0;

			var isLocale2 = name2.IndexOf("locale", StringComparison.Ordinal) >= 0 ||
			                name2.IndexOf("speech", StringComparison.Ordinal) >= 0 ||
			                name2.IndexOf("base", StringComparison.Ordinal) >= 0;

			if (isLocale1 == false && isLocale2)
				return -1;
			if (isLocale2 == false && isLocale1)
				return 1;

			if(isLocale1)
			{
				if (name1.Contains("expansion") && name2.Contains("locale"))
					return -1;
				if (name2.Contains("expansion") && name1.Contains("locale"))
					return 1;
			}

			var isPatch1 = name1.Contains("patch");
			var isPatch2 = name2.Contains("patch");

			if (isPatch1 == false && isPatch2)
				return 1;
			if (isPatch2 == false && isPatch1)
				return -1;

			if (isPatch1 == false)
				return -string.Compare(name1, name2, StringComparison.Ordinal);

			var i1 = name1.IndexOf("patch", StringComparison.Ordinal);
			var i2 = name2.IndexOf("patch", StringComparison.Ordinal);

			var patchNum1 = name1.Substring(i1 + 6);
			var patchNum2 = name2.Substring(i2 + 6);

			return -string.Compare(patchNum1, patchNum2, StringComparison.OrdinalIgnoreCase);
		}

		private bool GetLocale(string dataPath)
		{
			var values = Enum.GetValues(typeof (Locale));
			foreach(var value in values)
			{
				var dir = Path.Combine(dataPath, value.ToString());
				if (!Directory.Exists(dir))
					continue;

				mLocale = (Locale) value;
				return true;
			}

			return false;
		}

		private static IEnumerable<Archive> GetAllArchives(string path)
		{
			var files = Directory.GetFiles(path, "*.mpq");
			var ret = new List<Archive>();

			foreach(var file in files)
			{
				IntPtr handle;
				if (Imports.SFileOpenArchive(file, 0, 0x100, out handle) == false)
				{
					Log.Warning("Archive failed: " + file);
					continue;
				}
				Log.Debug("Loaded " + file);
				ret.Add(new Archive(handle, file));
			}

			return ret;
		}
	}
}

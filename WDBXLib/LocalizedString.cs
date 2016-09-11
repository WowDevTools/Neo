using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDBXLib
{
    public class LocalizedString
    {
        public string String;
        public uint Mask;
        private string[] _strings;

        public LocalizedString(List<string> s, uint m)
        {
            _strings = s.ToArray();
            Mask = m;

            String = _strings.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;
        }

        public object[] GetRawData()
        {
            var data = _strings.Cast<object>().ToList();
            data.Add((object)Mask);
            return data.ToArray();
        }

        public static implicit operator String(LocalizedString s)
        {
            return s.String;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.IO.CASC
{
    class KeyValueConfig
    {
        private readonly Dictionary<string, IEnumerable<string>> mValues = new Dictionary<string, IEnumerable<string>>();

        public IEnumerable<string> this[string key]
        {
            get
            {
                IEnumerable<string> ret;
                return mValues.TryGetValue(key.ToUpperInvariant(), out ret) ? ret : new string[0];
            }
        }

        public void Load(StreamReader reader)
        {
            try
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("#"))
                    {
	                    continue;
                    }

	                var tokens = line.Split('=');
                    if (tokens.Length != 2)
                    {
	                    continue;
                    }

	                var values = tokens[1].Trim().Split(' ').Where(s => string.IsNullOrEmpty(s) == false);
                    mValues[tokens[0].Trim().ToUpperInvariant()] = values;
                }
            }
            catch (IOException)
            {
            }
        }
    }
}

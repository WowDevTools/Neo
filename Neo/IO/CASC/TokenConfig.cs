using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WoWEditor6.IO.CASC
{
    class TokenConfig
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

        public void Load(StreamReader stream)
        {
            try
            {
                var firstLine = true;
                string line;
                var valuesOrdered = new List<List<string>>();

                while ((line = stream.ReadLine()) != null)
                {
                    var tokens = line.Split('|');
                    if (firstLine)
                    {
                        firstLine = false;
                        foreach (var token in tokens)
                        {
                            var subTokens = token.Split('!');
                            var elems = new List<string>();
                            mValues.Add(subTokens.First().ToUpperInvariant(), elems);
                            valuesOrdered.Add(elems);
                        }
                    }
                    else
                    {
                        if (tokens.Length != valuesOrdered.Count)
                            throw new IOException("Invalid format of Config file");

                        for (var i = 0; i < tokens.Length; ++i)
                            valuesOrdered[i].Add(tokens[i]);
                    }
                }
            }
            catch (IOException)
            {
            }
        }
    }
}

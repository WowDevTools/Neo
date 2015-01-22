using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Models
{
    class ModelFactory
    {
        public static ModelFactory Instance { get; } = new ModelFactory();

        public FileDataVersion Version { get; set; }

        public WmoRoot CreateWmo()
        {
            switch(Version)
            {
                case FileDataVersion.Warlords:
                    return new WoD.WmoRoot();

                default:
                    throw new NotSupportedException("Version not yet supported for WMO models");
            }
        }
    }
}

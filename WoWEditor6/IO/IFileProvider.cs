using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO
{
    interface IFileProvider
    {
        Stream OpenFile(string path);
    }
}

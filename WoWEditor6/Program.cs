using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new IO.CASC.FileManager();
            mgr.Initialize(@"E:\Save_Deskt_24_9_14\Program Files (x86)\World of Warcraft");
        }
    }
}

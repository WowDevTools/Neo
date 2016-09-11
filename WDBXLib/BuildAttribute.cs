using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDBXLib
{
    class BuildAttribute : Attribute
    {
        public readonly int Build;

        public BuildAttribute(int build)
        {
            this.Build = build;
        }
    }
}

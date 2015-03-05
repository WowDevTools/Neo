using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Terrain.WoD
{
    class MapQueryImpl : IMapAreaQueryImplementation
    {
        public void Execute(MapAreaQuery query)
        {

            foreach (var chunk in query.RequestedChunks)
            {

            }
        }
    }
}

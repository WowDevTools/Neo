using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.IO.Files.Terrain
{
	public interface IMapAreaQueryImplementation
    {
        void Execute(MapAreaQuery query);
    }
}

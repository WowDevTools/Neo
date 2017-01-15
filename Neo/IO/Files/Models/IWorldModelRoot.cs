using System;
using System.Collections.Generic;
using SlimTK;

namespace Neo.IO.Files.Models
{
	public interface IWorldModelRoot : IDisposable
    {
        string FileName { get; }
	    BoundingBox BoundingBox { get; }
	    IList<IWorldModelGroup> Groups { get; }

	    Graphics.Texture GetTexture(int index);
        WmoMaterial GetMaterial(int index);
        bool Load(string fileName);
    }
}

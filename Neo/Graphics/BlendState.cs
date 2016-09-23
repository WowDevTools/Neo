using System;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class BlendState
    {
        public bool BlendEnabled
        {
	        get;
	        set;
        }

	    public BlendingFactorSrc SourceBlend
	    {
		    get;
		    set;
	    }

	    public BlendingFactorDest DestinationBlend
	    {
		    get; set;
	    }

	    public BlendingFactorSrc SourceAlphaBlend
	    {
		    get; set;
	    }

	    public BlendingFactorDest DestinationAlphaBlend
	    {
		    get; set;
	    }

	    public BlendState()
        {

        }

	    public void Apply()
	    {
		    if (BlendEnabled)
		    {
			    GL.Enable(EnableCap.Blend);
		    }
		    else
		    {
			    GL.Disable(EnableCap.Blend);
		    }

			GL.BlendFuncSeparate(SourceBlend, DestinationBlend, SourceAlphaBlend, DestinationAlphaBlend);
	    }
    }
}

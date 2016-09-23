using System;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
    public class DepthState
    {
        public bool DepthEnabled
        {
	        get;
	        set;
        }

        public bool DepthWriteEnabled
        {
	        get;
	        set;
        }

        public DepthState()
        {

        }

	    public void Apply()
	    {
		    if (DepthEnabled)
		    {
			    GL.Enable(EnableCap.DepthTest);
		    }
		    else
		    {
			    GL.Disable(EnableCap.DepthTest);
		    }

		    GL.DepthMask(DepthWriteEnabled);
	    }
    }
}

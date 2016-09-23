using System;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class RasterState
    {
        public bool CullEnabled
        {
	        get;
	        set;
        }

	    public CullFaceMode CullingMode
	    {
		    get;
		    set;
	    }

	    public FrontFaceDirection CullingWindingDirection
	    {
		    get;
		    set;
	    }

        public PolygonMode RenderingMode
        {
	        get;
	        set;
        }

        public RasterState()
        {

        }

	    public void Apply()
	    {
		    if (CullEnabled)
		    {
			    GL.Enable(EnableCap.CullFace);
			    GL.CullFace(this.CullingMode);
			    GL.FrontFace(this.CullingWindingDirection);
		    }
		    else
		    {
			    GL.Disable(EnableCap.CullFace);
		    }

		    GL.PolygonMode(MaterialFace.FrontAndBack, this.RenderingMode);
	    }
    }
}

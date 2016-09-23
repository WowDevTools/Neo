using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="RasterState"/> class represents a set of rasterization settings.
	/// These can be reused and moved around as needed for different objects.
	///
	/// In order to use the settings, set the different properties to your required values.
	/// After that, the <see cref="Activate"/> function must be called, which will set the blend state
	/// for the current thread context.
	/// </summary>
	public class RasterState
    {
	    /// <summary>
	    /// Whether or not backface culling is enabled.
	    /// </summary>
	    public bool BackfaceCullingEnabled
        {
	        get;
	        set;
        }

	    /// <summary>
	    /// The culling mode to be used (if backface culling is enabled).
	    /// </summary>
	    public CullFaceMode CullingMode
	    {
		    get;
		    set;
	    }

	    /// <summary>
	    /// The winding direction of the faces to be used (if backface culling is enabled).
	    /// </summary>
	    public FrontFaceDirection CullingWindingDirection
	    {
		    get;
		    set;
	    }

	    /// <summary>
	    /// The rendering mode of polygons while this state is active, such as Wireframe, Filled or Point.
	    /// </summary>
	    public PolygonMode RenderingMode
        {
	        get;
	        set;
        }

	    /// <summary>
	    /// Creates a new instance of the <see cref="RasterState"/> class with a set of default values.
	    /// </summary>
	    public RasterState()
	    {
		    this.BackfaceCullingEnabled = true;
		    this.CullingMode = CullFaceMode.Back;
		    this.CullingWindingDirection = FrontFaceDirection.Cw;
		    this.RenderingMode = PolygonMode.Fill;
	    }

	    /// <summary>
		/// Creates a new instance of the <see cref="RasterState"/> class, and initializes it with a set of
	    /// rasterization settings.
	    /// </summary>
		/// <param name="isBackfaceCullingEnabled">Wheter or not backface culling should be enabled.</param>
		/// <param name="cullingMode">The culling mode which should be used.</param>
		/// <param name="windingDirection">The winding direction of the faces.</param>
		/// <param name="renderingMode">The polygon rendering mode for this state.</param>
        public RasterState(bool isBackfaceCullingEnabled, CullFaceMode cullingMode, FrontFaceDirection windingDirection,
	        PolygonMode renderingMode)
        {
	        this.BackfaceCullingEnabled = isBackfaceCullingEnabled;
	        this.CullingMode = cullingMode;
	        this.CullingWindingDirection = windingDirection;
	        this.RenderingMode = renderingMode;
        }

	    /// <summary>
	    /// Activates this state and applies the settings in this state to the current thread context, effectively
	    /// enabling them.
	    /// </summary>
	    public void Activate()
	    {
		    if (BackfaceCullingEnabled)
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

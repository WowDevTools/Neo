using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="BlendState"/> class represents a set of blending settings.
	/// These can be reused and moved around as needed for different objects.
	///
	/// In order to use the settings, set the different properties to your required values.
	/// After that, the <see cref="Activate"/> function must be called, which will set the blend state
	/// for the current thread context.
	/// </summary>
	public class BlendState
    {
	    /// <summary>
		/// Whether or not blending is enabled at all.
		/// </summary>
        public bool BlendEnabled
        {
	        get;
	        set;
        }

	    /// <summary>
	    /// The blending factor to be used for the source RGB values.
	    /// </summary>
	    public BlendingFactorSrc SourceBlend
	    {
		    get;
		    set;
	    }

	    /// <summary>
	    /// The blending factor to be used for the destination RGB values.
	    /// </summary>
	    public BlendingFactorDest DestinationBlend
	    {
		    get;
		    set;
	    }

	    /// <summary>
	    /// The blending factor to be used for the source alpha values.
	    /// </summary>
	    public BlendingFactorSrc SourceAlphaBlend
	    {
		    get;
		    set;
	    }

	    /// <summary>
	    /// The blending factor to be used for the destination alpha values.
	    /// </summary>
	    public BlendingFactorDest DestinationAlphaBlend
	    {
		    get;
		    set;
	    }

	    /// <summary>
	    /// Creates a new instance of the <see cref="DepthState"/> class with a set of default values.
	    /// </summary>
	    public BlendState()
	    {
		    this.BlendEnabled = true;

		    this.SourceBlend = BlendingFactorSrc.SrcAlpha;
		    this.DestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;

		    this.SourceAlphaBlend = BlendingFactorSrc.SrcAlpha;
		    this.DestinationAlphaBlend = BlendingFactorDest.OneMinusSrcAlpha;
	    }

	    /// <summary>
		/// Creates a new instance of the <see cref="BlendState"/> class, and initializes it with a set of
		/// blend settings.
		/// </summary>
		/// <param name="isBlendEnabled">Wheter or not blending is enabled.</param>
		/// <param name="sourceBlend">The blending factor for the source RGB values.</param>
		/// <param name="destinationBlend">The blending factor for the destination RGB values.</param>
		/// <param name="sourceAlphaBlend">The blending factor for the source alpha values.</param>
		/// <param name="destinationAlphaBlend">The blending factor for the destination alpha values.</param>
	    public BlendState(bool isBlendEnabled, BlendingFactorSrc sourceBlend, BlendingFactorDest destinationBlend,
		    BlendingFactorSrc sourceAlphaBlend, BlendingFactorDest destinationAlphaBlend)
	    {
		    this.BlendEnabled = isBlendEnabled;

		    this.SourceBlend = sourceBlend;
		    this.DestinationBlend = destinationBlend;

		    this.SourceAlphaBlend = sourceAlphaBlend;
		    this.DestinationAlphaBlend = destinationAlphaBlend;
	    }

	    /// <summary>
	    /// Activates the state and applies the settings in this state to the current thread context, effectively
	    /// enabling them.
	    /// </summary>
	    public void Activate()
	    {
		    if (BlendEnabled)
		    {
			    GL.Enable(EnableCap.Blend);
		    }
		    else
		    {
			    GL.Disable(EnableCap.Blend);
		    }

			GL.BlendFuncSeparate(
				SourceBlend,
				DestinationBlend,
				SourceAlphaBlend,
				DestinationAlphaBlend
			);
	    }
    }
}

using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="DepthState"/> class represents a set of depth test settings.
	/// These can be reused and moved around as needed for different objects.
	///
	/// In order to use the settings, set the different properties to your required values.
	/// After that, the <see cref="Activate"/> function must be called, which will set the blend state
	/// for the current thread context.
	/// </summary>
	public class DepthState
    {
	    /// <summary>
		/// Wheter or not depth testing is enabled.
		/// </summary>
        public bool DepthEnabled
        {
	        get;
	        set;
        }

	    /// <summary>
	    /// Wheter or not the depth buffer can be written to.
	    /// </summary>
	    public bool DepthWriteEnabled
        {
	        get;
	        set;
        }

	    /// <summary>
	    /// Creates a new instance of the <see cref="DepthState"/> class with a set of default values.
	    /// </summary>
	    public DepthState()
	    {
		    this.DepthEnabled = true;
		    this.DepthWriteEnabled = false;
	    }

	    /// <summary>
		/// Creates a new instance of the <see cref="DepthState"/> class, and initializes it with a set of
	    /// depth settings.
	    /// </summary>
		/// <param name="isDepthEnabled">Wheter or not depth testing should be enabled.</param>
		/// <param name="isDepthWriteEnabled">Wheter or not writing to the depth buffer should be enabled.</param>
        public DepthState(bool isDepthEnabled, bool isDepthWriteEnabled)
	    {
		    this.DepthEnabled = isDepthEnabled;
		    this.DepthWriteEnabled = isDepthWriteEnabled;
	    }

	    /// <summary>
	    /// Activates this state and applies the settings in this state to the current thread context, effectively
	    /// enabling them.
	    /// </summary>
	    public void Activate()
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

using System;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public sealed class ShaderProgram : IDisposable
	{
		private int ShaderProgramID
		{
			get;
		}

        public ShaderProgram(int shaderProgramID)
        {
	        this.ShaderProgramID = shaderProgramID;
        }

		public void Bind()
		{
			GL.UseProgram(this.ShaderProgramID);
		}

        public void SetVertexTexture(int slot, Texture texture)
        {

        }

        public void SetVertexTextures(int slot, params Texture[] textures)
        {

        }

        public void SetFragmentTexture(int slot, Texture texture)
        {

        }

		public void SetFragmentTextures(int slot, params Texture[] textures)
        {

        }

        public void SetVertexUniformBuffer(int slot, UniformBuffer buffer)
        {

        }

        public void SetVertexUniformBuffers(int slot, params UniformBuffer[] buffers)
        {

        }

        public void SetFragmentUniformBuffer(int slot, UniformBuffer buffer)
        {

        }

        public void SetFragmentUniformBuffers(int slot, params UniformBuffer[] buffers)
        {

        }

        ~ShaderProgram()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
	        if (this.ShaderProgramID > -1)
	        {
		        GL.DeleteShader(this.ShaderProgramID);
	        }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

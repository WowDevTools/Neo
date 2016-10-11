using System;
using System.Collections.Generic;
using Neo.Resources;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The ShaderCache class is responsible for creating and maintaining the shaders in Neo.
	/// Whenever a shader is required, it is fetched from this class using <see cref="GetShaderProgram"/>.
	///
	/// This method checks the internal cache for the specified shader, creates it if neccesary, and returns
	/// it to the user.
	/// </summary>
	public sealed class ShaderCache : IDisposable
	{
		/// <summary>
		/// The singleton instance of the cache.
		/// </summary>
		private static readonly ShaderCache Instance = new ShaderCache();

		/// <summary>
		/// The internal cache of shader programs.
		/// </summary>
		private readonly Dictionary<NeoShader, ShaderProgram> ShaderProgramCache = new Dictionary<NeoShader, ShaderProgram>();

		/// <summary>
		/// Creates a new instance of the <see cref="ShaderCache"/> class. This constructor is not used beyond
		/// initializing the singleton.
		/// </summary>
		private ShaderCache()
		{
		}

		/// <summary>
		/// Gets the specified shader program from the cache. If this is the first time the specified shader is
		/// requested, it will be created.
		/// </summary>
		/// <param name="neoShader">The shader to fetch.</param>
		/// <returns>The<see cref="ShaderProgram"/> for the specified shader.</returns>
		public static ShaderProgram GetShaderProgram(NeoShader neoShader)
		{
			return Instance.InternalGetShaderProgram(neoShader);
		}

		/// <summary>
		/// The internal implemenentation of <see cref="GetShaderProgram"/>. This checks the cache, creates the program
		/// if neccesary, and returns the corrent <see cref="ShaderProgram"/>
		/// </summary>
		/// <param name="neoShader">The shader to fetch.</param>
		/// <returns>The<see cref="ShaderProgram"/> for the specified shader.</returns>
		private ShaderProgram InternalGetShaderProgram(NeoShader neoShader)
		{
			if (HasCachedShaderProgram(neoShader))
			{
				return ShaderProgramCache[neoShader];
			}
			else
			{
				CreateCachedShaderProgram(neoShader);

				if (HasCachedShaderProgram(neoShader))
				{
					return ShaderProgramCache[neoShader];
				}
				else
				{
					return new ShaderProgram(-1);
				}
			}
		}

		/// <summary>
		/// Determines whether or not the specified shader already exists in the cache.
		/// </summary>
		/// <param name="neoShader">The shader to check.</param>
		/// <returns><value>true</value> if it exists; otherwise <value>false</value>.</returns>
		private bool HasCachedShaderProgram(NeoShader neoShader)
		{
			return this.ShaderProgramCache.ContainsKey(neoShader);
		}

		/// <summary>
		/// Creates a cached <see cref="ShaderProgram"/> for the specified shader.
		/// </summary>
		/// <param name="neoShader">The shader to create.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if an unknown shader is passed to the function.
		/// </exception>
		private void CreateCachedShaderProgram(NeoShader neoShader)
		{
			int shaderProgramID = GL.CreateProgram();
			int vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
			int fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

			string vertexShaderSource;
			string fragmentShaderSource;
			switch (neoShader)
			{
				case NeoShader.BoundingBox:
				{
					vertexShaderSource = Shaders.BoundingBoxVertex;
					fragmentShaderSource = Shaders.BoundingBoxFragment;
					break;
				}
				case NeoShader.M2Portrait:
				{
					vertexShaderSource = Shaders.M2VertexPortrait;
					fragmentShaderSource = Shaders.M2FragmentPortrait;
					break;
				}
				case NeoShader.M2Instanced:
				{
					vertexShaderSource = Shaders.M2VertexInstanced;
					fragmentShaderSource = Shaders.M2FragmentSingle;
					break;
				}
				case NeoShader.M2Single:
				{
					vertexShaderSource = Shaders.M2VertexSingle;
					fragmentShaderSource = Shaders.M2FragmentSingle;
					break;
				}
				case NeoShader.MapLow:
				{
					vertexShaderSource = Shaders.MapLowVertex;
					fragmentShaderSource = Shaders.MapLowFragment;
					break;
				}
				case NeoShader.Sky:
				{
					vertexShaderSource = Shaders.SkyVertex;
					fragmentShaderSource = Shaders.SkyFragment;
					break;
				}
				case NeoShader.Terrain:
				{
					vertexShaderSource = Shaders.TerrainVertex;
					fragmentShaderSource = Shaders.TerrainFragment;
					break;
				}
				case NeoShader.TexturedQuad:
				{
					vertexShaderSource = Shaders.TexturedQuadVertex;
					fragmentShaderSource = Shaders.TexturedQuadFragment;
					break;
				}
				case NeoShader.WMO:
				{
					vertexShaderSource = Shaders.WmoVertex;
					fragmentShaderSource = Shaders.WmoFragment;
					break;
				}
				case NeoShader.WorldText:
				{
					vertexShaderSource = Shaders.WorldTextVertex;
					fragmentShaderSource = Shaders.WorldTextFragment;
					break;
				}
				case NeoShader.WorldText2D:
				{
					vertexShaderSource = Shaders.WorldTextVertexOrtho;
					fragmentShaderSource = Shaders.WorldTextFragment;
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException(nameof(neoShader), neoShader, null);
				}
			}

			bool vertexSuccess = CompileShaderSource(vertexShaderID, vertexShaderSource, ShaderType.VertexShader);
			bool fragmentSuccess = CompileShaderSource(fragmentShaderID, fragmentShaderSource, ShaderType.FragmentShader);

			// If either fail to compile, throw out the results
			if (!vertexSuccess || !fragmentSuccess)
			{
				GL.DeleteShader(vertexShaderID);
				GL.DeleteShader(fragmentShaderID);

				GL.DeleteProgram(shaderProgramID);
				return;
			}

			bool linkSuccess = LinkProgram(shaderProgramID, vertexShaderID, fragmentShaderID);

			// Standard cleanup that always happens
			GL.DetachShader(shaderProgramID, vertexShaderID);
			GL.DetachShader(shaderProgramID, fragmentShaderID);

			GL.DeleteShader(vertexShaderID);
			GL.DeleteShader(fragmentShaderID);

			// If the program fails to link, throw out the results
			if (!linkSuccess)
			{
				GL.DeleteProgram(shaderProgramID);
				return;
			}

			this.ShaderProgramCache.Add(neoShader, new ShaderProgram(shaderProgramID));
		}

		/// <summary>
		/// Compiles the provided <paramref name="shaderSource"/> for the specified <paramref name="shaderID"/> of the
		/// specified <paramref name="shaderType"/>. Should the shader fail to compile, relevant logging information
		/// will be printed to the console.
		/// </summary>
		/// <param name="shaderID">The OpenGL ID of the shader to compile the code for.</param>
		/// <param name="shaderSource">The GLSL shader source code.</param>
		/// <param name="shaderType">The type of shader to compile.</param>
		/// <returns><value>true</value> if the compilation was successful; otherwise, <value>false</value>.</returns>
		private static bool CompileShaderSource(int shaderID, string shaderSource, ShaderType shaderType)
		{
			int result;
			int compilationLogLength;

			Console.WriteLine($@"Compiling a ""{shaderType}"" shader...");
			GL.ShaderSource(shaderID, shaderSource);
			GL.CompileShader(shaderID);

			GL.GetShader(shaderID, ShaderParameter.CompileStatus, out result);
			GL.GetShader(shaderID, ShaderParameter.InfoLogLength, out compilationLogLength);

			if (compilationLogLength > 0)
			{
				string compilationLog;
				GL.GetShaderInfoLog(shaderID, out compilationLog);

				Console.WriteLine(compilationLog);
			}

			return result != 0;
		}

		/// <summary>
		/// Links the OpenGL shader program specified by <paramref name="programID"/> with a vertex shader specified by
		/// <paramref name="vertexShaderID"/> and a fragment shader specified by <paramref name="fragmentShaderID"/>.
		/// Should the program fail to link with the shaders, relevant logging information will be printed to the console.
		/// </summary>
		/// <param name="programID">The OpenGL ID of the shader program to link with the shaders.</param>
		/// <param name="vertexShaderID">The OpenGL ID of the vertex shader to link with the program.</param>
		/// <param name="fragmentShaderID">The OpenGL ID of the fragment shader to link with the program.</param>
		/// <returns><value>true</value> if linking was successful; otherwise, <value>false</value>.</returns>
		private static bool LinkProgram(int programID, int vertexShaderID, int fragmentShaderID)
		{
			int result;
			int linkingLogLength;

			Console.WriteLine(@"Linking shader program...");
			GL.AttachShader(programID, vertexShaderID);
			GL.AttachShader(programID, fragmentShaderID);
			GL.LinkProgram(programID);

			GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out result);
			GL.GetProgram(programID, GetProgramParameterName.InfoLogLength, out linkingLogLength);

			if (linkingLogLength > 0)
			{
				string linkingLog;
				GL.GetProgramInfoLog(programID, out linkingLog);

				Console.WriteLine(linkingLog);
			}

			return result != 0;
		}

		/// <summary>
		/// Destructor for the <see cref="ShaderCache"/> class. Disposes of unmanaged data.
		/// </summary>
		~ShaderCache()
		{
			Dispose(false);
		}

		/// <summary>
		/// Disposal function for the <see cref="ShaderCache"/> class. Deletes all cached shader programs
		/// from graphics memory.
		/// </summary>
		/// <param name="disposing">
		/// Whether or not this function was called via the <see cref="IDisposable"/> interface definition.
		/// </param>
		private void Dispose(bool disposing)
		{
			foreach (ShaderProgram shader in this.ShaderProgramCache.Values)
			{
				shader.Dispose();
			}
		}

		/// <summary>
		/// Disposal function for the <see cref="ShaderCache"/> class. Forwards the call to <see cref="Dispose(bool)"/>
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
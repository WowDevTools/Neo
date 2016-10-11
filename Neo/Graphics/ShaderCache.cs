using System;
using System.Collections;
using System.Collections.Generic;
using Neo.Resources;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class ShaderCache : IDisposable
	{
		private readonly Dictionary<NeoShader, ShaderProgram> ShaderProgramCache = new Dictionary<NeoShader, ShaderProgram>();

		public ShaderProgram GetShaderProgram(NeoShader neoShader)
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

		private bool HasCachedShaderProgram(NeoShader neoShader)
		{
			return this.ShaderProgramCache.ContainsKey(neoShader);
		}

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

			bool linkSuccess = LinkShader(shaderProgramID, vertexShaderID, fragmentShaderID);

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

		private static bool CompileShaderSource(int shaderID, string shaderSource, ShaderType shaderType)
		{
			int result;
			int compilationLogLength;

			Console.WriteLine($"Compiling a \"{shaderType}\" shader...");
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

		private static bool LinkShader(int programID, int vertexShaderID, int fragmentShaderID)
		{
			int result;
			int linkingLogLength;

			Console.WriteLine("Linking shader program...");
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

		~ShaderCache()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			foreach (ShaderProgram shader in this.ShaderProgramCache.Values)
			{
				shader.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
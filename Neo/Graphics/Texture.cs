using System;
using System.Collections.Generic;
using System.Drawing;
using Warcraft.BLP;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The texture wrapping type.
	/// </summary>
	public enum SamplerFlagType
	{
		/// <summary>
		/// Wrap on both the U and V axes.
		/// </summary>
		WrapBoth,

		/// <summary>
		/// Wrap on the U axis.
		/// </summary>
		WrapU,

		/// <summary>
		/// Wrap on the V axis.
		/// </summary>
		WrapV,

		/// <summary>
		/// Clamp all coordinates to the standard [0.0f, 1.0f] range.
		/// </summary>
		ClampBoth
	}

	/// <summary>
	/// The <see cref="Texture"/> class acts as a wrapper around an OpenGL texture ID, and serves
	/// to simplyfy loading of textures into graphics memory.
	///
	/// When the object is disposed, the memory occupied by the loaded texture and its
	/// mipmaps is marked as released on the GPU.
	///
	/// Textures initialize with a default texture, and can be used right away. However,
	/// for normal use it is recommended to initialize the texture with a <see cref="Bitmap"/>
	/// or <see cref="Warcraft.BLP"/> image object. Bitmaps always have their mipmaps generated,
	/// while <see cref="Warcraft.BLP"/> images can usually be directly loaded into memory as
	/// DXT-compressed images.
	///
	/// Should a texture fail to load or otherwise be uploaded to the GPU, the default texture will be
	/// used instead.
	/// </summary>
	public sealed class Texture : IDisposable
    {
	    /// <summary>
		/// The native handle on the GPU which refers to the texture data held by this object.
		/// </summary>
	    private uint glTextureID;


	    /// <summary>
		/// The reference path of the texture in the game files. Used for texture lookup in the
		/// resource cache.
		/// </summary>
	    public string TexturePath
	    {
		    get
		    {
			   return this.texturePath;
		    }
		    set
		    {
			    this.texturePath = value.ToUpperInvariant();
		    }
	    }
	    private string texturePath;

	    /// <summary>
		/// The format of the internal texture on the GPU.
		/// </summary>
	    public PixelInternalFormat Format;

	    /// <summary>
	    /// The absolute width of the texture. This corresponds to the width of the
	    /// largest mipmap.
	    /// </summary>
	    public uint Width;

	    /// <summary>
	    /// The absoltue height of the texture. This corresponds to the width of the
	    /// largest mipmap.
	    /// </summary>
	    public uint Height;

	    /// <summary>
	    /// The number of mipmaps for this texture.
	    /// </summary>
	    public uint MipLevels;

	    public TextureWrapMode AddressU
	    {
		    get;
		    set;
	    } = TextureWrapMode.Repeat;

	    public TextureWrapMode AddressV
	    {
		    get;
		    set;
	    } = TextureWrapMode.Repeat;

	    public TextureWrapMode AddressW
	    {
		    get;
		    set;
	    }  = TextureWrapMode.Repeat;

	    public TextureMinFilter MinFilter
	    {
		    get;
		    set;
	    } = TextureMinFilter.LinearMipmapLinear;

	    public TextureMagFilter MagFilter
	    {
		    get;
		    set;
	    } = TextureMagFilter.Linear;

	    /// <summary>
		/// Initializes a new <see cref="Texture"/> object and uploads an image, using the default
		/// texture as data.
		/// </summary>
        public Texture()
        {
	        // TODO: Implement
        }

	    /// <summary>
		/// Initializes a new <see cref="Texture"/> object and uploads the provided <paramref name="texture"/>
		/// as data. Depending on the format of the <see cref="BLP"/> image, mipmaps may be generated. If the
		/// <see cref="BLP"/> is in DXTC format, the stored mipmaps will be used instead.
		///
		/// Non-power-of-two images will be resized to the nearest power of two.
		/// </summary>
		/// <param name="texture">The texture object to use.</param>
		/// <param name="texturePath">The reference path to the texture in the game files.</param>
	    public Texture(BLP texture, string texturePath)
	    {
		    // TODO: Implement
	    }

	    /// <summary>
		/// Initializes a new <see cref="Texture"/> object and uploads the provided <paramref name="texture"/>
	    /// as data.
	    ///
	    /// Mipmaps will be generated for any input <see cref="Bitmap"/>.
	    /// Non-power-of-two images will be resized to the nearest power of two.
	    /// </summary>
	    /// <param name="texture">The texture object to use.</param>
	    /// <param name="texturePath">The reference path to the texture in the game files.</param>
	    public Texture(Bitmap texture, string texturePath)
	    {
		    // TODO: Implement
	    }

        ~Texture()
        {
            Dispose(false);
        }

	    public void LoadFromBLP(BLP image)
	    {
		    // TODO: Implement
	    }

	    public void LoadFromBitmap(Bitmap bitmap, bool generateMipMaps)
	    {
		    // TODO: Implement
	    }

		[Obsolete]
	    public void LoadFromLoadInfo(IO.Files.Texture.TextureLoadInfo loadInfo)
        {
            var totalMips = loadInfo.Layers.Count;
            if (loadInfo.GenerateMipMaps)
            {
                var l2H = (int)Math.Log(loadInfo.Height, 2);
                var l2W = (int)Math.Log(loadInfo.Width, 2);
                totalMips = Math.Min(l2H, l2W) + 1;
            }

            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = loadInfo.Format,
                Height = loadInfo.Height,
                Width = loadInfo.Width,
                MipLevels = totalMips,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = loadInfo.Usage
            };

            if (mTexture != gDefaultTexture)
            {
                if (mTexture != null)
                {
	                mTexture.Dispose();
                }
	            if (NativeView != null)
	            {
		            NativeView.Dispose();
	            }
            }

            var boxes = new DataBox[loadInfo.Layers.Count];
            var streams = new DataStream[loadInfo.Layers.Count];

            try
            {
                if (!loadInfo.GenerateMipMaps)
                {
                    for (var i = 0; i < loadInfo.Layers.Count; ++i)
                    {
                        streams[i] = new DataStream(loadInfo.Layers[i].Length, true, true);
                        streams[i].WriteRange(loadInfo.Layers[i]);
                        streams[i].Position = 0;
                        boxes[i] = new DataBox(streams[i].DataPointer, loadInfo.RowPitchs[i], 0);
                    }

                    mTexture = new Texture2D(mContext.Device, texDesc, boxes);
                }
                else
                {
                    texDesc.Usage = ResourceUsage.Default;
                    texDesc.OptionFlags |= ResourceOptionFlags.GenerateMipMaps;
                    texDesc.BindFlags |= BindFlags.RenderTarget;

                    mTexture = new Texture2D(mContext.Device, texDesc);
                    mContext.Context.UpdateSubresource(loadInfo.Layers[0], mTexture, 0, loadInfo.RowPitchs[0]);
                }

                var srvd = new ShaderResourceViewDescription
                {
                    Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                    Format = loadInfo.Format,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource
                    {
                        MipLevels = texDesc.MipLevels,
                        MostDetailedMip = 0
                    }
                };

                NativeView = new ShaderResourceView(mContext.Device, mTexture, srvd);
                if (loadInfo.GenerateMipMaps)
                {
	                mContext.Context.GenerateMips(NativeView);
                }
            }
            finally
            {
                foreach (var stream in streams)
                {
                    if (stream != null)
                    {
	                    stream.Dispose();
                    }
                }
            }
        }

	    [Obsolete]
	    public void UpdateMemory(int width, int height, Format format, byte[] data, int pitch)
        {
            if (data == null)
            {
	            return;
            }

	        using (var stream = new DataStream(data.Length, true, true))
            {
                stream.WriteRange(data);
                stream.Position = 0;
                var box = new DataBox(stream.DataPointer, pitch, 0);

                if (width != mTexture.Description.Width || height != mTexture.Description.Height ||
                    format != mTexture.Description.Format || mTexture.Description.MipLevels != 1 ||
                    mTexture == gDefaultTexture)
                {
                    CreateNew(width, height, format, new[] { box });
                }
                else
                {
                    //TODO: Nihlus plz fix da below monstrosity ty <3
                    //Updating the subresource results in random chunks disappearing however re-creating the View works
                    var desc = mTexture.Description;
                    mTexture.Dispose();
                    mTexture = new Texture2D(mContext.Device, desc, new[] { box });

                    var srvd = new ShaderResourceViewDescription
                    {
                        Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                        Format = format,
                        Texture2D = new ShaderResourceViewDescription.Texture2DResource { MipLevels = 1, MostDetailedMip = 0 }
                    };

                    NativeView.Dispose();
                    NativeView = new ShaderResourceView(mContext.Device, mTexture, srvd);
                }
            }
        }

	    [Obsolete]
	    public void UpdateMemory(int width, int height, Format format, uint[] data, int pitch)
        {
            if (data == null)
            {
	            return;
            }

	        using (var stream = new DataStream(data.Length * 4, true, true))
            {
                stream.WriteRange(data);
                stream.Position = 0;
                var box = new DataBox(stream.DataPointer, pitch, 0);

                if (IsDirty(width, height, format, 1))
                {
	                CreateNew(width, height, format, new[] { box });
                }
                else
                {
                    var region = new ResourceRegion
                    {
                        Back = 1,
                        Bottom = height,
                        Front = 0,
                        Left = 0,
                        Right = width,
                        Top = 0
                    };
                    mContext.Context.UpdateSubresource(mTexture, 0, region, box.DataPointer, width * 4, 0);
                }
            }
        }

	    [Obsolete]
        public void UpdateTexture(int width, int height, Format format, List<byte[]> layers, List<int> rowSizes)
        {
            var boxes = new DataBox[layers.Count];
            var streams = new DataStream[layers.Count];
            try
            {
                for (var i = 0; i < layers.Count; ++i)
                {
                    streams[i] = new DataStream(layers[i].Length, true, true);
                    streams[i].WriteRange(layers[i]);
                    streams[i].Position = 0;
                    boxes[i] = new DataBox(streams[i].DataPointer, rowSizes[i], 0);
                }

                if (IsDirty(width, height, format, layers.Count))
                {
	                CreateNew(width, height, format, boxes);
                }
                else
                {
                    for (var i = 0; i < layers.Count; ++i)
                    {
                        mContext.Context.UpdateSubresource(boxes[i], mTexture, i);
                    }
                }
            }
            finally
            {
                foreach (var strm in streams)
                {
                    if (strm != null)
                    {
	                    strm.Dispose();
                    }
                }
            }
        }

        private bool IsDirty(int width, int height, Format format, int layers)
        {
            return width != mTexture.Description.Width || height != mTexture.Description.Height ||
                   format != mTexture.Description.Format || mTexture.Description.MipLevels != layers ||
                   mTexture == gDefaultTexture;
        }

        private void CreateNew(int width, int height, Format format, DataBox[] boxes)
        {
            if (mTexture != gDefaultTexture)
            {
                mTexture.Dispose();
                NativeView.Dispose();
            }

            var desc = mTexture.Description;
            desc.Width = width;
            desc.Height = height;
            desc.Format = format;
            desc.Usage = ResourceUsage.Default;
            mTexture = new Texture2D(mContext.Device, desc, boxes);

            var srvd = new ShaderResourceViewDescription
            {
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                Format = format,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource { MipLevels = boxes.Length, MostDetailedMip = 0 }
            };

            NativeView = new ShaderResourceView(mContext.Device, mTexture, srvd);
        }

	    // TODO: Rewrite & rework
        public static void InitDefaultTexture()
        {
            var desc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = 2,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Immutable,
                Width = 2
            };

            using (var strm = new DataStream(16, true, true))
            {
                strm.WriteRange(new[] {0xFFFF0000, 0xFF00FF00, 0xFF0000FF, 0xFFFFFFFF});
                var layerData = new DataBox(strm.DataPointer) {RowPitch = 8};
                gDefaultTexture = new Texture2D(context.Device, desc, new[] { layerData });
            }

            var srvd = new ShaderResourceViewDescription
            {
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                Format = Format.R8G8B8A8_UNorm,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };

            gDefaultView = new ShaderResourceView(context.Device, gDefaultTexture, srvd);

            DefaultTextures.Initialize();
        }

	    private void Dispose(bool disposing)
	    {
		    if (this.glTextureID > 0)
		    {
			    GL.DeleteTexture(this.glTextureID);
		    }
	    }

	    public void Dispose()
	    {
		    Dispose(true);
		    GC.SuppressFinalize(this);
	    }
    }
}

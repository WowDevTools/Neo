using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace WoWEditor6.Graphics
{
    class Texture
    {
        private static Texture2D gDefaultTexture;
        private static ShaderResourceView gDefaultView;

        private Texture2D mTexture;
        private readonly GxContext mContext;

        public ShaderResourceView NativeView { get; private set; }

        public Texture(GxContext context)
        {
            mContext = context;
            NativeView = gDefaultView;
            mTexture = gDefaultTexture;
        }

        public void LoadFromLoadInfo(IO.Files.Texture.TextureLoadInfo loadInfo)
        {
            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = loadInfo.Format,
                Height = loadInfo.Height,
                Width = loadInfo.Width,
                MipLevels = loadInfo.Layers.Count,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            if (mTexture != gDefaultTexture)
            {
                mTexture?.Dispose();
                NativeView?.Dispose();
            }

            var boxes = new DataBox[texDesc.MipLevels];
            var streams = new DataStream[texDesc.MipLevels];
            try
            {
                for(var i = 0; i < texDesc.MipLevels; ++i)
                {
                    streams[i] = new DataStream(loadInfo.Layers[i].Length, true, true);
                    streams[i].WriteRange(loadInfo.Layers[i]);
                    streams[i].Position = 0;
                    boxes[i] = new DataBox(streams[i].DataPointer, loadInfo.RowPitchs[i], 0);
                }

                mTexture = new Texture2D(mContext.Device, texDesc, boxes);
                var srvd = new ShaderResourceViewDescription
                {
                    Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                    Format = loadInfo.Format,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource { MipLevels = boxes.Length, MostDetailedMip = 0 }
                };

                NativeView = new ShaderResourceView(mContext.Device, mTexture, srvd);
            }
            finally
            {
                foreach (var stream in streams) stream?.Dispose();
            }
        }

        public void UpdateMemory(int width, int height, Format format, byte[] data, int pitch)
        {
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

        public void UpdateMemory(int width, int height, Format format, uint[] data, int pitch)
        {
            using (var stream = new DataStream(data.Length * 4, true, true))
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

                if (width != mTexture.Description.Width || height != mTexture.Description.Height ||
                    format != mTexture.Description.Format || mTexture.Description.MipLevels != layers.Count ||
                    mTexture == gDefaultTexture)
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
                foreach (var strm in streams) strm?.Dispose();
            }
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

        public static void InitDefaultTexture(GxContext context)
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
        }
    }
}

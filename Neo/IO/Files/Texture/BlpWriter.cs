using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Neo.Scene;

namespace Neo.IO.Files.Texture
{
	public static class BlpWriter
    {
        public static unsafe void Write(Stream output, Bitmap image, Format format, bool hasMipMap = true)
        {
            if (hasMipMap && (IsPowerOfTwo(image.Width) == false || IsPowerOfTwo(image.Height) == false))
            {
	            throw new ArgumentException(
		            "Cannot save texture with Mipmaps when one of the dimensions is not a power of two");
            }

	        switch (format)
            {
                case Format.BC1_UNorm:
                case Format.BC2_UNorm:
                case Format.BC3_UNorm:
                    break;

                default:
                    throw new NotImplementedException(string.Format("Format not implemented: {0}", format));
            }

            var writer = new BinaryWriter(output);
            var header = WriteHeader(writer, image, format, hasMipMap);
            using (var tex = CreateTexture(image, hasMipMap))
            {
                var sizes = new int[16];
                var offsets = new int[16];
                if (hasMipMap == false)
                {
                    var layer = GetLayerData(image.Width, image.Height, 0, tex);
                    layer = CompressLayer(layer, image.Width, image.Height, format);
                    WriteLayer(writer, layer, 0, sizes, offsets);
                }
                else
                {
                    var l2H = (int)Math.Log(image.Height, 2);
                    var l2W = (int)Math.Log(image.Width, 2);
                    var totalMips = Math.Min(l2H, l2W) + 1;

                    for (var i = 0; i < totalMips; ++i)
                    {
                        var w = Math.Max(image.Width >> i, 1);
                        var h = Math.Max(image.Height >> i, 1);
                        var layer = GetLayerData(w, h, i, tex);
                        layer = CompressLayer(layer, w, h, format);
                        WriteLayer(writer, layer, i, sizes, offsets);
                    }
                }

                writer.BaseStream.Position = 0;
                for (var i = 0; i < 16; ++i)
                {
                    header.Sizes[i] = sizes[i];
                    header.Offsets[i] = offsets[i];
                }

                writer.Write(header);
            }
        }

        private static BlpHeader WriteHeader(BinaryWriter output, Image image, Format format, bool hasMips)
        {
            var header = new BlpHeader
            {
                Magic = 0x32504C42,
                Width = image.Width,
                Height = image.Height,
                Compression = 2,
                AlphaCompression = (byte)(format == Format.BC1_UNorm ? 0 : (format == Format.BC2_UNorm ? 1 : 7)),
                AlphaDepth = 0,
                MipLevels =  (byte)(hasMips ? 1 : 0),
                Version = 1
            };

            output.Write(header);
            return header;
        }

        private static unsafe byte[] GetLayerData(int width, int height, int layer, Texture2D texture)
        {
            var data = new byte[width * height * 4];
            var eventObj = new EventWaitHandle(false, EventResetMode.AutoReset);
            var ctx = WorldFrame.Instance.GraphicsContext;

            InvokeOrExecuteOnGpu(() =>
            {
                var box = ctx.Context.MapSubresource(texture, layer, MapMode.Read, MapFlags.None);
                fixed (byte* ptr = data)
                {
                    for(var i = 0; i < height; ++i)
                    {
	                    UnsafeNativeMethods.CopyMemory(ptr + i * width * 4, ((byte*) box.DataPointer.ToPointer()) + i * box.RowPitch, width * 4);
                    }
                }
                ctx.Context.UnmapSubresource(texture, layer);
                eventObj.Set();
            }, eventObj);

            return data;
        }

        private static unsafe Texture2D CreateTexture(Bitmap bmp, bool withMips)
        {
            Texture2D tex = null;
            var ctx = WorldFrame.Instance.GraphicsContext;
            var eventObj = new EventWaitHandle(false, EventResetMode.AutoReset);

            var colors = new byte[bmp.Width * bmp.Height * 4];
            var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            fixed (byte* ptr = colors)
            {
	            UnsafeNativeMethods.CopyMemory(ptr, (byte*) bmpd.Scan0.ToPointer(), colors.Length);
            }

	        var totalMips = 1;
            if (withMips)
            {
                var l2H = (int)Math.Log(bmp.Height, 2);
                var l2W = (int)Math.Log(bmp.Width, 2);
                totalMips = Math.Min(l2H, l2W) + 1;
            }

            InvokeOrExecuteOnGpu(() =>
            {
                var texDesc = new Texture2DDescription
                {
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = bmp.Width,
                    Height = bmp.Height,
                    MipLevels = totalMips,
                    OptionFlags = withMips ? ResourceOptionFlags.GenerateMipMaps : ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default
                };

                if(withMips)
                {
	                texDesc.BindFlags |= BindFlags.RenderTarget;
                }

	            var gputex = new Texture2D(ctx.Device, texDesc);
                ctx.Context.UpdateSubresource(colors, gputex, 0, 4 * bmp.Width);

                if (withMips)
                {
                    var srvd = new ShaderResourceView(ctx.Device, gputex, new ShaderResourceViewDescription
                    {
                        Format = Format.B8G8R8A8_UNorm,
                        Dimension = ShaderResourceViewDimension.Texture2D,
                        Texture2D = new ShaderResourceViewDescription.Texture2DResource
                        {
                            MipLevels = totalMips,
                            MostDetailedMip = 0
                        }
                    });

                    ctx.Context.GenerateMips(srvd);
                    srvd.Dispose();
                }

                texDesc.BindFlags = BindFlags.None;
                texDesc.CpuAccessFlags = CpuAccessFlags.Read;
                texDesc.OptionFlags = ResourceOptionFlags.None;
                texDesc.Usage = ResourceUsage.Staging;

                tex = new Texture2D(ctx.Device, texDesc);
                for(var i = 0; i < totalMips; ++i)
                {
	                ctx.Context.CopySubresourceRegion(gputex, i, null, tex, i);
                }

	            gputex.Dispose();

                eventObj.Set();
            }, eventObj);

            return tex;
        }

        private static unsafe byte[] CompressLayer(byte[] layerData, int width, int height, Format format)
        {
            uint dstFormat;
            int blockSize;
            switch (format)
            {
                case Format.BC2_UNorm:
                    dstFormat = Utils.LibTxc.GlCompressedRgbaS3TcDxt3Ext;
                    blockSize = 8;
                    break;

                case Format.BC3_UNorm:
                    dstFormat = Utils.LibTxc.GlCompressedRgbaS3TcDxt5Ext;
                    blockSize = 16;
                    break;

                default:
                    dstFormat = Utils.LibTxc.GlCompressedRgbS3TcDxt1Ext;
                    blockSize = 16;
                    break;
            }

            var totalSize = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
            var outData = new byte[totalSize];
            fixed (byte* output = outData)
            {
                fixed (byte* input = layerData)
                {
                    Utils.LibTxc.CompressDxtn(width, height, input, dstFormat, output, 0);
                }
            }

            return outData;
        }

        private static void WriteLayer(BinaryWriter output, byte[] data, int layerIndex, int[] sizes, int[] offsets)
        {
            sizes[layerIndex] = data.Length;
            offsets[layerIndex] = (int) output.BaseStream.Position;
            output.Write(data);
        }

        private static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

        private static void InvokeOrExecuteOnGpu(Action action, WaitHandle optionalWaitHandle)
        {
            if (WorldFrame.Instance.Dispatcher.InvokeRequired)
            {
                WorldFrame.Instance.Dispatcher.BeginInvoke(action);
                optionalWaitHandle.WaitOne();
            }
            else
            {
	            action();
            }
        }
    }
}

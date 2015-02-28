using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Editing;
using WoWEditor6.Scene;

namespace WoWEditor6.IO.Files.Terrain.WoD
{
    sealed class MapChunk : Terrain.MapChunk
    {
        private ChunkStreamInfo mMainInfo;
        private ChunkStreamInfo mTexInfo;
        private ChunkStreamInfo mObjInfo;

        private BinaryReader mReader;
        private BinaryReader mTexReader;
        private BinaryReader mObjReader;

        private Mcnk mHeader;

        private IntPtr mAlphaDataCompressed;

        private WeakReference<MapArea> mParent;
        private Vector4[] mShadingFloats = new Vector4[145];

        private Dictionary<uint, DataChunk> mOriginalMainChunks = new Dictionary<uint, DataChunk>();
        private Dictionary<uint, DataChunk> mOriginalObjChunks = new Dictionary<uint, DataChunk>(); 
        private Dictionary<uint, DataChunk> mOriginalTexChunks = new Dictionary<uint, DataChunk>(); 
        private static readonly uint[] Indices = new uint[768];

        public bool HasMccv { get; private set; }

        public MapChunk(ChunkStreamInfo mainInfo, ChunkStreamInfo texInfo, ChunkStreamInfo objInfo,  int indexX, int indexY, MapArea parent)
        {
            mIsYInverted = true;
            Parent = new WeakReference<Terrain.MapArea>(parent);
            mParent = new WeakReference<MapArea>(parent);
            mMainInfo = mainInfo;
            mTexInfo = texInfo;
            mObjInfo = objInfo;

            mReader = mainInfo.Stream;
            mTexReader = texInfo.Stream;
            mObjReader = objInfo.Stream;

            IndexX = indexX;
            IndexY = indexY;

            for (var i = 0; i < 145; ++i) mShadingFloats[i] = Vector4.One;
        }

        protected override void Dispose(bool disposing)
        {
            if (mReader != null)
            {
                mReader.Dispose();
                mReader = null;
            }

            if (mTexReader != null)
            {
                mTexReader.Dispose();
                mTexReader = null;
            }

            if (mObjReader != null)
            {
                mObjReader.Dispose();
                mObjReader = null;
            }

            if (mOriginalMainChunks != null)
            {
                mOriginalMainChunks.Clear();
                mOriginalMainChunks = null;
            }

            if (mOriginalObjChunks != null)
            {
                mOriginalObjChunks.Clear();
                mOriginalObjChunks = null;
            }

            if (mOriginalTexChunks != null)
            {
                mOriginalTexChunks.Clear();
                mOriginalTexChunks = null;
            }

            mMainInfo = null;
            mTexInfo = null;
            mObjInfo = null;
            mParent = null;
            mShadingFloats = null;

            base.Dispose(disposing);
        }

        public void WriteBaseChunks(BinaryWriter writer)
        {
            var minHeight = Vertices.Select(v => v.Position.Z).Min();
            mHeader.Position.Z = minHeight;
            var heights = Vertices.Select(v => v.Position.Z - minHeight).ToArray();
            var normals =
                Vertices.SelectMany(
                    v => new[] {(sbyte) (-v.Normal.X * 127.0f), (sbyte) (-v.Normal.Y * 127.0f), (sbyte) (v.Normal.Z * 127.0f)})
                    .ToArray();

            var colors = mShadingFloats.Select(v =>
            {
                uint b = (byte) Math.Max(Math.Min((v.Z / 2.0f) * 255.0f, 255), 0);
                uint g = (byte) Math.Max(Math.Min((v.Y / 2.0f) * 255.0f, 255), 0);
                uint r = (byte) Math.Max(Math.Min((v.X / 2.0f) * 255.0f, 255), 0);
                return 0x7F000000 | (b << 16) | (g << 8) | r;
            }).ToArray();

            AddOrReplaceChunk(0x4D435654, heights);
            AddOrReplaceChunk(0x4D434E52, normals);
            if (HasMccv)
            {
                mHeader.Flags |= 0x40;
                AddOrReplaceChunk(0x4D434356, colors);
            }

            var totalSize = mOriginalMainChunks.Sum(pair => pair.Value.Size + 8);
            totalSize += SizeCache<Mcnk>.Size;

            writer.Write(0x4D434E4B);
            writer.Write(totalSize);
            writer.Write(mHeader);

            var startPos = writer.BaseStream.Position;
            foreach(var chunk in mOriginalMainChunks)
            {
                writer.Write(chunk.Key);
                writer.Write(chunk.Value.Size);
                writer.Write(chunk.Value.Data);
            }

            var endPos = writer.BaseStream.Position;
            writer.BaseStream.Position = startPos - SizeCache<Mcnk>.Size;
            writer.Write(mHeader);
            writer.BaseStream.Position = endPos;
        }

        public void WriteObjChunks(BinaryWriter writer)
        {
            var totalSize = mOriginalObjChunks.Sum(pair => pair.Value.Size + 8);
            writer.Write(0x4D434E4B);
            writer.Write(totalSize);

            foreach (var chunk in mOriginalObjChunks)
            {
                writer.Write(chunk.Key);
                writer.Write(chunk.Value.Size);
                writer.Write(chunk.Value.Data);
            }
        }

        public void WriteTexChunks(BinaryWriter writer)
        {
            var alpha = SaveAlpha();
            AddOrUpdateChunk(mOriginalTexChunks, 0x4D43414C, alpha.ToArray());
            AddOrUpdateChunk(mOriginalTexChunks, 0x4D434C59, mLayers);

            var totalSize = mOriginalTexChunks.Sum(pair => pair.Value.Size + 8);
            writer.Write(0x4D434E4B);
            writer.Write(totalSize);

            foreach (var chunk in mOriginalTexChunks)
            {
                writer.Write(chunk.Key);
                writer.Write(chunk.Value.Size);
                writer.Write(chunk.Value.Data);
            }
        }

        public override bool OnTerrainChange(TerrainChangeParameters parameters)
        {
            var changed = base.OnTerrainChange(parameters);

            if(changed)
            {
                MapArea parent;
                mParent.TryGetTarget(out parent);

                var omin = BoundingBox.Minimum;
                var omax = BoundingBox.Maximum;
                BoundingBox = new BoundingBox(new Vector3(omin.X, omin.Y, mMinHeight),
                    new Vector3(omax.X, omax.Y, mMaxHeight));

                if (parent != null)
                    parent.UpdateBoundingBox(BoundingBox);
            }

            return changed;
        }

        public override void UpdateNormals()
        {
            base.UpdateNormals();

            MapArea parent;
            mParent.TryGetTarget(out parent);
            if (parent != null)
                parent.UpdateVertices(this);
        }

        public bool Intersect(ref Ray ray, out float distance)
        {
            distance = float.MaxValue;
            if (BoundingBox.Intersects(ref ray) == false)
                return false;

            var minDist = float.MaxValue;
            var hasHit = false;
            var dir = ray.Direction;
            var orig = ray.Position;

            Vector3 e1, e2, p, T, q;

            for (var i = 0; i < Indices.Length; i += 3)
            {
                var i0 = Indices[i];
                var i1 = Indices[i + 1];
                var i2 = Indices[i + 2];
                Vector3.Subtract(ref Vertices[i1].Position, ref Vertices[i0].Position, out e1);
                Vector3.Subtract(ref Vertices[i2].Position, ref Vertices[i0].Position, out e2);

                Vector3.Cross(ref dir, ref e2, out p);
                float det;
                Vector3.Dot(ref e1, ref p, out det);

                if (Math.Abs(det) < 1e-4)
                    continue;

                var invDet = 1.0f / det;
                Vector3.Subtract(ref orig, ref Vertices[i0].Position, out T);
                float u;
                Vector3.Dot(ref T, ref p, out u);
                u *= invDet;

                if (u < 0 || u > 1)
                    continue;

                Vector3.Cross(ref T, ref e1, out q);
                float v;
                Vector3.Dot(ref dir, ref q, out v);
                v *= invDet;
                if (v < 0 || (u + v) > 1)
                    continue;

                float t;
                Vector3.Dot(ref e2, ref q, out t);
                t *= invDet;

                if (t < 1e-4) continue;

                hasHit = true;
                if (t < minDist)
                    minDist = t;
            }

            if(hasHit)
                distance = minDist;

            return hasHit;
        }

        public void AsyncLoad()
        {
            mReader.BaseStream.Position = mMainInfo.PosStart;
            var chunkSize = mReader.ReadInt32();
            mHeader = mReader.Read<Mcnk>();
            var hasMccv = false;

            for (var i = 0; i < 4096; ++i)
                AlphaValues[i] = 0xFF;

            while (mReader.BaseStream.Position + 8 <= mMainInfo.PosStart + 8 + chunkSize)
            {
                var id = mReader.ReadUInt32();
                var size = mReader.ReadInt32();

                if (mReader.BaseStream.Position + size > mMainInfo.PosStart + 8 + chunkSize)
                    break;

                var cur = mReader.BaseStream.Position;

                switch(id)
                {
                    case 0x4D435654:
                        LoadMcvt();
                        break;

                    case 0x4D434E52:
                        LoadMcnr();
                        break;

                    case 0x4D434356:
                        hasMccv = true;
                        LoadMccv();
                        break;

                    case 0x4D434C56:
                        LoadMclv();
                        break;
                }

                mReader.BaseStream.Position = cur;
                var data = mReader.ReadBytes(size);
                mOriginalMainChunks.Add(id, new DataChunk {Data = data, Signature = id, Size = size});
            }

            LoadHoles();

            if (hasMccv == false)
            {
                for (var i = 0; i < 145; ++i)
                    Vertices[i].Color = 0x7F7F7F7F;
            }

            LoadTexData();
            LoadObjData();

            WorldFrame.Instance.MapManager.OnLoadProgress();
        }

        private void LoadObjData()
        {
            mObjReader.BaseStream.Position = mObjInfo.PosStart;
            var chunkSize = mObjReader.ReadInt32();

            while(mObjReader.BaseStream.Position + 8 <= mObjInfo.PosStart + 8 + chunkSize)
            {
                var id = mObjReader.ReadUInt32();
                var size = mObjReader.ReadInt32();

                if (mObjReader.BaseStream.Position + size > mObjInfo.PosStart + 8 + chunkSize)
                    break;

                var cur = mObjReader.BaseStream.Position;
                switch(id)
                {
                    case 0x4D435244:
                        LoadMcrd(size);
                        break;
                }

                mObjReader.BaseStream.Position = cur;
                var data = mObjReader.ReadBytes(size);
                mOriginalObjChunks.Add(id, new DataChunk { Data = data, Signature = id, Size = size });
                mObjReader.BaseStream.Position = cur + size;
            }
        }

        private void LoadMcrd(int size)
        {
            DoodadReferences = mObjReader.ReadArray<int>(size / 4);
            var minPos = BoundingBox.Minimum;
            var maxPos = BoundingBox.Maximum;

            MapArea parent;
            if (mParent.TryGetTarget(out parent) == false)
                return;

            foreach (var reference in DoodadReferences)
            {
                var inst = parent.DoodadInstances[reference];
                var min = inst.BoundingBox.Minimum;
                var max = inst.BoundingBox.Maximum;

                if (min.X < minPos.X)
                    minPos.X = min.X;
                if (min.Y < minPos.Y)
                    minPos.Y = min.Y;
                if (min.Z < minPos.Z)
                    minPos.Z = min.Z;
                if (max.X > maxPos.X)
                    maxPos.X = max.X;
                if (max.Y > maxPos.Y)
                    maxPos.Y = max.Y;
                if (max.Z > maxPos.Z)
                    maxPos.Z = max.Z;
            }

            ModelBox = new BoundingBox(minPos, maxPos);
        }

        private void LoadTexData()
        {
            try
            {
                mTexReader.BaseStream.Position = mTexInfo.PosStart;
                var chunkSize = mTexReader.ReadInt32();

                while (mTexReader.BaseStream.Position + 8 <= mTexInfo.PosStart + 8 + chunkSize)
                {
                    var id = mTexReader.ReadUInt32();
                    var size = mTexReader.ReadInt32();

                    if (mTexReader.BaseStream.Position + size > mTexInfo.PosStart + 8 + chunkSize)
                        break;

                    var cur = mTexReader.BaseStream.Position;

                    switch (id)
                    {
                        case 0x4D434C59:
                            LoadMcly(size);
                            break;

                        case 0x4D43414C:
                            mAlphaDataCompressed = Marshal.AllocHGlobal(size);
                            mTexReader.ReadToPointer(mAlphaDataCompressed, size);
                            break;

                        case 0x4D435348:
                            {
                                if (size >= 512)
                                {
                                    var curPtr = 0;
                                    for (var i = 0; i < 64; ++i)
                                    {
                                        for (var j = 0; j < 8; ++j)
                                        {
                                            var mask = mTexReader.ReadByte();
                                            for (var k = 0; k < 8; ++k)
                                            {
                                                AlphaValues[curPtr] &= 0xFFFFFF00;
                                                AlphaValues[curPtr++] |= ((mask & (1 << k)) == 0) ? (byte)0xFF : (byte)0xCC;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    mTexReader.BaseStream.Position = cur;
                    var data = mTexReader.ReadBytes(size);
                    mOriginalTexChunks.Add(id, new DataChunk { Data = data, Signature = id, Size = size });
                    mTexReader.BaseStream.Position = cur + size;
                }

                LoadAlpha();

                var textures = new List<Graphics.Texture>();
                MapArea parent;
                mParent.TryGetTarget(out parent);
                if (parent == null)
                    throw new InvalidOperationException("Parent got disposed but loading was still invoked");

                TextureScales = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
                TextureNames = new string[mLayers.Length];
                for (var i = 0; i < mLayers.Length && i < 4; ++i)
                {
                    var texName = parent.GetTextureName(mLayers[i].TextureId);
                    TextureNames[i] = texName;
                    textures.Add(parent.GetTexture(mLayers[i].TextureId));
                    TextureScales[i] = parent.GetTextureScale(mLayers[i].TextureId);
                }

                Textures = textures;
            }
            finally
            {
                if (mAlphaDataCompressed != IntPtr.Zero)
                    Marshal.FreeHGlobal(mAlphaDataCompressed);
            }
        }

        private void LoadAlpha()
        {
            var nLayers = Math.Min(mLayers.Length, 4);
            for(var i = 1; i < nLayers; ++i)
            {
                if ((mLayers[i].Flags & 0x200) != 0)
                    LoadLayerRle(mLayers[i], i);
                else if ((mLayers[i].Flags & 0x100) != 0)
                {
                    if (WorldFrame.Instance.MapManager.HasNewBlend)
                        LoadUncompressed(mLayers[i], i);
                    else
                        LoadLayerCompressed(mLayers[i], i);
                }
                else
                {
                    for (var j = 0; j < 4096; ++j)
                        AlphaValues[j] |= 0xFFu << (8 * i);
                }
            }

            //mAlphaDataCompressed = null;
        }

        private unsafe void LoadUncompressed(Mcly layerInfo, int layer)
        {
            var ptr = mAlphaDataCompressed.ToPointer();
            var startPos = layerInfo.OfsMcal;
            for (var i = 0; i < 4096; ++i)
                AlphaValues[i] |= (uint) ((byte*)ptr)[startPos++] << (8 * layer);
        }

        private unsafe void LoadLayerCompressed(Mcly layerInfo, int layer)
        {
            var ptr = mAlphaDataCompressed.ToPointer();
            var startPos = layerInfo.OfsMcal;
            var counter = 0;
            for (var k = 0; k < 64; ++k)
            {
                for (var j = 0; j < 32; ++j)
                {
                    var alpha = ((byte*)ptr)[startPos++];
                    var val1 = alpha & 0xF;
                    var val2 = alpha >> 4;
                    val2 = j == 31 ? val1 : val2;
                    val1 = (byte)((val1 / 15.0f) * 255.0f);
                    val2 = (byte)((val2 / 15.0f) * 255.0f);
                    AlphaValues[counter++] |= (uint)val1 << (8 * layer);
                    AlphaValues[counter++] |= (uint)val2 << (8 * layer);
                }
            }
        }

        private unsafe void LoadLayerRle(Mcly layerInfo, int layer)
        {
            var ptr = mAlphaDataCompressed.ToPointer();
            var counterOut = 0;
            var startPos = layerInfo.OfsMcal;
            while (counterOut < 4096)
            {
                var indicator = ((byte*)ptr)[startPos++];
                if ((indicator & 0x80) != 0)
                {
                    var value = ((byte*)ptr)[startPos++];
                    var repeat = indicator & 0x7F;
                    for (var k = 0; k < repeat && counterOut < 4096; ++k)
                        AlphaValues[counterOut++] |= (uint)value << (layer * 8);
                }
                else
                {
                    for (var k = 0; k < (indicator & 0x7F) && counterOut < 4096; ++k)
                        AlphaValues[counterOut++] |= (uint)((byte*)ptr)[startPos++] << (8 * layer);
                }
            }
        }

        private void LoadMcvt()
        {
            var heights = mReader.ReadArray<float>(145);

            var posx = Metrics.MapMidPoint - mHeader.Position.Y;
            var posy = Metrics.MapMidPoint + mHeader.Position.X;
            var posz = mHeader.Position.Z;

            var counter = 0;

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for(var i = 0; i < 17; ++i)
            {
                for(var j = 0; j < (((i % 2) != 0) ? 8 : 9); ++j)
                {
                    var height = posz + heights[counter];
                    var x = posx + j * Metrics.UnitSize;
                    if ((i % 2) != 0)
                        x += 0.5f * Metrics.UnitSize;
                    var y = posy - i * Metrics.UnitSize * 0.5f;

                    Vertices[counter].Position = new Vector3(x, y, height);

                    if (height < minPos.Z)
                        minPos.Z = height;
                    if (height > maxPos.Z)
                        maxPos.Z = height;

                    if (x < minPos.X)
                        minPos.X = x;
                    if (x > maxPos.X)
                        maxPos.X = x;
                    if (y < minPos.Y)
                        minPos.Y = y;
                    if (y > maxPos.Y)
                        maxPos.Y = y;

                    Vertices[counter].TexCoordAlpha = new Vector2(j / 8.0f + ((i % 2) != 0 ? (0.5f / 8.0f) : 0), i / 16.0f);
                    Vertices[counter].TexCoord = new Vector2(j + ((i % 2) != 0 ? 0.5f : 0.0f), i * 0.5f);
                    ++counter;
                }
            }

            mMinHeight = minPos.Z;
            mMaxHeight = maxPos.Z;

            BoundingBox = new BoundingBox(minPos, maxPos);
            mMidPoint = minPos + (maxPos - minPos) / 2.0f;
        }

        private void LoadMcnr()
        {
            var normals = mReader.ReadArray<sbyte>(145 * 3);
            var counter = 0;

            for (var i = 0; i < 17; ++i)
            {
                for (var j = 0; j < (((i % 2) != 0) ? 8 : 9); ++j)
                {
                    var nx = normals[counter * 3] / -127.0f;
                    var ny = normals[counter * 3 + 1] / -127.0f;
                    var nz = normals[counter * 3 + 2] / 127.0f;

                    Vertices[counter].Normal = new Vector3(nx, ny, nz);
                    ++counter;
                }
            }
        }

        private void LoadMccv()
        {
            var colors = mReader.ReadArray<uint>(145);
            for (var i = 0; i < 145; ++i)
            {
                Vertices[i].Color = colors[i];
                var r = (colors[i] >> 16) & 0xFF;
                var g = (colors[i] >> 8) & 0xFF;
                var b = (colors[i]) & 0xFF;
                var a = (colors[i] >> 24) & 0xFF;

                mShadingFloats[i] = new Vector4(b * 2.0f / 255.0f, g * 2.0f / 255.0f, r * 2.0f / 255.0f, a * 2.0f / 255.0f);
            }
        }

        private void LoadMclv()
        {
            var colors = mReader.ReadArray<uint>(145);
            for(var i = 0; i < 145; ++i)
            {
                Vertices[i].AdditiveColor = colors[i];
            }
        }

        private void LoadHoles()
        {
            if((mHeader.Flags & 0x10000) == 0)
            {
                for (var i = 0; i < 4; ++i)
                {
                    for (var j = 0; j < 4; ++j)
                    {
                        var baseIndex = i * 2 * 8 + j * 2;
                        var mask = (mHeader.Holes & (1 << (i * 4 + j))) != 0;
                        HoleValues[baseIndex] = HoleValues[baseIndex + 1] =
                                HoleValues[baseIndex + 8] = HoleValues[baseIndex + 9] = (byte)(mask ? 0x00 : 0xFF);
                    }
                }
            }
            else
            {
                var holeBytes = new byte[8];
                Buffer.BlockCopy(BitConverter.GetBytes(mHeader.Mcvt), 0, holeBytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(mHeader.Mcnr), 0, holeBytes, 4, 4);

                for(var i = 0; i < 8; ++i)
                {
                    for(var j = 0; j < 8; ++j)
                    {
                        HoleValues[i * 8 + j] = (byte) (((holeBytes[i] >> j) & 1) != 0 ? 0x00 : 0xFF);
                    }
                }
            }
        }

        private void LoadMcly(int size)
        {
            mLayers = mTexReader.ReadArray<Mcly>(size / SizeCache<Mcly>.Size);
        }

        private unsafe DataChunk ChunkFromArray<T>(uint signature, T[] data) where T : struct
        {
            var byteData = new byte[data.Length * SizeCache<T>.Size];
            fixed(byte* ptr = byteData)
                UnsafeNativeMethods.CopyMemory(ptr, (byte*) SizeCache<T>.GetUnsafePtr(ref data[0]), byteData.Length);

            return new DataChunk
            {
                Data = byteData,
                Signature = signature,
                Size = byteData.Length
            };
        }

        private void AddOrReplaceChunk<T>(uint signature, T[] data) where T : struct
        {
            var chunk = ChunkFromArray(signature, data);
            if (mOriginalMainChunks.ContainsKey(signature) == false)
                mOriginalMainChunks.Add(signature, chunk);
            else
            {
                var old = mOriginalMainChunks[signature];
                if (old.Size >= chunk.Size)
                    Buffer.BlockCopy(chunk.Data, 0, old.Data, 0, chunk.Size);
                else
                    old = chunk;

                mOriginalMainChunks[signature] = old;
            }
        }

        protected override bool HandleMccvPaint(TerrainChangeParameters parameters)
        {
            var amount = (parameters.Amount / 75.0f) * (float)parameters.TimeDiff.TotalSeconds;
            var changed = false;

            var destColor = parameters.Shading;
            if(parameters.Inverted)
            {
                destColor.X = 2 - destColor.X;
                destColor.Y = 2 - destColor.Y;
                destColor.Z = 2 - destColor.Z;
            }

            var radius = parameters.OuterRadius;
            for(var i = 0; i < 145; ++i)
            {
                var p = Vertices[i].Position;
                var dist = (new Vector2(p.X, p.Y) - new Vector2(parameters.Center.X, parameters.Center.Y)).Length();
                if (dist > radius)
                    continue;

                HasMccv = true;
                changed = true;
                var factor = dist / radius;
                if (dist < parameters.InnerRadius)
                    factor = 1.0f;

                var curColor = mShadingFloats[i];
                var dr = destColor.X - curColor.Z;
                var dg = destColor.Y - curColor.Y;
                var db = destColor.Z - curColor.X;

                var cr = Math.Min(Math.Abs(dr), amount * factor);
                var cg = Math.Min(Math.Abs(dg), amount * factor);
                var cb = Math.Min(Math.Abs(db), amount * factor);

                if (dr < 0)
                {
                    curColor.Z -= cr;
                    if (curColor.Z < destColor.X)
                        curColor.Z = destColor.X;
                }
                else
                {
                    curColor.Z += cr;
                    if (curColor.Z > destColor.X)
                        curColor.Z = destColor.X;
                }
                if (dg < 0)
                {
                    curColor.Y -= cg;
                    if (curColor.Y < destColor.Y)
                        curColor.Y = destColor.Y;
                }
                else
                {
                    curColor.Y += cg;
                    if (curColor.Y > destColor.Y)
                        curColor.Y = destColor.Y;
                }
                if (db < 0)
                {
                    curColor.X -= cb;
                    if (curColor.X < destColor.Z)
                        curColor.X = destColor.Z;
                }
                else
                {
                    curColor.X += cb;
                    if (curColor.X > destColor.Z)
                        curColor.X = destColor.Z;
                }

                mShadingFloats[i] = curColor;

                curColor.X = Math.Min(Math.Max(curColor.X, 0), 2);
                curColor.Y = Math.Min(Math.Max(curColor.Y, 0), 2);
                curColor.Z = Math.Min(Math.Max(curColor.Z, 0), 2);

                var r = (byte) ((curColor.Z / 2.0f) * 255.0f);
                var g = (byte) ((curColor.Y / 2.0f) * 255.0f);
                var b = (byte) ((curColor.X / 2.0f) * 255.0f);
                var a = (byte) ((curColor.W / 2.0f) * 255.0f);

                var color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
                Vertices[i].Color = color;
            }

            return changed;
        }

        protected override int AddTextureLayer(string textureName)
        {
            var old = TextureNames;
            TextureNames = new string[TextureNames.Count + 1];
            for (var i = 0; i < old.Count; ++i)
                TextureNames[i] = old[i];

            TextureNames[TextureNames.Count - 1] = textureName;

            MapArea parent;
            if (mParent.TryGetTarget(out parent) == false)
                throw new InvalidOperationException("Couldnt get parent of map chunk");

            var texId = parent.GetOrAddTexture(textureName);
            var layer = new Mcly
            {
                Flags = 0,
                TextureId = texId,
                EffectId = -1,
                OfsMcal = 0,
                Padding = 0
            };

            var layers = mLayers;
            mLayers = new Mcly[layers.Length + 1];
            for (var i = 0; i < layers.Length; ++i)
                mLayers[i] = layers[i];

            mLayers[layers.Length] = layer;

            Textures.Add(parent.GetTexture(texId));
            TexturesChanged = true;
            return mLayers.Length - 1;
        }

        private unsafe void AddOrUpdateChunk<T>(Dictionary<uint, DataChunk> chunks, uint signature, T[] data) where T : struct
        {
            var chunk = chunks.ContainsKey(signature) ? chunks[signature] : new DataChunk {Signature = signature};
            var totalSize = SizeCache<T>.Size * data.Length;
            chunk.Size = totalSize;
            chunk.Data = new byte[totalSize];
            if (chunk.Data.Length > 0)
            {
                var ptr = SizeCache<T>.GetUnsafePtr(ref data[0]);
                fixed (byte* bptr = chunk.Data)
                    UnsafeNativeMethods.CopyMemory(bptr, (byte*) ptr, totalSize);
            }

            if (chunks.ContainsKey(signature))
                chunks[signature] = chunk;
            else
                chunks.Add(signature, chunk);
        }

        private MemoryStream SaveAlpha()
        {
            if (mLayers.Length == 0)
                return new MemoryStream();

            var strm = new MemoryStream();
            var writer = new BinaryWriter(strm);

            var curPos = 0;
            mLayers[0].Flags &= ~0x300u;
            mLayers[0].OfsMcal = 0;
            for (var i = 1; i < mLayers.Length; ++i)
            {
                bool compressed;
                var data = GetSavedAlphaForLayer(i, out compressed);
                mLayers[i].OfsMcal = curPos;
                if (compressed)
                    mLayers[i].Flags |= 0x300;
                else
                {
                    mLayers[i].Flags |= 0x100;
                    mLayers[i].Flags &= ~0x200u;
                }

                writer.Write(data);
                curPos += data.Length;
            }

            return strm;
        }

        private float CalculateAlphaHomogenity(int layer)
        {
            var numCompressable = 1;
            var lastAlpha = (AlphaValues[0] >> (layer * 8)) & 0xFF;
            for (var i = 1; i < 4096; ++i)
            {
                var value = (AlphaValues[i] >> (layer * 8)) & 0xFF;
                if (value == lastAlpha)
                    ++numCompressable;

                lastAlpha = value;
            }

            return numCompressable / 4096.0f;
        }

        private byte[] GetSavedAlphaForLayer(int layer, out bool compressed)
        {
            compressed = false;
            var homogenity = CalculateAlphaHomogenity(layer);
            if (homogenity < float.MaxValue)
            {
                compressed = true;
                return GetAlphaCompressed(layer);
            }

            return GetAlphaUncompressed(layer);
        }

        static MapChunk()
        {
            var indices = Indices;
            for (uint y = 0; y < 8; ++y)
            {
                for (uint x = 0; x < 8; ++x)
                {
                    var i = y * 8 * 12 + x * 12;
                    indices[i + 0] = y * 17 + x;
                    indices[i + 2] = y * 17 + x + 1;
                    indices[i + 1] = y * 17 + x + 9;

                    indices[i + 3] = y * 17 + x + 1;
                    indices[i + 5] = y * 17 + x + 18;
                    indices[i + 4] = y * 17 + x + 9;

                    indices[i + 6] = y * 17 + x + 18;
                    indices[i + 8] = y * 17 + x + 17;
                    indices[i + 7] = y * 17 + x + 9;

                    indices[i + 9] = y * 17 + x + 17;
                    indices[i + 11] = y * 17 + x;
                    indices[i + 10] = y * 17 + x + 9;
                }
            }
        }
    }
}

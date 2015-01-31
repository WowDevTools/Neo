using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Scene;

namespace WoWEditor6.IO.Files.Terrain.WoD
{
    class MapChunk : Terrain.MapChunk
    {
        private readonly ChunkStreamInfo mMainInfo;
        private readonly ChunkStreamInfo mTexInfo;
        private readonly ChunkStreamInfo mObjInfo;

        private readonly BinaryReader mReader;
        private readonly BinaryReader mTexReader;
        private readonly BinaryReader mObjReader;

        private static readonly uint[] Indices = new uint[768];

        private Mcnk mHeader;

        private IntPtr mAlphaDataCompressed;

        private readonly WeakReference<MapArea> mParent;

        private readonly List<Mcly> mLayerInfos = new List<Mcly>();

        private Vector3 mMidPoint;
        private float mMinHeight = float.MaxValue;
        private float mMaxHeight = float.MinValue;
        private bool mUpdateNormals;
        private readonly Vector4[] mShadingFloats = new Vector4[145];

        public MapChunk(ChunkStreamInfo mainInfo, ChunkStreamInfo texInfo, ChunkStreamInfo objInfo,  int indexX, int indexY, MapArea parent)
        {
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

        public bool OnTerrainChange(Editing.TerrainChangeParameters parameters)
        {
            var diffVec = new Vector2(mMidPoint.X, mMidPoint.Y) - new Vector2(parameters.Center.X, parameters.Center.Y);
            var dsquare = Vector2.Dot(diffVec, diffVec);

            var maxRadius = parameters.OuterRadius + Metrics.ChunkRadius;

            if (dsquare > maxRadius * maxRadius)
                return false;

            // always update the normals if we are closer than ChunkRadius to the modified area
            // since nearby changes might affect the normals of this chunk even if the positions
            // them self didn't change
            mUpdateNormals = true;

            var changed = false;
            switch(parameters.Method)
            {
                case Editing.TerrainChangeType.Elevate:
                    changed = HandleElevateTerrain(parameters);
                    break;

                case Editing.TerrainChangeType.Flatten:
                    changed = HandleFlatten(parameters);
                    break;

                case Editing.TerrainChangeType.Shading:
                    changed = HandleMccvPaint(parameters);
                    break;
            }

            if(changed)
            {
                MapArea parent;
                mParent.TryGetTarget(out parent);

                var omin = BoundingBox.Minimum;
                var omax = BoundingBox.Maximum;
                BoundingBox = new BoundingBox(new Vector3(omin.X, omin.Y, mMinHeight),
                    new Vector3(omax.X, omax.Y, mMaxHeight));

                parent?.UpdateBoundingBox(BoundingBox);
            }

            return changed;
        }

        public void UpdateNormals()
        {
            if (mUpdateNormals == false)
                return;

            mUpdateNormals = false;
            for(var i = 0; i < 145; ++i)
            {
                var p1 = Vertices[i].Position;
                var p2 = p1;
                var p3 = p2;
                var p4 = p3;
                var v = p1;

                p1.X -= 0.5f * Metrics.UnitSize;
                p1.Y -= 0.5f * Metrics.UnitSize;
                p2.X += 0.5f * Metrics.UnitSize;
                p2.Y -= 0.5f * Metrics.UnitSize;
                p3.X += 0.5f * Metrics.UnitSize;
                p3.Y += 0.5f * Metrics.UnitSize;
                p4.X -= 0.5f * Metrics.UnitSize;
                p4.Y += 0.5f * Metrics.UnitSize;

                var mgr = WorldFrame.Instance.MapManager;
                float h;
                if (mgr.GetLandHeight(p1.X, 64.0f * Metrics.TileSize - p1.Y, out h)) p1.Z = h;
                if (mgr.GetLandHeight(p2.X, 64.0f * Metrics.TileSize - p2.Y, out h)) p2.Z = h;
                if (mgr.GetLandHeight(p3.X, 64.0f * Metrics.TileSize - p3.Y, out h)) p3.Z = h;
                if (mgr.GetLandHeight(p4.X, 64.0f * Metrics.TileSize - p4.Y, out h)) p4.Z = h;

                var n1 = Vector3.Cross((p2 - v), (p1 - v));
                var n2 = Vector3.Cross((p3 - v), (p2 - v));
                var n3 = Vector3.Cross((p4 - v), (p3 - v));
                var n4 = Vector3.Cross((p1 - v), (p4 - v));

                var n = n1 + n2 + n3 + n4;
                n.Normalize();
                n *= -1;

                Vertices[i].Normal = n;
            }

            MapArea parent;
            mParent.TryGetTarget(out parent);
            parent?.UpdateVertices(this);
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

            Vector3 e1, e2, P, T, Q;

            for (var i = 0; i < Indices.Length; i += 3)
            {
                var i0 = Indices[i];
                var i1 = Indices[i + 1];
                var i2 = Indices[i + 2];
                Vector3.Subtract(ref Vertices[i1].Position, ref Vertices[i0].Position, out e1);
                Vector3.Subtract(ref Vertices[i2].Position, ref Vertices[i0].Position, out e2);

                Vector3.Cross(ref dir, ref e2, out P);
                float det;
                Vector3.Dot(ref e1, ref P, out det);

                if (Math.Abs(det) < 1e-4)
                    continue;

                var invDet = 1.0f / det;
                Vector3.Subtract(ref orig, ref Vertices[i0].Position, out T);
                float u;
                Vector3.Dot(ref T, ref P, out u);
                u *= invDet;

                if (u < 0 || u > 1)
                    continue;

                Vector3.Cross(ref T, ref e1, out Q);
                float v;
                Vector3.Dot(ref dir, ref Q, out v);
                v *= invDet;
                if (v < 0 || (u + v) > 1)
                    continue;

                float t;
                Vector3.Dot(ref e2, ref Q, out t);
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

        public override void AsyncLoad()
        {
            mReader.BaseStream.Position = mMainInfo.PosStart;
            var chunkSize = mReader.ReadInt32();
            mHeader = mReader.Read<Mcnk>();
            var hasMccv = false;

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
                }

                mReader.BaseStream.Position = cur + size;
            }

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
                    }

                    mTexReader.BaseStream.Position = cur + size;
                }

                LoadAlpha();

                var textures = new List<Graphics.Texture>();
                MapArea parent;
                mParent.TryGetTarget(out parent);
                if (parent == null)
                    throw new InvalidOperationException("Parent got disposed but loading was still invoked");

                TextureScales = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
                for (var i = 0; i < mLayerInfos.Count && i < 4; ++i)
                {
                    textures.Add(parent.GetTexture(mLayerInfos[i].TextureId));
                    TextureScales[i] = parent.GetTextureScale(mLayerInfos[i].TextureId);
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
            var nLayers = Math.Min(mLayerInfos.Count, 4);
            for(var i = 0; i < nLayers; ++i)
            {
                if ((mLayerInfos[i].Flags & 0x200) != 0)
                    LoadLayerRle(mLayerInfos[i], i);
                else if ((mLayerInfos[i].Flags & 0x100) != 0)
                {
                    if (WorldFrame.Instance.MapManager.HasNewBlend)
                        LoadUncompressed(mLayerInfos[i], i);
                    else
                        LoadLayerCompressed(mLayerInfos[i], i);
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

        private void LoadMcly(int size)
        {
            mLayerInfos.AddRange(mTexReader.ReadArray<Mcly>(size / SizeCache<Mcly>.Size));
        }

        private bool HandleFlatten(Editing.TerrainChangeParameters parameters)
        {
            var radius = parameters.OuterRadius;
            var amount = parameters.Amount / 550.0f;
            if (amount > 1) amount = 1;

            amount = 1 - amount;
            var changed = false;

            for (var i = 0; i < 145; ++i)
            {
                var p = Vertices[i].Position;
                var dist = (p - parameters.Center).Length();
                if (dist > radius)
                    continue;

                changed = true;
                var factor = dist / radius;

                switch (parameters.Algorithm)
                {
                    case Editing.TerrainAlgorithm.Flat:
                        p.Z = amount * p.Z + (1 - amount) * parameters.Center.Z;
                        break;

                    case Editing.TerrainAlgorithm.Linear:
                        {
                            var nremain = 1 - (1 - amount) * (1 - factor);
                            p.Z = nremain * p.Z + (1 - nremain) * parameters.Center.Z;
                            break;
                        }

                    case Editing.TerrainAlgorithm.Quadratic:
                        {
                            var nremain = 1 - (float)Math.Pow(1 - amount, 1 + factor);
                            p.Z = nremain * p.Z + (1 - nremain) * parameters.Center.Z;
                            break;
                        }

                    case Editing.TerrainAlgorithm.Trigonometric:
                        {
                            var nremain = 1 - (1 - amount) * (1 - (float)Math.Cos(factor * Math.PI / 2.0f));
                            p.Z = nremain * p.Z + (1 - nremain) * parameters.Center.Z;
                            break;
                        }
                }

                if (p.Z < mMinHeight)
                    mMinHeight = p.Z;
                if (p.Z > mMaxHeight)
                    mMaxHeight = p.Z;

                Vertices[i].Position = p;
            }

            return changed;
        }

        private bool HandleMccvPaint(Editing.TerrainChangeParameters parameters)
        {
            var amount = (parameters.Amount / 75.0f) * (float)parameters.TimeDiff.TotalSeconds;
            var changed = false;

            var radius = parameters.OuterRadius;
            for(var i = 0; i < 145; ++i)
            {
                var p = Vertices[i].Position;
                var dist = (p - parameters.Center).Length();
                if (dist > radius)
                    continue;

                changed = true;
                var factor = dist / radius;
                if (dist < parameters.InnerRadius)
                    factor = 1.0f;

                var curColor = mShadingFloats[i];
                var dr = parameters.Shading.X - curColor.X;
                var dg = parameters.Shading.Y - curColor.Y;
                var db = parameters.Shading.Z - curColor.Z;

                var cr = Math.Min(Math.Abs(dr), amount * factor);
                var cg = Math.Min(Math.Abs(dg), amount * factor);
                var cb = Math.Min(Math.Abs(db), amount * factor);

                if (dr < 0) curColor.X -= cr;
                else curColor.X += cr;
                if (dg < 0) curColor.Y -= cg;
                else curColor.Y += cg;
                if (db < 0) curColor.Z -= cb;
                else curColor.Z += cb;

                mShadingFloats[i] = curColor;

                var r = (byte) ((curColor.X / 2.0f) * 255.0f);
                var g = (byte) ((curColor.Y / 2.0f) * 255.0f);
                var b = (byte) ((curColor.Z / 2.0f) * 255.0f);
                var a = (byte) ((curColor.W / 2.0f) * 255.0f);

                var color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
                Vertices[i].Color = color;
            }

            return changed;
        }

        private bool HandleElevateTerrain(Editing.TerrainChangeParameters parameters)
        {
            var amount = parameters.Amount * (float) parameters.TimeDiff.TotalSeconds;
            var changed = false;
            var radius = parameters.OuterRadius;

            for(var i = 0; i < 145; ++i)
            {
                var p = Vertices[i].Position;
                var dist = (p - parameters.Center).Length();
                if (dist > radius)
                    continue;

                changed = true;
                var factor = dist / radius;

                switch(parameters.Algorithm)
                {
                    case Editing.TerrainAlgorithm.Flat:
                        p.Z += amount;
                        break;

                    case Editing.TerrainAlgorithm.Linear:
                        p.Z += (amount * (1.0f - factor));
                        break;

                    case Editing.TerrainAlgorithm.Quadratic:
                        p.Z += ((-amount) / (radius * radius) * (dist * dist)) + amount;
                        break;

                    case Editing.TerrainAlgorithm.Trigonometric:
                        var cs = Math.Cos(factor * Math.PI / 2);
                        p.Z += amount * (float) cs;
                        break;
                }

                if (p.Z < mMinHeight)
                    mMinHeight = p.Z;
                if (p.Z > mMaxHeight)
                    mMaxHeight = p.Z;

                Vertices[i].Position = p;
            }

            return changed;
        }

        public override void Dispose()
        {
            Textures.Clear();
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.Editing;
using Neo.Scene;
using OpenTK;
using SlimTK;
using Warcraft.Core;

namespace Neo.IO.Files.Terrain.Wotlk
{
	public class MapChunk : Terrain.MapChunk
    {
        private WeakReference<MapArea> mParent;
        private Mcnk mHeader;

        private Vector4[] mShadingFloats = new Vector4[145];
        private byte[] mAlphaCompressed;
        private static readonly uint[] Indices = new uint[768];
        private Dictionary<uint, DataChunk> mSaveChunks = new Dictionary<uint, DataChunk>();
        private byte[] mNormalExtra;
        private int[] mWmoRefs;

        public bool HasMccv { get; private set; }
        public override int AreaId { get { return this.mHeader.AreaId; } set { this.mHeader.AreaId = value; } }
        public override uint Flags { get { return this.mHeader.Flags; } set { this.mHeader.Flags = value; } }
        public override Vector3 Position => this.mHeader.Position;

        public MapChunk(int indexX, int indexY, WeakReference<MapArea> parent)
        {
	        this.SpecularFactors = new float[4];
            MapArea area;
            parent.TryGetTarget(out area);
	        this.Parent = new WeakReference<Terrain.MapArea>(area);
	        this.IndexX = indexX;
	        this.IndexY = indexY;
	        this.mParent = parent;
	        this.TextureScales = new float[] { 1, 1, 1, 1 };
            for (var i = 0; i < 145; ++i)
            {
	            this.mShadingFloats[i] = Vector4.One;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.mSaveChunks != null)
            {
	            this.mSaveChunks.Clear();
	            this.mSaveChunks = null;
            }

	        this.mParent = null;
	        this.mShadingFloats = null;
	        this.mAlphaCompressed = null;
	        this.mLayers = null;
	        this.mNormalExtra = null;

            base.Dispose(disposing);
        }

        public void AddDoodad(int mcrfValue, BoundingBox box)
        {
            var references = this.DoodadReferences;
            Array.Resize(ref references, references.Length + 1);
            references[references.Length - 1] = mcrfValue;
	        this.DoodadReferences = references;

            var min = box.Minimum;
            var max = box.Maximum;

            var cmin = this.ModelBox.Minimum;
            var cmax = this.ModelBox.Maximum;

            if (min.X < cmin.X)
            {
	            cmin.X = min.X;
            }
	        if (min.Y < cmin.Y)
	        {
		        cmin.Y = min.Y;
	        }
	        if (min.Z < cmin.Z)
	        {
		        cmin.Z = min.Z;
	        }
	        if (max.X > cmax.X)
	        {
		        cmax.X = max.X;
	        }
	        if (max.Y > cmax.Y)
	        {
		        cmax.Y = max.Y;
	        }
	        if (max.Z > cmax.Z)
	        {
		        cmax.Z = max.Z;
	        }

	        this.ModelBox = new BoundingBox(cmin, cmax);
            MapArea parent;
            if (this.mParent.TryGetTarget(out parent))
            {
	            parent.UpdateModelBox(this.ModelBox);
            }

	        this.DoodadsChanged = true;
        }

        public void TryAddDoodad(int mcrfValue, BoundingBox box)
        {
            var chunkBox = new BoundingBox(new Vector3(this.BoundingBox.Minimum.X, this.BoundingBox.Minimum.Y, float.MinValue),
                new Vector3(this.BoundingBox.Maximum.X, this.BoundingBox.Maximum.Y, float.MaxValue));

            var intersects = chunkBox.Intersects(ref box);
            if (intersects == false)
            {
	            return;
            }

	        var references = this.DoodadReferences;
            Array.Resize(ref references, references.Length + 1);
            references[references.Length - 1] = mcrfValue;
	        this.DoodadReferences = references;

            var min = box.Minimum;
            var max = box.Maximum;

            var cmin = this.ModelBox.Minimum;
            var cmax = this.ModelBox.Maximum;

            if (min.X < cmin.X)
            {
	            cmin.X = min.X;
            }
	        if (min.Y < cmin.Y)
	        {
		        cmin.Y = min.Y;
	        }
	        if (min.Z < cmin.Z)
	        {
		        cmin.Z = min.Z;
	        }
	        if (max.X > cmax.X)
	        {
		        cmax.X = max.X;
	        }
	        if (max.Y > cmax.Y)
	        {
		        cmax.Y = max.Y;
	        }
	        if (max.Z > cmax.Z)
	        {
		        cmax.Z = max.Z;
	        }

	        this.ModelBox = new BoundingBox(cmin, cmax);
            MapArea parent;
            if (this.mParent.TryGetTarget(out parent))
            {
	            parent.UpdateModelBox(this.ModelBox);
            }

	        this.DoodadsChanged = true;
        }

        public void SaveChunk(BinaryWriter writer)
        {
            int unusedSize;
            var basePos = (int)writer.BaseStream.Position;
            writer.Write(0x4D434E4B);
            writer.Write(0);
            var header = this.mHeader;
            var headerPos = writer.BaseStream.Position;
            var startPos = writer.BaseStream.Position;
            writer.Write(header);

            SaveHeights(writer, basePos, ref header);
            SaveMccv(writer, basePos, ref header);
            SaveNormals(writer, basePos, ref header);

            // INFO: SaveAlpha must be called before SaveLayers since SaveAlpha modifies the layer flags
            int alphaChunkSize;
            var alphaStream = SaveAlpha(ref header, out alphaChunkSize);
            SaveLayers(writer, basePos, ref header);

            header.Mcrf = (int)writer.BaseStream.Position - basePos;
            var references = this.DoodadReferences.Concat(this.mWmoRefs).ToArray();
            writer.Write(0x4D435246);
            writer.Write(references.Length * 4);
            writer.WriteArray(references);
            header.NumDoodadRefs = this.DoodadReferences.Length;

            //SaveUnusedChunk(writer, 0x4D435246, basePos, out header.Mcrf, out unusedSize);
            SaveUnusedChunk(writer, 0x4D435348, basePos, out header.Mcsh, out unusedSize);

            // Noggit panics when MCAL is not after MCLY even though there is no reason
            // that any sane person would imply an order in an interchangeable file format,
            // but lets make them happy anyway.
            header.Mcal = (int)writer.BaseStream.Position - basePos;
            writer.Write(0x4D43414C);
            writer.Write(alphaChunkSize);
            writer.Write(alphaStream.ToArray());

            SaveUnusedChunks(writer, basePos, ref header);

            var endPos = writer.BaseStream.Position;
            writer.BaseStream.Position = headerPos;
            writer.Write(header);
            writer.BaseStream.Position = headerPos - 4;
            writer.Write((int)(endPos - startPos));
            writer.BaseStream.Position = endPos;
        }

        public bool AsyncLoad(BinaryReader reader, ChunkInfo chunkInfo)
        {
            // chunkInfo.Offset points to right after the MCNK signature, the offsets in the header are relative to the signature tho
            var basePosition = chunkInfo.Offset - 4;
            reader.BaseStream.Position = chunkInfo.Offset;
            reader.ReadInt32();
	        this.mHeader = reader.Read<Mcnk>();
            reader.BaseStream.Position = basePosition + this.mHeader.Mcvt;
            var signature = reader.ReadUInt32();
            reader.ReadInt32();
            if (signature != 0x4D435654)
            {
                Log.Error("Chunk is missing valid MCVT sub chunk");
                return false;
            }

            LoadMcvt(reader);

            reader.BaseStream.Position = basePosition + this.mHeader.Mcnr;
            signature = reader.ReadUInt32();
            reader.ReadInt32();

            if (signature != 0x4D434E52)
            {
                Log.Error("Chunk is missing valid MCNR sub chunk");
                return false;
            }

            LoadMcnr(reader);

            if (this.mHeader.Mcrf > 0)
            {
                reader.BaseStream.Position = basePosition + this.mHeader.Mcrf;
                signature = reader.ReadUInt32();
                var chunkSize = reader.ReadInt32();
                if (signature == 0x4D435246)
                {
	                LoadReferences(reader, chunkSize);
                }
            }

            var hasMccv = false;
            if (this.mHeader.Mccv != 0)
            {
                reader.BaseStream.Position = basePosition + this.mHeader.Mccv;
                signature = reader.ReadUInt32();
                reader.ReadInt32();
                if (signature == 0x4D434356)
                {
                    LoadMccv(reader);
                    hasMccv = true;
	                this.HasMccv = true;
                }
            }

            reader.BaseStream.Position = basePosition + this.mHeader.Mcly;
            signature = reader.ReadUInt32();
            var size = reader.ReadInt32();

            if (signature != 0x4D434C59)
            {
	            return false;
            }

	        LoadLayers(reader, size);

            if (this.mHeader.SizeAlpha > 8)
            {
                reader.BaseStream.Position = basePosition + this.mHeader.Mcal;
                signature = reader.ReadUInt32();
                if (signature == 0x4D43414C)
                {
                    reader.ReadInt32();
	                this.mAlphaCompressed = reader.ReadBytes(this.mHeader.SizeAlpha - 8);
                }
            }

            InitLayerData();

            if (this.mHeader.SizeShadow > 8 && this.mHeader.Mcsh > 0)
            {
                reader.BaseStream.Position = basePosition + this.mHeader.Mcsh + 8;
                var curPtr = 0;
                for (var i = 0; i < 64; ++i)
                {
                    for (var j = 0; j < 8; ++j)
                    {
                        var mask = reader.ReadByte();
                        for (var k = 0; k < 8; ++k)
                        {
	                        this.AlphaValues[curPtr] &= 0xFFFFFF00;
	                        this.AlphaValues[curPtr++] |= ((mask & (1 << k)) == 0) ? (byte)0xFF : (byte)0xCC;
                        }
                    }
                }
            }

            if (this.mHeader.Mclv > 0)
            {
                reader.BaseStream.Position = basePosition + this.mHeader.Mclv + 8;
                var colors = reader.ReadArray<uint>(145);
                for (var i = 0; i < 145; ++i)
                {
	                this.Vertices[i].AdditiveColor = colors[i];
                }
            }

            LoadHoles();
            LoadGroundEffectLayers();

            if (hasMccv == false)
            {
                for (var i = 0; i < 145; ++i)
                {
	                this.Vertices[i].Color = 0x7F7F7F7F;
                }
            }

            if (this.mHeader.Mcrf > 0)
            {
	            LoadUnusedChunk(0x4D435246, basePosition + this.mHeader.Mcrf, (this.mHeader.NumDoodadRefs + this.mHeader.NumMapObjRefs) * 4, reader);
            }
	        if (this.mHeader.SizeShadow > 0)
	        {
		        LoadUnusedChunk(0x4D435348, basePosition + this.mHeader.Mcsh, this.mHeader.SizeShadow, reader);
	        }
	        LoadUnusedChunk(0x4D435345, basePosition + this.mHeader.Mcse, this.mHeader.NumSoundEmitters * 0x1C, reader);
            if (this.mHeader.SizeLiquid > 8)
            {
	            LoadUnusedChunk(0x4D434C51, basePosition + this.mHeader.Mclq, this.mHeader.SizeLiquid - 8, reader);
            }
	        if (this.mHeader.Mclv > 0)
	        {
		        LoadUnusedChunk(0x4D434C56, basePosition + this.mHeader.Mclv, 0, reader);
	        }


	        WorldFrame.Instance.MapManager.OnLoadProgress();

            return true;
        }

        public override void UpdateNormals()
        {
            if (this.mUpdateNormals == false)
            {
	            return;
            }

	        this.mUpdateNormals = false;
            for (var i = 0; i < 145; ++i)
            {
                var p1 = this.Vertices[i].Position;
                var p2 = p1;
                var p3 = p1;
                var p4 = p1;
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
                if (mgr.GetLandHeight(p1.X, p1.Y, out h))
                {
	                p1.Z = h;
                }
	            if (mgr.GetLandHeight(p2.X, p2.Y, out h))
	            {
		            p2.Z = h;
	            }
	            if (mgr.GetLandHeight(p3.X, p3.Y, out h))
	            {
		            p3.Z = h;
	            }
	            if (mgr.GetLandHeight(p4.X, p4.Y, out h))
	            {
		            p4.Z = h;
	            }

	            var n1 = Vector3.Cross((p2 - v), (p1 - v));
                var n2 = Vector3.Cross((p3 - v), (p2 - v));
                var n3 = Vector3.Cross((p4 - v), (p3 - v));
                var n4 = Vector3.Cross((p1 - v), (p4 - v));

                var n = n1 + n2 + n3 + n4;
                n.Normalize();
                n *= -1;
                var tmp = n.X;
                n.X = n.Y;
                n.Y = tmp;

                n.X = ((sbyte)(n.X * 127)) / 127.0f;
                n.Y = ((sbyte)(n.Y * 127)) / 127.0f;
                n.Z = ((sbyte)(n.Z * 127)) / 127.0f;

	            this.Vertices[i].Normal = n;
            }

            MapArea parent;
	        this.mParent.TryGetTarget(out parent);
            if (parent != null)
            {
	            parent.UpdateVertices(this);
            }
        }

        public override bool OnTerrainChange(TerrainChangeParameters parameters)
        {
            var changed = base.OnTerrainChange(parameters);

            if (changed)
            {
                MapArea parent;
	            this.mParent.TryGetTarget(out parent);

                var omin = this.BoundingBox.Minimum;
                var omax = this.BoundingBox.Maximum;
	            this.BoundingBox = new BoundingBox(new Vector3(omin.X, omin.Y, this.mMinHeight),
                new Vector3(omax.X, omax.Y, this.mMaxHeight));

                if (parent != null)
                {
	                parent.UpdateBoundingBox(this.BoundingBox);
                }
            }

            return changed;
        }

        public bool Intersect(ref Ray ray, out float distance)
        {
            distance = float.MaxValue;
            if (this.BoundingBox.Intersects(ref ray) == false)
            {
	            return false;
            }

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
                Vector3.Subtract(ref this.Vertices[i1].Position, ref this.Vertices[i0].Position, out e1);
                Vector3.Subtract(ref this.Vertices[i2].Position, ref this.Vertices[i0].Position, out e2);

                Vector3.Cross(ref dir, ref e2, out p);
                float det;
                Vector3.Dot(ref e1, ref p, out det);

                if (Math.Abs(det) < 1e-4)
                {
	                continue;
                }

	            var invDet = 1.0f / det;
                Vector3.Subtract(ref orig, ref this.Vertices[i0].Position, out T);
                float u;
                Vector3.Dot(ref T, ref p, out u);
                u *= invDet;

                if (u < 0 || u > 1)
                {
	                continue;
                }

	            Vector3.Cross(ref T, ref e1, out q);
                float v;
                Vector3.Dot(ref dir, ref q, out v);
                v *= invDet;
                if (v < 0 || (u + v) > 1)
                {
	                continue;
                }

	            float t;
                Vector3.Dot(ref e2, ref q, out t);
                t *= invDet;

                if (t < 1e-4)
                {
	                continue;
                }

	            hasHit = true;
                if (t < minDist)
                {
	                minDist = t;
                }
            }

            if (hasHit)
            {
	            distance = minDist;
            }

	        return hasHit;
        }

        protected override bool HandleMccvPaint(TerrainChangeParameters parameters)
        {
            var amount = (parameters.Amount / 75.0f) * (float)parameters.TimeDiff.TotalSeconds;
            var changed = false;

            var destColor = parameters.Shading;
            if (parameters.Inverted)
            {
                destColor.X = 2 - destColor.X;
                destColor.Y = 2 - destColor.Y;
                destColor.Z = 2 - destColor.Z;
            }

            var radius = parameters.OuterRadius;
            for (var i = 0; i < 145; ++i)
            {
                var p = this.Vertices[i].Position;
                var dist = (new Vector2(p.X, p.Y) - new Vector2(parameters.Center.X, parameters.Center.Y)).Length;
                if (dist > radius)
                {
	                continue;
                }

	            this.HasMccv = true;
                changed = true;
                var factor = dist / radius;
                if (dist < parameters.InnerRadius)
                {
	                factor = 1.0f;
                }

	            var curColor = this.mShadingFloats[i];
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
                    {
	                    curColor.Z = destColor.X;
                    }
                }
                else
                {
                    curColor.Z += cr;
                    if (curColor.Z > destColor.X)
                    {
	                    curColor.Z = destColor.X;
                    }
                }
                if (dg < 0)
                {
                    curColor.Y -= cg;
                    if (curColor.Y < destColor.Y)
                    {
	                    curColor.Y = destColor.Y;
                    }
                }
                else
                {
                    curColor.Y += cg;
                    if (curColor.Y > destColor.Y)
                    {
	                    curColor.Y = destColor.Y;
                    }
                }
                if (db < 0)
                {
                    curColor.X -= cb;
                    if (curColor.X < destColor.Z)
                    {
	                    curColor.X = destColor.Z;
                    }
                }
                else
                {
                    curColor.X += cb;
                    if (curColor.X > destColor.Z)
                    {
	                    curColor.X = destColor.Z;
                    }
                }

	            this.mShadingFloats[i] = curColor;

                curColor.X = Math.Min(Math.Max(curColor.X, 0), 2);
                curColor.Y = Math.Min(Math.Max(curColor.Y, 0), 2);
                curColor.Z = Math.Min(Math.Max(curColor.Z, 0), 2);

                var r = (byte)((curColor.Z / 2.0f) * 255.0f);
                var g = (byte)((curColor.Y / 2.0f) * 255.0f);
                var b = (byte)((curColor.X / 2.0f) * 255.0f);
                var a = (byte)((curColor.W / 2.0f) * 255.0f);

                var color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
	            this.Vertices[i].Color = color;
            }

            return changed;
        }

        private void LoadUnusedChunk(uint signature, int offset, int size, BinaryReader reader)
        {
            if (offset == 0)
            {
	            return;
            }

	        reader.BaseStream.Position = offset;
            var sig = reader.ReadUInt32();
            if (sig != signature)
            {
                Log.Warning(
                    string.Format(
                        "Info: Expected signature {0:X8} inside chunk, got {1:X8}. Since this chunk is not used for rendering its ignored.",
                        signature, sig));
                return;
            }

            var dataSize = reader.ReadInt32();
            if (dataSize != size && size != 0)
            {
                Log.Warning(
                    string.Format(
                        "Info: Expected chunk size {0} was not the same as actual data size {1}. Chunk was: {2:X8}. Using expected chunk size.",
                        size, dataSize, signature));
            }

            var data = reader.ReadBytes(size);
            if (this.mSaveChunks.ContainsKey(signature))
            {
	            return;
            }

	        this.mSaveChunks.Add(signature, new DataChunk { Data = data, Signature = signature, Size = size });
        }

        private void LoadGroundEffectLayers()
        {
            for (var i = 0; i < 64; ++i)
            {
                var value = (i < 32) ? this.mHeader.Low1 : this.mHeader.Low2;
                var index = (i < 32) ? (i * 2) : ((i - 32) * 2);
                var layer = (value >> index) & 0x3;
	            this.GroundEffectLayer[i] = (int)layer;
            }
        }

        private void LoadMcvt(BinaryReader reader)
        {
            var heights = reader.ReadArray<float>(145);

            var posx = Metrics.MapMidPoint - this.mHeader.Position.Y;
            var posy = Metrics.MapMidPoint - this.mHeader.Position.X;
            var posz = this.mHeader.Position.Z;

            var counter = 0;

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < 17; ++i)
            {
                for (var j = 0; j < (((i % 2) != 0) ? 8 : 9); ++j)
                {
                    var height = posz + heights[counter];
                    var x = posx + j * Metrics.UnitSize;
                    if ((i % 2) != 0)
                    {
	                    x += 0.5f * Metrics.UnitSize;
                    }
	                var y = posy + i * Metrics.UnitSize * 0.5f;

	                this.Vertices[counter].Position = new Vector3(x, y, height);

                    if (height < minPos.Z)
                    {
	                    minPos.Z = height;
                    }
	                if (height > maxPos.Z)
	                {
		                maxPos.Z = height;
	                }

	                if (x < minPos.X)
	                {
		                minPos.X = x;
	                }
	                if (x > maxPos.X)
	                {
		                maxPos.X = x;
	                }
	                if (y < minPos.Y)
	                {
		                minPos.Y = y;
	                }
	                if (y > maxPos.Y)
	                {
		                maxPos.Y = y;
	                }

	                this.Vertices[counter].TexCoordAlpha = new Vector2(j / 8.0f + ((i % 2) != 0 ? (0.5f / 8.0f) : 0), i / 16.0f);
	                this.Vertices[counter].TexCoord = new Vector2(j + ((i % 2) != 0 ? 0.5f : 0.0f), i * 0.5f);
                    ++counter;
                }
            }

	        this.mMinHeight = minPos.Z;
	        this.mMaxHeight = maxPos.Z;

	        this.BoundingBox = new BoundingBox(minPos, maxPos);
	        this.mMidPoint = minPos + (maxPos - minPos) / 2.0f;
        }

        private void LoadMcnr(BinaryReader reader)
        {
            var normals = reader.ReadArray<sbyte>(145 * 3);
	        this.mNormalExtra = reader.ReadBytes(13);

            for (var i = 0; i < 145; ++i)
            {
                var nx = -(normals[i * 3] / 127.0f);
                var ny = -(normals[i * 3 + 1] / 127.0f);
                var nz = normals[i * 3 + 2] / 127.0f;

	            this.Vertices[i].Normal = new Vector3(nx, ny, nz);
            }
        }

        private void LoadMccv(BinaryReader reader)
        {
            var colors = reader.ReadArray<uint>(145);
            for (var i = 0; i < 145; ++i)
            {
	            this.Vertices[i].Color = colors[i];
                var r = (colors[i] >> 16) & 0xFF;
                var g = (colors[i] >> 8) & 0xFF;
                var b = (colors[i]) & 0xFF;
                var a = (colors[i] >> 24) & 0xFF;

	            this.mShadingFloats[i] = new Vector4(b * 2.0f / 255.0f, g * 2.0f / 255.0f, r * 2.0f / 255.0f, a * 2.0f / 255.0f);
            }
        }

        private void LoadReferences(BinaryReader reader, int size)
        {
            var refs = reader.ReadArray<int>(size / 4);
	        this.mWmoRefs = this.mHeader.NumMapObjRefs > 0 ? refs.Skip(this.mHeader.NumDoodadRefs).ToArray() : new int[0];

            if (refs.Length < this.mHeader.NumDoodadRefs || this.mHeader.NumDoodadRefs == 0)
            {
	            return;
            }

	        this.DoodadReferences = refs.Take(this.mHeader.NumDoodadRefs).ToArray();
            var minPos = this.BoundingBox.Minimum;
            var maxPos = this.BoundingBox.Maximum;

            MapArea parent;
            if (this.mParent.TryGetTarget(out parent) == false)
            {
	            return;
            }

	        foreach (var reference in this.DoodadReferences)
            {
                var inst = parent.DoodadInstances[reference];
                var min = inst.BoundingBox.Minimum;
                var max = inst.BoundingBox.Maximum;

                if (min.X < minPos.X)
                {
	                minPos.X = min.X;
                }
	            if (min.Y < minPos.Y)
	            {
		            minPos.Y = min.Y;
	            }
	            if (min.Z < minPos.Z)
	            {
		            minPos.Z = min.Z;
	            }
	            if (max.X > maxPos.X)
	            {
		            maxPos.X = max.X;
	            }
	            if (max.Y > maxPos.Y)
	            {
		            maxPos.Y = max.Y;
	            }
	            if (max.Z > maxPos.Z)
	            {
		            maxPos.Z = max.Z;
	            }
            }

	        this.ModelBox = new BoundingBox(minPos, maxPos);
        }

        private void LoadLayers(BinaryReader reader, int size)
        {
	        this.mLayers = reader.ReadArray<Mcly>(size / SizeCache<Mcly>.Size);
            MapArea parent;
            if (this.mParent.TryGetTarget(out parent) == false)
            {
	            this.Textures = new List<Graphics.Texture>();
                return;
            }

	        this.Textures = this.mLayers.Select(l => parent.GetTexture(l.TextureId)).ToList();
	        this.SpecularTextures = this.mLayers.Select(l => parent.GetSpecularTexture(l.TextureId)).ToList();
            for (var i = 0; i < 4; ++i)
            {
                if (i >= this.mLayers.Length)
                {
	                this.SpecularFactors[i] = 0;
                    continue;
                }

	            this.SpecularFactors[i] = parent.IsSpecularTextureLoaded(this.mLayers[i].TextureId) ? 1.0f : 0.0f;
            }

	        this.TextureNames = this.mLayers.Select(l => parent.GetTextureName(l.TextureId)).ToArray();
        }

        private void LoadHoles()
        {
            for (var i = 0; i < 4; ++i)
            {
                for (var j = 0; j < 4; ++j)
                {
                    var baseIndex = i * 2 * 8 + j * 2;
                    var mask = (this.mHeader.Holes & (1 << (i * 4 + j))) != 0;
	                this.HoleValues[baseIndex] = this.HoleValues[baseIndex + 1] = this.HoleValues[baseIndex + 8] = this.HoleValues[baseIndex + 9] = (byte)(mask ? 0x00 : 0xFF);
                }
            }
        }

        private void InitLayerData()
        {
            var nLayers = Math.Min(this.mLayers.Length, 4);
            for (var i = 0; i < nLayers; ++i)
            {
                if ((this.mLayers[i].Flags & 0x200) != 0)
                {
	                LoadLayerRle(this.mLayers[i], i);
                }
                else if ((this.mLayers[i].Flags & 0x100) != 0)
                {
                    if (WorldFrame.Instance.MapManager.HasNewBlend)
                    {
	                    LoadUncompressed(this.mLayers[i], i);
                    }
                    else
                    {
	                    LoadLayerCompressed(this.mLayers[i], i);
                    }
                }
                else
                {
                    for (var j = 0; j < 4096; ++j)
                    {
	                    this.AlphaValues[j] |= 0xFFu << (8 * i);
                    }
                }
            }
        }

        private void LoadUncompressed(Mcly layerInfo, int layer)
        {
            var startPos = layerInfo.OfsMcal;
            for (var i = 0; i < 4096; ++i)
            {
	            this.AlphaValues[i] |= (uint)this.mAlphaCompressed[startPos++] << (8 * layer);
            }
        }

        private void LoadLayerCompressed(Mcly layerInfo, int layer)
        {
            var startPos = layerInfo.OfsMcal;
            var counter = 0;
            for (var k = 0; k < 63; ++k)
            {
                for (var j = 0; j < 32; ++j)
                {
                    var alpha = this.mAlphaCompressed[startPos++];
                    var val1 = alpha & 0xF;
                    var val2 = alpha >> 4;
                    val2 = j == 31 ? val1 : val2;
                    val1 = (byte)((val1 / 15.0f) * 255.0f);
                    val2 = (byte)((val2 / 15.0f) * 255.0f);
	                this.AlphaValues[counter++] |= (uint)val1 << (8 * layer);
	                this.AlphaValues[counter++] |= (uint)val2 << (8 * layer);
                }
            }

            for (uint j = 0; j < 64; ++j)
            {
	            this.AlphaValues[63 * 64 + j] |= (uint)(this.AlphaValues[(62 * 64) + j] & (0xFF << (layer * 8)));
            }
        }

        private void LoadLayerRle(Mcly layerInfo, int layer)
        {
            var counterOut = 0;
            var startPos = layerInfo.OfsMcal;
            while (counterOut < 4096)
            {
                var indicator = this.mAlphaCompressed[startPos++];
                if ((indicator & 0x80) != 0)
                {
                    var value = this.mAlphaCompressed[startPos++];
                    var repeat = indicator & 0x7F;
                    for (var k = 0; k < repeat && counterOut < 4096; ++k)
                    {
	                    this.AlphaValues[counterOut++] |= (uint)value << (layer * 8);
                    }
                }
                else
                {
                    for (var k = 0; k < (indicator & 0x7F) && counterOut < 4096; ++k)
                    {
	                    this.AlphaValues[counterOut++] |= (uint)this.mAlphaCompressed[startPos++] << (8 * layer);
                    }
                }
            }
        }

        private void SaveHeights(BinaryWriter writer, int basePosition, ref Mcnk header)
        {
            header.Mcvt = (int)writer.BaseStream.Position - basePosition;
            var minPos = this.Vertices.Min(v => v.Position.Z);
            header.Position.Z = minPos;
            var heights = this.Vertices.Select(v => v.Position.Z - minPos);
            writer.Write(0x4D435654);
            writer.Write(145 * 4);
            writer.WriteArray(heights.ToArray());
        }

        private void SaveNormals(BinaryWriter writer, int basePosition, ref Mcnk header)
        {
            header.Mcnr = (int)writer.BaseStream.Position - basePosition;

            var normals = this.Vertices.SelectMany(v => new[] { (sbyte)(v.Normal.X * -127.0f), (sbyte)(v.Normal.Y * -127.0f), (sbyte)(v.Normal.Z * 127.0f) });

            writer.Write(0x4D434E52);
            writer.Write(145 * 3);
            writer.WriteArray(normals.ToArray());
            writer.Write(this.mNormalExtra);
        }

        private void SaveMccv(BinaryWriter writer, int basePosition, ref Mcnk header)
        {
            if (this.HasMccv == false)
            {
                header.Mccv = 0;
                header.Flags &= ~0x40u;
                return;
            }

            header.Flags |= 0x40;

            var colors = this.mShadingFloats.Select(v =>
            {
                uint b = (byte)Math.Max(Math.Min((v.Z / 2.0f) * 255.0f, 255), 0);
                uint g = (byte)Math.Max(Math.Min((v.Y / 2.0f) * 255.0f, 255), 0);
                uint r = (byte)Math.Max(Math.Min((v.X / 2.0f) * 255.0f, 255), 0);
                return 0x7F000000 | (b << 16) | (g << 8) | r;
            }).ToArray();

            header.Mccv = (int)writer.BaseStream.Position - basePosition;
            writer.Write(0x4D434356);
            writer.Write(145 * 4);
            writer.WriteArray(colors.ToArray());
        }

        private void SaveLayers(BinaryWriter writer, int basePosition, ref Mcnk header)
        {
            header.NumLayers = this.mLayers.Length;
            if (header.NumLayers == 0)
            {
                header.Mcly = 0;
                return;
            }

            header.Mcly = (int)writer.BaseStream.Position - basePosition;
            writer.Write(0x4D434C59);
            writer.Write(this.mLayers.Length * SizeCache<Mcly>.Size);
            writer.WriteArray(this.mLayers);
        }

        private MemoryStream SaveAlpha(ref Mcnk header, out int chunkSize)
        {
            //header.Mcal = (int) writer.BaseStream.Position - basePosition;
            //writer.Write(0x4D43414C);
            //var sizePos = writer.BaseStream.Position;
            //writer.Write(0);
            if (this.mLayers.Length == 0)
            {
                header.SizeAlpha = 8;
                chunkSize = 0;
                return new MemoryStream();
            }

            var strm = new MemoryStream();
            var writer = new BinaryWriter(strm);

            var curPos = 0;
	        this.mLayers[0].Flags &= ~0x300u;
	        this.mLayers[0].OfsMcal = 0;
            for (var i = 1; i < this.mLayers.Length; ++i)
            {
                bool compressed;
                var data = GetSavedAlphaForLayer(i, out compressed);
	            this.mLayers[i].OfsMcal = curPos;
                if (compressed)
                {
	                this.mLayers[i].Flags |= 0x300;
                }
                else
                {
	                this.mLayers[i].Flags |= 0x100;
	                this.mLayers[i].Flags &= ~0x200u;
                }
                writer.Write(data);
                curPos += data.Length;
            }

            //var endPos = writer.BaseStream.Position;
            //writer.BaseStream.Position = sizePos;
            //writer.Write((int) (endPos - sizePos - 4));
            //writer.BaseStream.Position = endPos;

            chunkSize = (int)strm.Length;
            header.SizeAlpha = chunkSize + 8;
            return strm;
        }

        private void SaveUnusedChunks(BinaryWriter writer, int basePosition, ref Mcnk header)
        {
            int unusedSize;
            SaveUnusedChunk(writer, 0x4D435345, basePosition, out header.Mcse, out unusedSize);
            SaveUnusedChunk(writer, 0x4D434C51, basePosition, out header.Mclq, out unusedSize);
            SaveUnusedChunk(writer, 0x4D434C56, basePosition, out header.Mclv, out unusedSize);

            //header.NumSoundEmitters /= 0x1C;
        }

        private void SaveUnusedChunk(BinaryWriter writer, uint signature, int basePosition, out int offset, out int size, bool sizeWithHeader = true)
        {
            if (this.mSaveChunks.ContainsKey(signature) == false)
            {
                offset = 0;
                size = 0 + (sizeWithHeader ? 8 : 0);
                return;
            }

            var cnk = this.mSaveChunks[signature];
            size = cnk.Size + (sizeWithHeader ? 8 : 0);
            offset = (int)writer.BaseStream.Position - basePosition;
            writer.Write(signature);
            writer.Write(cnk.Size);
            writer.Write(cnk.Data);
        }

        private float CalculateAlphaHomogenity(int layer)
        {
            var numCompressable = 1;
            var lastAlpha = (this.AlphaValues[0] >> (layer * 8)) & 0xFF;
            for (var i = 1; i < 4096; ++i)
            {
                var value = (this.AlphaValues[i] >> (layer * 8)) & 0xFF;
                if (value == lastAlpha)
                {
	                ++numCompressable;
                }

	            lastAlpha = value;
            }

            return numCompressable / 4096.0f;
        }

        private byte[] GetSavedAlphaForLayer(int layer, out bool compressed)
        {
            compressed = false;
            var homogenity = CalculateAlphaHomogenity(layer);
            if (homogenity < float.MaxValue && WorldFrame.Instance.MapManager.HasNewBlend)
            {
                compressed = true;
                return GetAlphaCompressed(layer);
            }

            return GetAlphaUncompressed(layer);
        }

        protected override int AddTextureLayer(string textureName)
        {
            var old = this.TextureNames;
	        this.TextureNames = new string[old.Count + 1];
            for (var i = 0; i < old.Count; ++i)
            {
	            this.TextureNames[i] = old[i];
            }

	        this.TextureNames[this.TextureNames.Count - 1] = textureName;

            MapArea parent;
            if (this.mParent.TryGetTarget(out parent) == false)
            {
	            throw new InvalidOperationException("Couldnt get parent of map chunk");
            }

	        var texId = parent.GetOrAddTexture(textureName);
            var layer = new Mcly
            {
                Flags = 0,
                TextureId = texId,
                EffectId = -1,
                OfsMcal = 0,
                Padding = 0
            };

            var layers = this.mLayers;
	        this.mLayers = new Mcly[layers.Length + 1];
            for (var i = 0; i < layers.Length; ++i)
            {
	            this.mLayers[i] = layers[i];
            }

	        this.mLayers[layers.Length] = layer;

	        this.Textures.Add(parent.GetTexture(texId));
	        this.SpecularTextures.Add(parent.GetSpecularTexture(texId));
	        this.SpecularFactors[this.SpecularTextures.Count - 1] = parent.IsSpecularTextureLoaded(texId) ? 1 : 0;
	        this.TexturesChanged = true;
            return this.mLayers.Length - 1;
        }

        public override void SetHole(IntersectionParams intersection, bool add)
        {
            float holesize = CHUNKSIZE / 4.0f;
            var min = this.BoundingBox.Minimum;
            var intersect = new Vector2(intersection.TerrainPosition.X, intersection.TerrainPosition.Y);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    RectangleF bounds = new RectangleF
                    (
                        min.X + (x * holesize),
                        min.Y + (y * holesize),
                        holesize,
                        holesize
                    );

                    if (bounds.Contains(intersect))
                    {
                        var baseIndex = y * 2 * 8 + x * 2;
                        int bit = (1 << (y * 4 + x));

                        if (add)
                        {
	                        this.mHeader.Holes |= bit;
                        }
                        else
                        {
	                        this.mHeader.Holes &= ~bit;
                        }

	                    this.HoleValues[baseIndex] = this.HoleValues[baseIndex + 1] = this.HoleValues[baseIndex + 8] = this.HoleValues[baseIndex + 9] = (byte)(add ? 0x00 : 0xFF);
                        return;
                    }
                }
            }
        }

        public override void SetHoleBig(bool add)
        {
	        this.mHeader.Holes = add ? int.MaxValue : 0;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    var baseIndex = y * 2 * 8 + x * 2;
	                this.HoleValues[baseIndex] = this.HoleValues[baseIndex + 1] = this.HoleValues[baseIndex + 8] = this.HoleValues[baseIndex + 9] = (byte)(add ? 0x0 : 0xFF);
                }
            }
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

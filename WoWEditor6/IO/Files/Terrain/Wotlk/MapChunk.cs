using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX;
using WoWEditor6.Editing;
using WoWEditor6.Scene;

namespace WoWEditor6.IO.Files.Terrain.Wotlk
{
	class MapChunk : Terrain.MapChunk
	{
		private readonly WeakReference<MapArea> mParent;
		private Mcnk mHeader;

		private readonly Vector4[] mShadingFloats = new Vector4[145];
		private bool mForceMccv;
		private byte[] mAlphaCompressed;
		private List<Mcly> mLayers = new List<Mcly>();
		private static readonly uint[] Indices = new uint[768];

		public override uint[] RenderIndices => Indices;

		public MapChunk(int indexX, int indexY, WeakReference<MapArea> parent)
		{
			IndexX = indexX;
			IndexY = indexY;
			mParent = parent;
			TextureScales = new float[] {1, 1, 1, 1};
			for (var i = 0; i < 145; ++i) mShadingFloats[i] = Vector4.One;
		}

		public bool AsyncLoad(BinaryReader reader, ChunkInfo chunkInfo)
		{
			reader.BaseStream.Position = chunkInfo.Offset;

			var csize = reader.ReadInt32();
			mHeader = reader.Read<Mcnk>();
			var hasMccv = false;
			while(reader.BaseStream.Position + 8 < chunkInfo.Offset + 4 + csize)
			{
				var signature = reader.ReadUInt32();
				var size = reader.ReadInt32();
				if (reader.BaseStream.Position + size > chunkInfo.Offset + 4 + csize)
					break;

				var cur = reader.BaseStream.Position;

				switch (signature)
				{
					case 0x4D435654:
						LoadMcvt(reader);
						break;

					case 0x4D434E52:
						size += 13;
						LoadMcnr(reader);
						break;

					case 0x4D434356:
						hasMccv = true;
						LoadMccv(reader);
						break;

					case 0x4D434C59:
						LoadLayers(reader, size);
						break;

					case 0x4D43414C:
						mAlphaCompressed = reader.ReadBytes(size);
						break;
				}
				
				reader.BaseStream.Position = cur + size;
			}

			if (hasMccv == false)
			{
				for (var i = 0; i < 145; ++i)
					Vertices[i].Color = 0x7F7F7F7F;
			}

			InitLayerData();

			WorldFrame.Instance.MapManager.OnLoadProgress();

			return true;
		}

		public override void UpdateNormals()
		{
			if (mUpdateNormals == false)
				return;

			mUpdateNormals = false;
			for (var i = 0; i < 145; ++i)
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
				if (mgr.GetLandHeight(p1.X, p1.Y, out h)) p1.Z = h;
				if (mgr.GetLandHeight(p2.X, p2.Y, out h)) p2.Z = h;
				if (mgr.GetLandHeight(p3.X, p3.Y, out h)) p3.Z = h;
				if (mgr.GetLandHeight(p4.X, p4.Y, out h)) p4.Z = h;

				var n1 = Vector3.Cross((p2 - v), (p1 - v));
				var n2 = Vector3.Cross((p3 - v), (p2 - v));
				var n3 = Vector3.Cross((p4 - v), (p3 - v));
				var n4 = Vector3.Cross((p1 - v), (p4 - v));

				var n = n1 + n2 + n3 + n4;
				n.Normalize();
				n *= -1;

				n.X = ((sbyte)(n.X * 127)) / 127.0f;
				n.Y = ((sbyte)(n.Y * 127)) / 127.0f;
				n.Z = ((sbyte)(n.Z * 127)) / 127.0f;

				Vertices[i].Normal = n;
			}

			MapArea parent;
			mParent.TryGetTarget(out parent);
			parent?.UpdateVertices(this);
		}

		public override bool OnTerrainChange(TerrainChangeParameters parameters)
		{
			var changed = base.OnTerrainChange(parameters);

			if (changed)
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

			if (hasHit)
				distance = minDist;

			return hasHit;
		}

		public override void Dispose()
		{

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
				var p = Vertices[i].Position;
				var dist = (p - parameters.Center).Length();
				if (dist > radius)
					continue;

				mForceMccv = true;
				changed = true;
				var factor = dist / radius;
				if (dist < parameters.InnerRadius)
					factor = 1.0f;

				var curColor = mShadingFloats[i];
				var dr = destColor.X - curColor.X;
				var dg = destColor.Y - curColor.Y;
				var db = destColor.Z - curColor.Z;

				var cr = Math.Min(Math.Abs(dr), amount * factor);
				var cg = Math.Min(Math.Abs(dg), amount * factor);
				var cb = Math.Min(Math.Abs(db), amount * factor);

				if (dr < 0)
				{
					curColor.Z -= cr;
					if (curColor.Z < destColor.Z)
						curColor.Z = destColor.Z;
				}
				else
				{
					curColor.Z += cr;
					if (curColor.Z > destColor.Z)
						curColor.Z = destColor.Z;
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
					if (curColor.X < destColor.X)
						curColor.X = destColor.Y;
				}
				else
				{
					curColor.X += cb;
					if (curColor.X > destColor.X)
						curColor.X = destColor.X;
				}

				mShadingFloats[i] = curColor;

				curColor.X = Math.Min(Math.Max(curColor.X, 0), 2);
				curColor.Y = Math.Min(Math.Max(curColor.Y, 0), 2);
				curColor.Z = Math.Min(Math.Max(curColor.Z, 0), 2);

				var r = (byte)((curColor.Z / 2.0f) * 255.0f);
				var g = (byte)((curColor.Y / 2.0f) * 255.0f);
				var b = (byte)((curColor.X / 2.0f) * 255.0f);
				var a = (byte)((curColor.W / 2.0f) * 255.0f);

				var color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
				Vertices[i].Color = color;
			}

			return changed;
		}

		private void LoadMcvt(BinaryReader reader)
		{
			var heights = reader.ReadArray<float>(145);

			var posx = Metrics.MapMidPoint - mHeader.Position.Y;
			var posy = Metrics.MapMidPoint - mHeader.Position.X;
			var posz = mHeader.Position.Z;

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
						x += 0.5f * Metrics.UnitSize;
					var y = posy + i * Metrics.UnitSize * 0.5f;

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

		private void LoadMcnr(BinaryReader reader)
		{
			var normals = reader.ReadArray<sbyte>(145 * 3);
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

		private void LoadMccv(BinaryReader reader)
		{
			var colors = reader.ReadArray<uint>(145);
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

		private void LoadLayers(BinaryReader reader, int size)
		{
			mLayers = reader.ReadArray<Mcly>(size / SizeCache<Mcly>.Size).ToList();
			MapArea parent;
			if(mParent.TryGetTarget(out parent) == false)
			{
				Textures = new List<Graphics.Texture>();
				return;
			}

			Textures = mLayers.Select(l => parent.GetTexture(l.TextureId)).ToList().AsReadOnly();
		}

		private void InitLayerData()
		{
			var nLayers = Math.Min(mLayers.Count, 4);
			for (var i = 0; i < nLayers; ++i)
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

			mAlphaCompressed = null;
		}

		private void LoadUncompressed(Mcly layerInfo, int layer)
		{
			var startPos = layerInfo.OfsMcal;
			for (var i = 0; i < 4096; ++i)
				AlphaValues[i] |= (uint)mAlphaCompressed[startPos++] << (8 * layer);
		}

		private void LoadLayerCompressed(Mcly layerInfo, int layer)
		{
			var startPos = layerInfo.OfsMcal;
			var counter = 0;
			for (var k = 0; k < 63; ++k)
			{
				for (var j = 0; j < 32; ++j)
				{
					var alpha = mAlphaCompressed[startPos++];
					var val1 = alpha & 0xF;
					var val2 = alpha >> 4;
					val2 = j == 31 ? val1 : val2;
					val1 = (byte)((val1 / 15.0f) * 255.0f);
					val2 = (byte)((val2 / 15.0f) * 255.0f);
					AlphaValues[counter++] |= (uint)val1 << (8 * layer);
					AlphaValues[counter++] |= (uint)val2 << (8 * layer);
				}
			}

			for (uint j = 0; j < 64; ++j)
			{
				AlphaValues[63 * 64 + j] |= (uint)(AlphaValues[(62 * 64) + j] & (0xFF << (layer * 8)));
			}
		}

		private void LoadLayerRle(Mcly layerInfo, int layer)
		{
			var counterOut = 0;
			var startPos = layerInfo.OfsMcal;
			while (counterOut < 4096)
			{
				var indicator = mAlphaCompressed[startPos++];
				if ((indicator & 0x80) != 0)
				{
					var value = mAlphaCompressed[startPos++];
					var repeat = indicator & 0x7F;
					for (var k = 0; k < repeat && counterOut < 4096; ++k)
						AlphaValues[counterOut++] |= (uint)value << (layer * 8);
				}
				else
				{
					for (var k = 0; k < (indicator & 0x7F) && counterOut < 4096; ++k)
						AlphaValues[counterOut++] |= (uint)mAlphaCompressed[startPos++] << (8 * layer);
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

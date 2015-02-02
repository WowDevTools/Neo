using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX;
using WoWEditor6.Editing;
using WoWEditor6.Scene.Texture;

namespace WoWEditor6.IO.Files.Terrain.Wotlk
{
	struct ChunkInfo
	{
		public int Offset;
		public int Size;
	}

	class MapArea : Terrain.MapArea
	{
		private readonly List<ChunkInfo> mChunkInfos = new List<ChunkInfo>();
		private readonly List<string> mTextureNames = new List<string>();
		private readonly List<Graphics.Texture> mTextures = new List<Graphics.Texture>();
		private readonly MapChunk[] mChunks = new MapChunk[256];

		private bool mWasChanged;

		public MapArea(string continent, int ix, int iy)
		{
			Continent = continent;
			IndexX = ix;
			IndexY = iy;
		}

		public void UpdateBoundingBox(BoundingBox chunkBox)
		{
			var minPos = chunkBox.Minimum;
			var maxPos = chunkBox.Maximum;

			var omin = BoundingBox.Minimum;
			var omax = BoundingBox.Maximum;

			omin.X = Math.Min(omin.X, minPos.X);
			omin.Y = Math.Min(omin.Y, minPos.Y);
			omin.Z = Math.Min(omin.Z, minPos.Z);
			omax.X = Math.Max(omax.X, maxPos.X);
			omax.Y = Math.Max(omax.Y, maxPos.Y);
			omax.Z = Math.Max(omax.Z, maxPos.Z);

			BoundingBox = new BoundingBox(omin, omax);
		}

		public void UpdateVertices(MapChunk chunk)
		{
			if (chunk == null)
				return;

			var ix = chunk.IndexX;
			var iy = chunk.IndexY;

			var index = (ix + iy * 16) * 145;
			for (var i = 0; i < 145; ++i)
				FullVertices[i + index] = chunk.Vertices[i];
		}

		public override void Save()
		{
			if (mWasChanged == false)
				return;

			throw new NotImplementedException();
		}

		public override Graphics.Texture GetTexture(int index)
		{
			if (index >= mTextures.Count)
				throw new IndexOutOfRangeException();

			return mTextures[index];
		}

		public override void AsyncLoad()
		{
			using (var file = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", Continent, IndexX, IndexY)))
			{
				if(file == null)
				{
					IsValid = false;
					return;
				}

				var reader = new BinaryReader(file);
				InitChunkInfos(reader);
				InitTextures(reader);
				InitChunks(reader);
			}
		}

		public override Terrain.MapChunk GetChunk(int index)
		{
			if (index >= mChunks.Length)
				throw new IndexOutOfRangeException();

			return mChunks[index];
		}

		public override bool Intersect(ref Ray ray, out Terrain.MapChunk chunk, out float distance)
		{
			distance = float.MaxValue;
			chunk = null;

			var mindistance = float.MaxValue;
			if (BoundingBox.Intersects(ref ray) == false)
				return false;

			Terrain.MapChunk chunkHit = null;
			var hasHit = false;
			foreach (var cnk in mChunks)
			{
				float dist;
				if (cnk.Intersect(ref ray, out dist) == false)
					continue;

				hasHit = true;
				if (dist >= mindistance) continue;

				mindistance = dist;
				chunkHit = cnk;
			}

			chunk = chunkHit;
			distance = mindistance;
			return hasHit;
		}

		public override void Dispose()
		{

		}

		public override bool OnChangeTerrain(TerrainChangeParameters parameters)
		{
			var changed = false;
			foreach (var chunk in mChunks)
			{
				if (chunk?.OnTerrainChange(parameters) ?? false)
					changed = true;
			}

			if (changed)
				mWasChanged = true;

			return changed;
		}

		public override void UpdateNormals()
		{
			foreach (var chunk in mChunks)
				chunk?.UpdateNormals();
		}

		private void InitChunkInfos(BinaryReader reader)
		{
			reader.BaseStream.Position = 0;
			for(var i = 0; i < 256; ++i)
			{
				if (SeekNextMcnk(reader) == false)
					throw new InvalidOperationException("Area is missing chunks");

				mChunkInfos.Add(new ChunkInfo
				{
					Offset = (int)(reader.BaseStream.Position),
					Size = reader.ReadInt32()
				});

				reader.ReadBytes(mChunkInfos.Last().Size);
			}
		}

		private void InitTextures(BinaryReader reader)
		{
			if (SeekChunk(reader, 0x4D544558) == false)
				return;

			var size = reader.ReadInt32();
			var bytes = reader.ReadBytes(size);
			var fullString = Encoding.ASCII.GetString(bytes);
			mTextureNames.AddRange(fullString.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries));
			mTextureNames.ForEach(t => mTextures.Add(TextureManager.Instance.GetTexture(t)));
		}

		private void InitChunks(BinaryReader reader)
		{
			var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			var modelMin = new Vector3(float.MaxValue);
			var modelMax = new Vector3(float.MinValue);

			for (var i = 0; i < 256; ++i)
			{
				var chunk = new MapChunk(i % 16, i / 16, new WeakReference<MapArea>(this));
				if (chunk.AsyncLoad(reader, mChunkInfos[i]) == false)
					throw new InvalidOperationException("Unable to load chunk");

				var bbmin = chunk.BoundingBox.Minimum;
				var bbmax = chunk.BoundingBox.Maximum;
				if (bbmin.X < minPos.X)
					minPos.X = bbmin.X;
				if (bbmax.X > maxPos.X)
					maxPos.X = bbmax.X;
				if (bbmin.Y < minPos.Y)
					minPos.Y = bbmin.Y;
				if (bbmax.Y > maxPos.Y)
					maxPos.Y = bbmax.Y;
				if (bbmin.Z < minPos.Z)
					minPos.Z = bbmin.Z;
				if (bbmax.Z > maxPos.Z)
					maxPos.Z = bbmax.Z;

				bbmin = chunk.ModelBox.Minimum;
				bbmax = chunk.ModelBox.Maximum;
				if (bbmin.X < modelMin.X)
					modelMin.X = bbmin.X;
				if (bbmax.X > modelMax.X)
					modelMax.X = bbmax.X;
				if (bbmin.Y < modelMin.Y)
					modelMin.Y = bbmin.Y;
				if (bbmax.Y > modelMax.Y)
					modelMax.Y = bbmax.Y;
				if (bbmin.Z < modelMin.Z)
					modelMin.Z = bbmin.Z;
				if (bbmax.Z > modelMax.Z)
					modelMax.Z = bbmax.Z;

				mChunks[i] = chunk;
				Array.Copy(chunk.Vertices, 0, FullVertices, i * 145, 145);
			}

			BoundingBox = new BoundingBox(minPos, maxPos);
			ModelBox = new BoundingBox(modelMin, modelMax);
		}

		private static bool SeekNextMcnk(BinaryReader reader) => SeekChunk(reader, 0x4D434E4B, false);

		private static bool SeekChunk(BinaryReader reader, uint signature, bool begin = true)
		{
			if (begin)
				reader.BaseStream.Position = 0;

			try
			{
				var sig = reader.ReadUInt32();
				while (sig != signature)
				{
					var size = reader.ReadInt32();
					reader.ReadBytes(size);
					sig = reader.ReadUInt32();
				}

				return sig == signature;
			}
			catch (EndOfStreamException)
			{
				return false;
			}
		}
	}
}

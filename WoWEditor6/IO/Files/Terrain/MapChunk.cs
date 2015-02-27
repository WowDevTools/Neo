using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using WoWEditor6.Scene;

namespace WoWEditor6.IO.Files.Terrain
{
    abstract class MapChunk : IDisposable
    {
        protected Vector3 mMidPoint;
        protected float mMinHeight = float.MaxValue;
        protected float mMaxHeight = float.MinValue;
        protected bool mUpdateNormals;

        public int IndexX { get; protected set; }
        public int IndexY { get; protected set; }

        public int StartVertex {get{return (IndexX + IndexY * 16) * 145;}}

        public AdtVertex[] Vertices { get; private set; }
        public uint[] AlphaValues { get; private set; }
        public byte[] HoleValues { get; private set; }
        public IList<Graphics.Texture> Textures { get; protected set; }
        public BoundingBox BoundingBox { get; protected set; }
        public BoundingBox ModelBox { get; protected set; }
        public float[] TextureScales { get; protected set; }

        public int[] DoodadReferences { get; protected set; }

        public bool IsAlphaChanged { get; set; }
        public bool TexturesChanged { get; set; }

        public WeakReference<MapArea> Parent { get; protected set; }

        protected MapChunk()
        {
            HoleValues = new byte[64];
            for (var i = 0; i < 64; ++i) HoleValues[i] = 0xFF;

            Vertices = new AdtVertex[145];
            AlphaValues = new uint[4096];
            DoodadReferences = new int[0];
        }

        public virtual void Dispose()
        {

        }

        public abstract bool OnTextureTerrain(Editing.TextureChangeParameters parameters);

        public virtual bool OnTerrainChange(Editing.TerrainChangeParameters parameters)
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
            switch (parameters.Method)
            {
                case Editing.TerrainChangeType.Elevate:
                    changed = HandleElevateTerrain(parameters);
                    break;

                case Editing.TerrainChangeType.Flatten:
                    changed = HandleFlatten(parameters);
                    break;

                case Editing.TerrainChangeType.Blur:
                    changed = HandleBlur(parameters);
                    break;

                case Editing.TerrainChangeType.Shading:
                    changed = HandleMccvPaint(parameters);
                    break;
            }

            return changed;
        }

        public virtual void UpdateNormals()
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
                var tmp = n.Y;
                n.Y = n.X;
                n.X = tmp;
                n.Y *= -1;
                n.Z *= -1;

                n.X = ((sbyte) (n.X * 127)) / 127.0f;
                n.Y = ((sbyte) (n.Y * 127)) / 127.0f;
                n.Z = ((sbyte) (n.Z * 127)) / 127.0f;

                Vertices[i].Normal = n;
            }
        }

        protected abstract bool HandleMccvPaint(Editing.TerrainChangeParameters parameters);

        private bool HandleBlur(Editing.TerrainChangeParameters parameters)
        {
            var radius = parameters.OuterRadius;
            var amount = parameters.Amount / 550.0f;
            if (amount > 1) amount = 1;

            amount = 1 - amount;
            var changed = false;

            for(var i = 0; i < 145; ++i)
            {
                var p = Vertices[i].Position;
                var dist = (new Vector2(p.X, p.Y) - new Vector2(parameters.Center.X, parameters.Center.Y)).Length();
                if (dist > radius)
                    continue;

                changed = true;
                var totalHeight = 0.0f;
                var totalWeight = 0.0f;
                var rad = (int) (radius / Metrics.UnitSize);
                for(var j = -rad; j <= rad; ++j)
                {
                    var ty = parameters.Center.Y + j * Metrics.UnitSize;
                    for(var k = -rad; k <= rad; ++k)
                    {
                        var tx = parameters.Center.X + k * Metrics.UnitSize;
                        var xdiff = tx - p.X;
                        var ydiff = ty - p.Y;
                        var diff = xdiff * xdiff + ydiff * ydiff;
                        if (diff > radius * radius)
                            continue;

                        float height;
                        if (WorldFrame.Instance.MapManager.GetLandHeight(tx, 64.0f * Metrics.TileSize - ty, out height) ==
                            false)
                            height = p.Z;

                        var dist2 = (float) Math.Sqrt(diff);
                        totalHeight += (1 - dist2 / radius) * height;
                        totalWeight += (1 - dist2 / radius);
                    }
                }

                var h = totalHeight / totalWeight;
                switch(parameters.Algorithm)
                {
                    case Editing.TerrainAlgorithm.Flat:
                        p.Z = amount * p.Z + (1 - amount) * h;
                        break;

                    case Editing.TerrainAlgorithm.Linear:
                    {
                        var nremain = 1 - (1 - amount) * (1 - dist / radius);
                        p.Z = nremain * p.Z + (1 - nremain) * h;
                    }
                        break;

                    case Editing.TerrainAlgorithm.Quadratic:
                    {
                        var nremain = 1 - (float)Math.Pow(1 - amount, 1 + dist / radius);
                        p.Z = nremain * p.Z + (1 - nremain) * h;
                    }
                        break;

                    case Editing.TerrainAlgorithm.Trigonometric:
                    {
                        var nremain = 1 - (1 - amount) * (float)Math.Cos(dist / radius);
                        p.Z = nremain * p.Z + (1 - nremain) * h;
                    }
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
                var dist = (new Vector2(p.X, p.Y) - new Vector2(parameters.Center.X, parameters.Center.Y)).Length();
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

        private bool HandleElevateTerrain(Editing.TerrainChangeParameters parameters)
        {
            var amount = parameters.Amount * (float) parameters.TimeDiff.TotalSeconds;
            var changed = false;
            var radius = parameters.OuterRadius;

            for(var i = 0; i < 145; ++i)
            {
                var p = Vertices[i].Position;
                var dist = (new Vector2(p.X, p.Y) - new Vector2(parameters.Center.X, parameters.Center.Y)).Length();
                if (dist > radius)
                    continue;

                changed = true;
                var factor = dist / radius;

                switch(parameters.Algorithm)
                {
                    case Editing.TerrainAlgorithm.Flat:
                        p.Z += amount * (parameters.Inverted ? -1 : 1);
                        break;

                    case Editing.TerrainAlgorithm.Linear:
                        p.Z += (amount * (1.0f - factor)) * (parameters.Inverted ? -1 : 1);
                        break;

                    case Editing.TerrainAlgorithm.Quadratic:
                        p.Z += (((-amount) / (radius * radius) * (dist * dist)) + amount) * (parameters.Inverted ? -1 : 1);
                        break;

                    case Editing.TerrainAlgorithm.Trigonometric:
                        var cs = Math.Cos(factor * Math.PI / 2);
                        p.Z += (amount * (float) cs) * (parameters.Inverted ? -1 : 1);
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

        private void CompressRow(Stream strm, int row, int layer)
        {
            var baseIndex = row * 64;

            // step 1: find ranges of identical values
            var ranges = new List<Tuple<int, int>>();
            var lastValue = (byte)((AlphaValues[baseIndex] >> (layer * 8)) & 0xFF);
            var curRangeStart = 0;
            for (var i = 1; i < 64; ++i)
            {
                var cur = (byte)((AlphaValues[i + baseIndex] >> (layer * 8)) & 0xFF);
                if (cur == lastValue)
                    continue;

                if (i - curRangeStart > 1)
                    ranges.Add(new Tuple<int, int>(curRangeStart, i));

                curRangeStart = i;
                lastValue = cur;
            }

            // step 2: Write the ranges appropriately
            var read = 0;
            while (read < 64)
            {
                var range = ranges.Count > 0 ? ranges[0] : null;
                if (range != null && range.Item1 == read)
                {
                    var value = (byte)((AlphaValues[read + baseIndex] >> (layer * 8)) & 0xFF);
                    var repeatCount = range.Item2 - range.Item1;
                    while (repeatCount >= 0x7F)
                    {
                        strm.WriteByte(0xFF);
                        strm.WriteByte(value);
                        repeatCount -= 0x7F;
                    }

                    if (repeatCount > 0)
                    {
                        strm.WriteByte((byte)(0x80 | repeatCount));
                        strm.WriteByte(value);
                    }

                    ranges.RemoveAt(0);

                    read = range.Item2;
                }
                else
                {
                    var nextRange = ranges.Count > 0 ? ranges[0] : null;
                    int repeatCount;
                    if (nextRange == null)
                        repeatCount = 64 - read;
                    else
                        repeatCount = nextRange.Item1 - read;

                    while (repeatCount >= 0x7F)
                    {
                        strm.WriteByte(0x7F);
                        for (var i = 0; i < 0x7F; ++i)
                            strm.WriteByte((byte)((AlphaValues[baseIndex + read++] >> (layer * 8)) & 0xFF));

                        repeatCount -= 0x7F;
                    }

                    if (repeatCount > 0)
                    {
                        strm.WriteByte((byte)repeatCount);
                        for (var i = 0; i < repeatCount; ++i)
                            strm.WriteByte((byte)((AlphaValues[baseIndex + read++] >> (layer * 8)) & 0xFF));
                    }
                }
            }
        }

        protected byte[] GetAlphaCompressed(int layer)
        {
            var strm = new MemoryStream();

            for (var i = 0; i < 64; ++i)
                CompressRow(strm, i, layer);

            return strm.ToArray();
        }

        protected byte[] GetAlphaUncompressed(int layer)
        {
            if (WorldFrame.Instance.MapManager.HasNewBlend)
            {
                var ret = new byte[4096];
                for (var i = 0; i < 4096; ++i)
                    ret[i] = (byte)((AlphaValues[i] >> (layer * 8)) & 0xFF);
                return ret;
            }
            else
            {
                var ret = new byte[2048];
                for (var i = 0; i < 2048; ++i)
                {
                    var a1 = (byte)((AlphaValues[i * 2] >> (layer * 8)) & 0xFF);
                    var a2 = (byte)((AlphaValues[i * 2 + 1] >> (layer * 8)) & 0xFF);

                    var v1 = (uint)((a1 / 255.0f) * 15.0f);
                    var v2 = (uint)((a2 / 255.0f) * 15.0f);
                    ret[i] = (byte)((v2 << 4) | v1);
                }

                return ret;
            }
        }
    }
}

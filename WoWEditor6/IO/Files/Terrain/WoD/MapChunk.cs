using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Terrain;

namespace WoWEditor6.IO.Files.Terrain.WoD
{
    class MapChunk
    {
        private readonly ChunkStreamInfo mMainInfo;
        private readonly ChunkStreamInfo mTexInfo;
        private ChunkStreamInfo mObjInfo;

        private readonly BinaryReader mReader;
        private readonly BinaryReader mTexReader;
        private BinaryReader mObjReader;

        private MCNK mHeader;
        private readonly AdtVertex[] mVertices = new AdtVertex[145];

        private byte[] mAlphaDataCompressed;

        private readonly List<MCLY> mLayerInfos = new List<MCLY>();
        private readonly uint[] mAlphaData = new uint[4096];

        public int IndexX { get; }
        public int IndexY { get; }

        public MapChunk(BinaryReader reader, BinaryReader texReader, BinaryReader objReader, 
            ChunkStreamInfo mainInfo, ChunkStreamInfo texInfo, ChunkStreamInfo objInfo, 
            int indexX, int indexY)
        {
            mMainInfo = mainInfo;
            mTexInfo = texInfo;
            mObjInfo = objInfo;

            mReader = reader;
            mTexReader = texReader;
            mObjReader = objReader;

            IndexX = indexX;
            IndexY = indexY;
        }

        public void AsyncLoad()
        {
            mReader.BaseStream.Position = mMainInfo.PosStart;
            var chunkSize = mReader.ReadInt32();
            mHeader = mReader.Read<MCNK>();
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
                    mVertices[i].Color = 0x7F7F7F7F;
            }

            LoadTexData();
        }

        private void LoadTexData()
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
                        mAlphaDataCompressed = mTexReader.ReadBytes(size);
                        break;
                }

                mTexReader.BaseStream.Position = cur + size;
            }

            LoadAlpha();
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
                        mAlphaData[j] |= 0xFFu << (8 * i);
                }
            }

            mAlphaDataCompressed = null;
        }

        private void LoadUncompressed(MCLY layerInfo, int layer)
        {
            var startPos = layerInfo.OfsMcal;
            for (var i = 0; i < 4096; ++i)
                mAlphaData[i] |= (uint) mAlphaDataCompressed[startPos++] << (8 * layer);
        }

        private void LoadLayerCompressed(MCLY layerInfo, int layer)
        {
            var startPos = layerInfo.OfsMcal;
            var counter = 0;
            for (var k = 0; k < 64; ++k)
            {
                for (var j = 0; j < 32; ++j)
                {
                    var alpha = mAlphaDataCompressed[startPos++];
                    var val1 = alpha & 0xF;
                    var val2 = alpha >> 4;
                    val2 = j == 31 ? val1 : val2;
                    val1 = (byte)((val1 / 15.0f) * 255.0f);
                    val2 = (byte)((val2 / 15.0f) * 255.0f);
                    mAlphaData[counter++] |= (uint)val1 << (8 * layer);
                    mAlphaData[counter++] |= (uint)val2 << (8 * layer);
                }
            }
        }

        private void LoadLayerRle(MCLY layerInfo, int layer)
        {
            var counterOut = 0;
            var startPos = layerInfo.OfsMcal;
            while (counterOut < 4096)
            {
                var indicator = mAlphaDataCompressed[startPos++];
                if ((indicator & 0x80) != 0)
                {
                    var value = mAlphaDataCompressed[startPos++];
                    var repeat = indicator & 0x7F;
                    for (var k = 0; k < repeat && counterOut < 4096; ++k)
                        mAlphaData[counterOut++] |= (uint)value << (layer * 8);
                }
                else
                {
                    for (var k = 0; k < (indicator & 0x7F) && counterOut < 4096; ++k)
                        mAlphaData[counterOut++] |= (uint)mAlphaDataCompressed[startPos++] << (8 * layer);
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

            for(var i = 0; i < 17; ++i)
            {
                for(var j = 0; j < (((i % 2) != 0) ? 8 : 9); ++j)
                {
                    mVertices[counter].Position = new Vector3(posx + j * Metrics.UnitSize,
                        posy - i * Metrics.UnitSize * 0.5f, posz + heights[counter]);

                    mVertices[counter].TexCoordAlpha = new Vector2(j / 8.0f, i / 16.0f);
                    mVertices[counter].TexCoord = new Vector2(j, i * 0.5f + ((i % 2) != 0 ? 0.5f : 0.0f));
                    ++counter;
                }
            }
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

                    mVertices[counter].Normal = new Vector3(nx, ny, nz);
                    ++counter;
                }
            }
        }

        private void LoadMccv()
        {
            var colors = mReader.ReadArray<uint>(145);
            for (var i = 0; i < 145; ++i)
                mVertices[i].Color = colors[i];
        }

        private void LoadMcly(int size)
        {
            mLayerInfos.AddRange(mTexReader.ReadArray<MCLY>(size / SizeCache<MCLY>.Size));
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using WoWEditor6.IO;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.IO.Files.Terrain;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Models.M2;
using WoWEditor6.UI;
using WoWEditor6.Utils;
using Point = System.Drawing.Point;

namespace WoWEditor6.Editing
{
    class ModelSpawnManager
    {
        public const int M2InstanceUuid = -1;

        public static ModelSpawnManager Instance { get; private set; }

        private M2RenderInstance mHoveredInstance;
        private M2Instance[] mInstanceRef;
        private string mSelectedModel;
        private Point mLastCursorPos;

        public M2RenderInstance ClickedInstance { get; set; }

        static ModelSpawnManager()
        {
            Instance = new ModelSpawnManager();
        }

        private ModelSpawnManager()
        {
            
        }

        public void CopyClickedModel()
        {
            if (ClickedInstance == null)
                return;

            SelectModel(ClickedInstance.Model.FileName);
        }

        public void SelectModel(string model)
        {
            if (mHoveredInstance != null)
            {
                WorldFrame.Instance.M2Manager.RemoveInstance(mSelectedModel, M2InstanceUuid);
                mHoveredInstance = null;
            }

            var position = Vector3.Zero;

            if (WorldFrame.Instance.LastMouseIntersection != null &&
                WorldFrame.Instance.LastMouseIntersection.TerrainHit)
                position = WorldFrame.Instance.LastMouseIntersection.TerrainPosition;

            mSelectedModel = model;
            mHoveredInstance = WorldFrame.Instance.M2Manager.AddInstance(model, M2InstanceUuid, position, Vector3.Zero, Vector3.One);

            if (mHoveredInstance == null)
            {
                mSelectedModel = null;
                return;
            }

            mInstanceRef = new[]
            {
                new M2Instance
                {
                    BoundingBox = mHoveredInstance.BoundingBox,
                    Hash = mSelectedModel.ToUpperInvariant().GetHashCode(),
                    MddfIndex = -1,
                    RenderInstance = mHoveredInstance,
                    Uuid = M2InstanceUuid
                }
            };

            WorldFrame.Instance.OnWorldClicked += OnTerrainClicked;
        }

        public void OnUpdate()
        {
            var cursor = Cursor.Position;
            if (mHoveredInstance == null)
            {
                mLastCursorPos = cursor;
                return;
            }

            var intersection = WorldFrame.Instance.LastMouseIntersection;
            if (intersection == null || intersection.TerrainHit == false)
            {
                mLastCursorPos = cursor;
                return;
            }

            CheckUpdateScale(cursor);

            mHoveredInstance.UpdatePosition(intersection.TerrainPosition);
            mInstanceRef[0].BoundingBox = mHoveredInstance.BoundingBox;

            WorldFrame.Instance.M2Manager.PushMapReferences(mInstanceRef);

            mLastCursorPos = cursor;
        }

        private void CheckUpdateScale(Point cursor)
        {
            var state = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(state);

            if (KeyHelper.AreKeysDown(state, Keys.Menu, Keys.RButton))
            {
                var dx = cursor.X - mLastCursorPos.X;
                var newScale = mHoveredInstance.Scale + dx * 0.05f;
                newScale = Math.Max(newScale, 0);
                mHoveredInstance.UpdateScale(newScale);
            }
        }

        private void OnTerrainClicked(IntersectionParams parameters, MouseEventArgs args)
        {
            if (args.Button != MouseButtons.Left)
            {
                if (args.Button == MouseButtons.Right)
                {
                    var state = new byte[256];
                    UnsafeNativeMethods.GetKeyboardState(state);
                    if (KeyHelper.IsKeyDown(state, Keys.ControlKey))
                    {
                        WorldFrame.Instance.M2Manager.RemoveInstance(mSelectedModel, M2InstanceUuid);
                        mHoveredInstance = null;
                        mSelectedModel = null;
                        WorldFrame.Instance.OnWorldClicked -= OnTerrainClicked;
                    }
                }

                return;
            }

            if (parameters.TerrainHit == false)
                return;

            if (mHoveredInstance == null)
                return;

            SpawnModel(parameters.TerrainPosition);

            if (EditorWindowController.Instance.SpawnModel.DeselectModelOnClick)
            {
                WorldFrame.Instance.M2Manager.RemoveInstance(mSelectedModel, M2InstanceUuid);
                mHoveredInstance = null;
                mSelectedModel = null;
                WorldFrame.Instance.OnWorldClicked -= OnTerrainClicked;
            }
        }

        private void SpawnModel(Vector3 rootPosition)
        {
            var minPos = mHoveredInstance.BoundingBox.Minimum;
            var maxPos = mHoveredInstance.BoundingBox.Maximum;

            if (FileManager.Instance.Version == FileDataVersion.Warlords)
            {
                minPos.Y -= 64.0f - Metrics.TileSize;
                maxPos.Y -= 64.0f - Metrics.TileSize;
                var tmp = maxPos.Y;
                maxPos.Y = minPos.Y;
                minPos.Y = tmp;
            }

            var adtMinX = (int)(minPos.X / Metrics.TileSize);
            var adtMinY = (int) (minPos.Y / Metrics.TileSize);

            var adtMaxX = (int) (maxPos.X / Metrics.TileSize);
            var adtMaxY = (int) (maxPos.Y / Metrics.TileSize);

            // if the model is only on one adt dont bother checking
            // all adts for the UUID.
            if (adtMinX == adtMaxX && adtMinY == adtMaxY)
            {
                var area = WorldFrame.Instance.MapManager.GetAreaByIndex(adtMinX, adtMinY);
                if (area == null)
                    return;

                if (area.AreaFile == null || area.AreaFile.IsValid == false)
                    return;

                area.AreaFile.AddDoodadInstance(area.AreaFile.GetFreeM2Uuid(), mSelectedModel, mHoveredInstance.BoundingBox,
                    mHoveredInstance.Position,
                    new Vector3(0, 0, 0), mHoveredInstance.Scale);
            }
            else
            {
                AddToGrid(adtMinX, adtMaxX, adtMinY, adtMaxY, (int) (rootPosition.X / Metrics.TileSize),
                    (int) (rootPosition.Y / Metrics.TileSize));
            }
        }

        private void AddToGrid(int adtMinX, int adtMaxX, int adtMinY, int adtMaxY, int clickX, int clickY)
        {
            var area = WorldFrame.Instance.MapManager.GetAreaByIndex(clickX, clickY);
            if (area == null || area.AreaFile == null || area.AreaFile.IsValid == false)
                return;

            var minPos = mHoveredInstance.BoundingBox.Minimum;
            var maxPos = mHoveredInstance.BoundingBox.Maximum;

            var baseUuid = area.AreaFile.GetFreeM2Uuid();
            if (baseUuid == -1)
                return;

            for (var x = adtMinX; x <= adtMaxX; ++x)
            {
                for (var y = adtMinY; y <= adtMaxY; ++y)
                {
                    var testArea = WorldFrame.Instance.MapManager.GetAreaByIndex(x, y);
                    if (testArea != null && testArea.AreaFile != null)
                    {
                        if (testArea.AreaFile.IsValid == false)
                            continue;

                        while (testArea.AreaFile.IsUuidAvailable(baseUuid) == false)
                        {
                            if ((baseUuid & 0xFFFFF) < 0xFFFFF)
                                ++baseUuid;
                            else
                                return;
                        }
                    }
                    else if (CheckUuidForFileArea(x, y, ref baseUuid) == false)
                        return;
                }
            }

            for (var x = adtMinX; x <= adtMaxX; ++x)
            {
                for (var y = adtMinY; y <= adtMaxY; ++y)
                {
                    var testArea = WorldFrame.Instance.MapManager.GetAreaByIndex(x, y);
                    if (testArea != null && testArea.AreaFile != null)
                    {
                        testArea.AreaFile.AddDoodadInstance(baseUuid, mSelectedModel, mHoveredInstance.BoundingBox,
                            mHoveredInstance.Position, mHoveredInstance.Rotation,
                            mHoveredInstance.Scale);
                    }
                    else
                        AddDoodadToFileArea(x, y, baseUuid, mHoveredInstance.BoundingBox);
                }
            }
        }

        private void AddDoodadToFileArea(int x, int y, int uuid, BoundingBox box)
        {
            var area = AdtFactory.Instance.CreateArea(WorldFrame.Instance.MapManager.Continent, x, y);
            try
            {
                area.AsyncLoad();
                area.AddDoodadInstance(uuid, mSelectedModel, box, mHoveredInstance.Position, mHoveredInstance.Rotation, mHoveredInstance.Scale);
                area.Save();
            }
            catch (Exception)
            {
            }
        }

        private static bool CheckUuidForFileArea(int x, int y, ref int uuid)
        {
            switch (FileManager.Instance.Version)
            {
                case FileDataVersion.Lichking:
                    return CheckUuidForFileAreaWotlk(x, y, ref uuid);

                default:
                    return true;
            }
        }

        private static bool CheckUuidForFileAreaWotlk(int x, int y, ref int uuid)
        {
            var fileName = string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", WorldFrame.Instance.MapManager.Continent, x, y);
            using (var strm = FileManager.Instance.Provider.OpenFile(fileName))
            {
                if (strm == null)
                    return true;

                var reader = new BinaryReader(strm);
                while (strm.Position + 8 < strm.Length)
                {
                    var signature = reader.ReadUInt32();
                    var size = reader.ReadInt32();
                    if (signature == Chunks.Mddf)
                    {
                        var doodads = reader.ReadArray<Mddf>(size / SizeCache<Mddf>.Size);
                        var id = uuid;
                        while (doodads.Any(d => d.UniqueId == id))
                        {
                            if ((id & 0xFFFFF) < 0xFFFFF)
                                ++id;
                            else
                                return false;
                        }

                        uuid = id;
                        return true;
                    }
                    else
                        strm.Position += size;
                }

                return true;
            }
        }
    }
}

using System;
using System.Windows.Forms;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Models.M2;
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

        static ModelSpawnManager()
        {
            Instance = new ModelSpawnManager();
        }

        private ModelSpawnManager()
        {
            
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
        }

        public void OnUpdate(TimeSpan diff)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Models;
using WoWEditor6.Utils;
using WoWEditor6.UI;
using Point = System.Drawing.Point;

namespace WoWEditor6.Editing
{
    class ModelEditManager
    {
        public static ModelEditManager Instance { get; private set; }

        public IModelInstance SelectedModel { get; set; }

        private Point mLastCursorPosition = Cursor.Position;
        private Vector3 mLastBrushPosition = EditManager.Instance.MousePosition;
        private Vector3 mLastPos = EditManager.Instance.MousePosition;
        int slowness = 1;

        static ModelEditManager()
        {
            Instance = new ModelEditManager();
        }

        public void Update()
        {
            if (SelectedModel == null)
            {
                mLastCursorPosition = Cursor.Position;
                mLastBrushPosition = EditManager.Instance.MousePosition;
                mLastPos = EditManager.Instance.MousePosition;

                EditorWindowController.Instance.OnUpdate(new Vector3(0.0f,0.0f,0.0f), new Vector3(0.0f, 0.0f, 0.0f));
                return;
            }

            var curPos = Cursor.Position;
            var dpos = new Point(curPos.X - mLastCursorPosition.X, curPos.Y - mLastCursorPosition.Y);

            EditorWindowController.Instance.OnUpdate(SelectedModel.GetPosition(),SelectedModel.GetNamePlatePosition());

            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);
            var altDown = KeyHelper.IsKeyDown(keyState, Keys.Menu);
            var ctrlDown = KeyHelper.IsKeyDown(keyState, Keys.ControlKey);
            var shiftDown = KeyHelper.IsKeyDown(keyState, Keys.ShiftKey);
            var RMBDown = KeyHelper.IsKeyDown(keyState, Keys.RButton);
            var MMBDown = KeyHelper.IsKeyDown(keyState, Keys.MButton);
            var DelDown = KeyHelper.IsKeyDown(keyState, Keys.Delete);
            var rDown = KeyHelper.IsKeyDown(keyState, Keys.R);
            var mDown = KeyHelper.IsKeyDown(keyState, Keys.M);
            var pagedownDown = KeyHelper.IsKeyDown(keyState, Keys.PageDown);


            if ((altDown || ctrlDown || shiftDown) & RMBDown) // Rotating
            {
                var angle = MathUtil.DegreesToRadians(dpos.X * 6);
                SelectedModel.Rotate(altDown ? angle : 0, ctrlDown ? angle : 0, shiftDown ? angle : 0);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            if (altDown & MMBDown & !shiftDown) // Scaling
            {
                var amount = (mLastCursorPosition.Y - curPos.Y) / 512.0f;
                SelectedModel.UpdateScale(altDown ? amount : 0);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            if (MMBDown && !altDown && Cursor.Position != mLastCursorPosition) // Moving 
            {
                Vector3 delta;
                Vector3 position;

                delta.X = EditManager.Instance.MousePosition.X - mLastPos.X;
                delta.Y = EditManager.Instance.MousePosition.Y - mLastPos.Y;
                delta.Z = -(Cursor.Position.Y - mLastCursorPosition.Y); //Better to use the 2d screen pos of the mouse.

                position = new Vector3(MMBDown && !shiftDown ? delta.X/ slowness : 0, MMBDown && !shiftDown ? delta.Y/ slowness : 0, MMBDown && shiftDown ? delta.Z/ slowness : 0);

                SelectedModel.UpdatePosition(position);

                mLastBrushPosition = SelectedModel.GetPosition();
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            mLastCursorPosition = curPos;
            mLastPos = EditManager.Instance.MousePosition;

            if(DelDown) // Delete model
            {
                SelectedModel.Remove();
            }

           if(ctrlDown && rDown) // Reset rotation
            {
                var newRotation = SelectedModel.GetRotation() * (-1);
                SelectedModel.Rotate(newRotation.X, newRotation.Y, newRotation.Z);
                WorldFrame.Instance.UpdateSelectedBoundingBox();

            }

            if (ctrlDown && mDown) // Move to cursor pos.
            {
                Vector3 brushPos = EditManager.Instance.MousePosition;

                var delta = brushPos - SelectedModel.GetPosition();
                SelectedModel.UpdatePosition(delta);
                WorldFrame.Instance.UpdateSelectedBoundingBox();

            }

            if(pagedownDown) // Snap model to ground.
            {
                var curPosition = SelectedModel.GetPosition();
                WorldFrame.Instance.MapManager.GetLandHeight(curPosition.X, curPosition.Y, out curPosition.Z);
                var delta = curPosition - SelectedModel.GetPosition();
                SelectedModel.UpdatePosition(delta);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
        }
    }
}



using System.Windows.Forms;
using SharpDX;
using Neo.Scene;
using Neo.Scene.Models;
using Neo.Scene.Models.M2;
using Neo.Utils;
using Neo.UI;
using Point = System.Drawing.Point;

namespace Neo.Editing
{
    class ModelEditManager
    {
        public static ModelEditManager Instance { get; private set; }

        public IModelInstance SelectedModel { get; set; }

        private Point mLastCursorPosition = Cursor.Position;
        private Vector3 mLastPos = EditManager.Instance.MousePosition;
        private const int Slowness = 1;
        public bool IsCopying { get; set; }

        static ModelEditManager()
        {
            Instance = new ModelEditManager();
        }

        public void Update()
        {
            if (SelectedModel == null)
            {
                IsCopying = false;
                mLastCursorPosition = Cursor.Position;
                mLastPos = EditManager.Instance.MousePosition;

                EditorWindowController.Instance.OnUpdate(Vector3.Zero, Vector3.Zero);
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
            var rmbDown = KeyHelper.IsKeyDown(keyState, Keys.RButton);
            var mmbDown = KeyHelper.IsKeyDown(keyState, Keys.MButton);
            var delDown = KeyHelper.IsKeyDown(keyState, Keys.Delete);
            var rDown = KeyHelper.IsKeyDown(keyState, Keys.R);
            var mDown = KeyHelper.IsKeyDown(keyState, Keys.M);
            var vDown = KeyHelper.IsKeyDown(keyState, Keys.V);
            var pagedownDown = KeyHelper.IsKeyDown(keyState, Keys.PageDown);

            if (ctrlDown && vDown) // Pasting
            {
                if (IsCopying == false)
                {
                    IsCopying = true;
                    Editing.ModelSpawnManager.Instance.CopyClickedModel();
                }
            }

            if ((altDown || ctrlDown || shiftDown) & rmbDown) // Rotating
            {
                var angle = MathUtil.DegreesToRadians(dpos.X * 6);
                SelectedModel.Rotate(altDown ? angle : 0, ctrlDown ? angle : 0, shiftDown ? angle : 0);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            if (altDown & mmbDown & !shiftDown) // Scaling
            {
                var amount = (mLastCursorPosition.Y - curPos.Y) / 512.0f;
                SelectedModel.UpdateScale(amount);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            if (mmbDown && !altDown && Cursor.Position != mLastCursorPosition) // Moving 
            {
                Vector3 delta;

                delta.X = EditManager.Instance.MousePosition.X - mLastPos.X;
                delta.Y = EditManager.Instance.MousePosition.Y - mLastPos.Y;
                delta.Z = -(Cursor.Position.Y - mLastCursorPosition.Y); //Better to use the 2d screen pos of the mouse.

                var position = new Vector3(!shiftDown ? delta.X/ Slowness : 0, !shiftDown ? delta.Y/ Slowness : 0, shiftDown ? delta.Z/ Slowness : 0);

                SelectedModel.UpdatePosition(position);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            mLastCursorPosition = curPos;
            mLastPos = EditManager.Instance.MousePosition;

            if(delDown) // Delete model
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



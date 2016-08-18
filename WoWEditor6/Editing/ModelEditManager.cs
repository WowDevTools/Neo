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
        private Vector3 mLastBrushPosition = Editing.EditManager.Instance.MousePosition;
        private Vector3 mLastPos = Editing.EditManager.Instance.MousePosition;
        static ModelEditManager()
        {
            Instance = new ModelEditManager();
        }

        public void Update()
        {
            if (SelectedModel == null)
            {
                mLastCursorPosition = Cursor.Position;
                mLastBrushPosition = Editing.EditManager.Instance.MousePosition;
                mLastPos = Editing.EditManager.Instance.MousePosition;

                EditorWindowController.Instance.OnMouseMove(new Vector3(0.0f,0.0f,0.0f));
                return;
            }

            var curPos = Cursor.Position;
            var dpos = new Point(curPos.X - mLastCursorPosition.X, curPos.Y - mLastCursorPosition.Y);

            EditorWindowController.Instance.OnMouseMove(SelectedModel.GetPosition());

            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);
            var altDown = KeyHelper.IsKeyDown(keyState, Keys.Menu);
            var ctrlDown = KeyHelper.IsKeyDown(keyState, Keys.ControlKey);
            var shiftDown = KeyHelper.IsKeyDown(keyState, Keys.ShiftKey);
            var RMBDown = KeyHelper.IsKeyDown(keyState, Keys.RButton);
            var MMBDown = KeyHelper.IsKeyDown(keyState, Keys.MButton);

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
                Vector2 delta;
                Vector3 position;

                delta.X = Editing.EditManager.Instance.MousePosition.X - mLastPos.X;
                delta.Y = Editing.EditManager.Instance.MousePosition.Y - mLastPos.Y;

                position = new Vector3(MMBDown && !shiftDown ? delta.X / 32.0f : 0, MMBDown && !shiftDown ? delta.Y / 32.0f : 0, MMBDown && shiftDown ? delta.Y / 32.0f : 0);

                SelectedModel.UpdatePosition(position);

                mLastBrushPosition = SelectedModel.GetPosition();

                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            mLastCursorPosition = curPos;
            mLastPos = Editing.EditManager.Instance.MousePosition;
        }

        public long GCD(long a, long b)
        {
            long Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

    }
}



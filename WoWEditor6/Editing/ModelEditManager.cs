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
using Point = System.Drawing.Point;

namespace WoWEditor6.Editing
{
    class ModelEditManager
    {
        public static ModelEditManager Instance { get; private set; }

        public IModelInstance SelectedModel { get; set; }

        private Point mLastCursorPosition = Cursor.Position;
        private Vector3 mLastBrushPosition = Editing.EditManager.Instance.MousePosition;

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
                return;
            }

            var curPos = Cursor.Position;
            var dpos = new Point(curPos.X - mLastCursorPosition.X, curPos.Y - mLastCursorPosition.Y);

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
            if (MMBDown && !altDown) // Moving 
            {
                Vector2 delta;
                Vector3 position;

                delta.X = -(mLastBrushPosition.X - Editing.EditManager.Instance.MousePosition.X);
                delta.Y = -(mLastBrushPosition.Y - Editing.EditManager.Instance.MousePosition.Y);

                float x = delta.X;
                float y = delta.Y;
                int xSign = 1;
                int ySign = 1;

                if (x < 0)
                {
                    xSign = -1; //We save the fact that x was <0
                    x *= -1; // We transform x in pos value
                }

                if (y < 0)
                {
                    ySign = -1; //We save the fact that y was <0
                    y *= -1; // We transform y in pos value
                }

                int xInteger = (int)(x * 1000000); //We transform x in an integer ( 7decimal prec)
                int yInteger = (int)(y * 1000000); //We transform y in an integer ( 7decimal prec)

                float gcd = GCD(xInteger, yInteger); //GCD Between x/y

                gcd /= 1000000;

                x = (float)(xInteger / gcd) / 1000000; //Then we re-go in float
                y = (float)(yInteger / gcd) / 1000000; //Then we re-go in float

                x *= xSign; //Finally, re-put the sign
                y *= ySign; //Finally, re-put the sign

                position = new Vector3(MMBDown && !shiftDown ? x / 64.0f : 0, MMBDown && !shiftDown ? y / 64.0f : 0, MMBDown && shiftDown ? y / 64.0f : 0);

                SelectedModel.UpdatePosition(position);

                mLastBrushPosition = SelectedModel.GetPosition();

                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }

            mLastCursorPosition = curPos;
        }

        long GCD(long a, long b)
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



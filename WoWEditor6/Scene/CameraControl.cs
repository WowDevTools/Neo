using System;
using System.Drawing;
using System.Windows.Forms;
using WoWEditor6.Settings;
using WoWEditor6.UI;
using WoWEditor6.Utils;

namespace WoWEditor6.Scene
{
    class CameraControl
    {
        private readonly MainWindow mWindow;
        private Point mLastCursorPos;
        private DateTime mLastUpdate = DateTime.Now;

        private float speedFactor = 100.0f;
        private float speedFactorWheel = 0.5f;
        private float turnFactor = 0.2f;
        public bool InvertX { get; set; }
		public bool InvertY { get; set; }

        public CameraControl(MainWindow window)
        {
            mWindow = window;
        }

        public void Update()
        {
            if (mWindow.Focused == false || WorldFrame.Instance.State != AppState.World)
            {
                mLastCursorPos = Cursor.Position;
                mLastUpdate = DateTime.Now;
                return;
            }

            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);

            var positionChanged = false;
            var updateTerrain = false;
            var cam = WorldFrame.Instance.ActiveCamera;
            var diff = (float)(DateTime.Now - mLastUpdate).TotalSeconds;

            var camBind = KeyBindings.Instance.Camera;

            if (KeyHelper.AreKeysDown(keyState, camBind.Forward))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(diff * speedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Backward))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(-diff * speedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Right))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(diff * speedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Left))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(-diff * speedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Up))
            {
                positionChanged = true;
                cam.MoveUp(diff * speedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Down))
            {
                positionChanged = true;
                cam.MoveUp(-diff * speedFactor);
            }

            if (KeyHelper.IsKeyDown(keyState, Keys.RButton))
            {
                var curPos = Cursor.Position;
                var dx = curPos.X - mLastCursorPos.X;
                var dy = curPos.Y - mLastCursorPos.Y;

                if (dx != 0)
                    cam.Yaw(dx * turnFactor * (InvertX ? -1 : 1));
                if (dy != 0)
                    cam.Pitch(dy * turnFactor * (InvertY ? -1 : 1));
            }

            if (positionChanged)
                WorldFrame.Instance.MapManager.UpdatePosition(cam.Position, updateTerrain);

            mLastUpdate = DateTime.Now;
            mLastCursorPos = Cursor.Position;
        }

        public void HandleMouseWheel(int delta)
        {
            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);

            if (KeyHelper.IsKeyDown(keyState, Keys.RButton))
            {
                var cam = WorldFrame.Instance.ActiveCamera;
                cam.MoveForward(delta * speedFactorWheel);
                WorldFrame.Instance.MapManager.UpdatePosition(cam.Position, true);
            }
        }
    }
}
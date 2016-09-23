using System;
using Neo.Settings;
using Neo.Utils;
using OpenTK;
using Point = System.Drawing.Point;

namespace Neo.Scene
{
    class CameraControl
    {
        public delegate void PositionChangedHandler(Vector3 newPosition, bool updateTerrain);

        private readonly Control mWindow;
        private Point mLastCursorPos;
        private DateTime mLastUpdate = DateTime.Now;

        private float mSpeedFactor = 100.0f;
        private float mSpeedFactorWheel = 0.5f;
        public bool InvertX { get; set; }
        public bool InvertY { get; set; }

        public float TurnFactor { get; set; }

        public float SpeedFactor
        {
            get { return mSpeedFactor; }
            set { mSpeedFactor = value; }
        }

        public float SpeedFactorWheel
        {
            get { return mSpeedFactorWheel; }
            set { mSpeedFactorWheel = value; }
        }

        public event PositionChangedHandler PositionChanged;

        public CameraControl(Control window)
        {
            TurnFactor = 0.2f;
            mWindow = window;
        }

        public void ForceUpdate(Vector3 position)
        {
            if (PositionChanged != null) PositionChanged.Invoke(position, true);
        }

        public void Update(Camera cam, bool stateOnly)
        {
            if (mWindow.Focused == false || stateOnly)
            {
                mLastCursorPos = InterfaceHelper.GetCursorPosition();
                mLastUpdate = DateTime.Now;
                return;
            }

            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);

            var positionChanged = false;
            var updateTerrain = false;
            var diff = (float)(DateTime.Now - mLastUpdate).TotalSeconds;

            var camBind = KeyBindings.Instance.CameraKeys;

            if (KeyHelper.AreKeysDown(keyState, camBind.Forward))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(diff * mSpeedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Backward))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(-diff * mSpeedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Right))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(diff * mSpeedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Left))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(-diff * mSpeedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Up))
            {
                positionChanged = true;
                cam.MoveUp(diff * mSpeedFactor);
            }

            if (KeyHelper.AreKeysDown(keyState, camBind.Down))
            {
                positionChanged = true;
                cam.MoveUp(-diff * mSpeedFactor);
            }

            if (KeyHelper.IsKeyDown(keyState, Keys.RButton) && !KeyHelper.IsKeyDown(keyState, Keys.ControlKey) && !KeyHelper.IsKeyDown(keyState, Keys.Menu) && !KeyHelper.IsKeyDown(keyState, Keys.ShiftKey))
            {
                var curPos = InterfaceHelper.GetCursorPosition();
                var dx = curPos.X - mLastCursorPos.X;
                var dy = curPos.Y - mLastCursorPos.Y;

                if (dx != 0)
                    cam.Yaw(dx * TurnFactor * (InvertX ? 1 : -1));

                if (dy != 0)
                    cam.Pitch(dy * TurnFactor * (InvertY ? 1 : -1));
            }

            if (positionChanged && PositionChanged != null)
                PositionChanged(cam.Position, updateTerrain);

            mLastUpdate = DateTime.Now;
            mLastCursorPos = InterfaceHelper.GetCursorPosition();
        }

        public void HandleMouseWheel(int delta)
        {
            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);

            if (KeyHelper.IsKeyDown(keyState, Key.RButton))
            {
                var cam = WorldFrame.Instance.ActiveCamera;
                cam.MoveForward(delta * mSpeedFactorWheel);
                WorldFrame.Instance.MapManager.UpdatePosition(cam.Position, true);
            }
        }
    }
}
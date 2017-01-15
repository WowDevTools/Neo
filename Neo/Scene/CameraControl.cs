using System;
using Neo.Settings;
using Neo.Utils;
using OpenTK;
using OpenTK.Input;
using Point = System.Drawing.Point;

namespace Neo.Scene
{
	internal class CameraControl
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
            get { return this.mSpeedFactor; }
            set { this.mSpeedFactor = value; }
        }

        public float SpeedFactorWheel
        {
            get { return this.mSpeedFactorWheel; }
            set { this.mSpeedFactorWheel = value; }
        }

        public event PositionChangedHandler PositionChanged;

        public CameraControl(Control window)
        {
	        this.TurnFactor = 0.2f;
	        this.mWindow = window;
        }

        public void ForceUpdate(Vector3 position)
        {
            if (PositionChanged != null)
            {
	            PositionChanged.Invoke(position, true);
            }
        }

        public void Update(Camera cam, bool stateOnly)
        {
            if (this.mWindow.Focused == false || stateOnly)
            {
	            this.mLastCursorPos = InterfaceHelper.GetCursorPosition();
	            this.mLastUpdate = DateTime.Now;
                return;
            }

            var positionChanged = false;
            var updateTerrain = false;
            var diff = (float)(DateTime.Now - this.mLastUpdate).TotalSeconds;

            var camBind = KeyBindings.Instance.CameraKeys;

            if (InputHelper.AreKeysDown(camBind.Forward))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(diff * this.mSpeedFactor);
            }

            if (InputHelper.AreKeysDown(camBind.Backward))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(-diff * this.mSpeedFactor);
            }

            if (InputHelper.AreKeysDown(camBind.Right))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(diff * this.mSpeedFactor);
            }

            if (InputHelper.AreKeysDown(camBind.Left))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(-diff * this.mSpeedFactor);
            }

            if (InputHelper.AreKeysDown(camBind.Up))
            {
                positionChanged = true;
                cam.MoveUp(diff * this.mSpeedFactor);
            }

            if (InputHelper.AreKeysDown(camBind.Down))
            {
                positionChanged = true;
                cam.MoveUp(-diff * this.mSpeedFactor);
            }

	        KeyboardState keyboardState = Keyboard.GetState();
	        MouseState mouseState = Mouse.GetState();
            if (mouseState.IsButtonDown(MouseButton.Right) &&
                !keyboardState.IsKeyDown(Key.ControlLeft) &&
                !keyboardState.IsKeyDown(Key.Menu) &&
                !keyboardState.IsKeyDown(Key.ShiftLeft))
            {
                var curPos = InterfaceHelper.GetCursorPosition();
                var dx = curPos.X - this.mLastCursorPos.X;
                var dy = curPos.Y - this.mLastCursorPos.Y;

	            if (dx != 0)
	            {
		            cam.Yaw(dx * this.TurnFactor * (this.InvertX ? 1 : -1));
	            }

	            if (dy != 0)
	            {
		            cam.Pitch(dy * this.TurnFactor * (this.InvertY ? 1 : -1));
	            }
            }

            if (positionChanged && PositionChanged != null)
            {
	            PositionChanged(cam.Position, updateTerrain);
            }

	        this.mLastUpdate = DateTime.Now;
	        this.mLastCursorPos = InterfaceHelper.GetCursorPosition();
        }

        public void HandleMouseWheel(int delta)
        {
	        MouseState mouseState = Mouse.GetState();
	        if (mouseState.IsButtonDown(MouseButton.Right))
	        {
                var cam = WorldFrame.Instance.ActiveCamera;
                cam.MoveForward(delta * this.mSpeedFactorWheel);
                WorldFrame.Instance.MapManager.UpdatePosition(cam.Position, true);
            }
        }
    }
}
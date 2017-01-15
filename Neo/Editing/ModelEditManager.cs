using Neo.Scene;
using Neo.Scene.Models;
using Neo.Utils;
using Neo.UI;
using OpenTK;
using OpenTK.Input;
using Point = System.Drawing.Point;

namespace Neo.Editing
{
	internal class ModelEditManager
    {
        public static ModelEditManager Instance { get; private set; }

        public IModelInstance SelectedModel { get; set; }

	    private Point mLastCursorPosition = InterfaceHelper.GetCursorPosition();
        private Vector3 mLastPos = EditManager.Instance.MousePosition;
        private const int Slowness = 1;
        public bool IsCopying { get; set; }

        static ModelEditManager()
        {
            Instance = new ModelEditManager();
        }

        public void Update()
        {
            if (this.SelectedModel == null)
            {
	            this.IsCopying = false;
	            this.mLastCursorPosition = InterfaceHelper.GetCursorPosition();
	            this.mLastPos = EditManager.Instance.MousePosition;

                EditorWindowController.Instance.OnUpdate(Vector3.Zero, Vector3.Zero);
                return;
            }

            var curPos = InterfaceHelper.GetCursorPosition();
	        var dpos = new Point(curPos.X - this.mLastCursorPosition.X, curPos.Y - this.mLastCursorPosition.Y);

            EditorWindowController.Instance.OnUpdate(this.SelectedModel.GetPosition(), this.SelectedModel.GetNamePlatePosition());


	        KeyboardState keyboardState = Keyboard.GetState();
	        var altDown = keyboardState.IsKeyDown(Key.AltLeft);
	        var ctrlDown = keyboardState.IsKeyDown(Key.ControlLeft);
	        var shiftDown = keyboardState.IsKeyDown(Key.ShiftLeft);

	        MouseState mouseState = Mouse.GetState();
	        var rmbDown = mouseState.IsButtonDown(MouseButton.Right);
            var mmbDown = mouseState.IsButtonDown(MouseButton.Middle);

	        var delDown = keyboardState.IsKeyDown(Key.Delete);
	        var rDown = keyboardState.IsKeyDown(Key.R);
	        var mDown = keyboardState.IsKeyDown(Key.M);
	        var vDown = keyboardState.IsKeyDown(Key.V);
	        var cDown = keyboardState.IsKeyDown(Key.C);
	        var pagedownDown = keyboardState.IsKeyDown(Key.PageDown);

	        if (ctrlDown && cDown) // Copying
            {
                ModelSpawnManager.Instance.CopyClickedModel();
	            this.IsCopying = ModelSpawnManager.Instance.ClickedInstance != null;
            }

            if (ctrlDown && vDown) // Pasting
            {
	            if (this.IsCopying)
	            {
		            // INVESTIGATE: Possible introduction of new bug (removed explicit passing of mouse state
		            ModelSpawnManager.Instance.OnTerrainClicked(WorldFrame.Instance.LastMouseIntersection);
	            }
            }

            if ((altDown || ctrlDown || shiftDown) & rmbDown) // Rotating
            {
                var angle = MathHelper.DegreesToRadians(dpos.X * 6);
	            this.SelectedModel.Rotate(altDown ? angle : 0, ctrlDown ? angle : 0, shiftDown ? angle : 0);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            if (altDown & mmbDown & !shiftDown) // Scaling
            {
                var amount = (this.mLastCursorPosition.Y - curPos.Y) / 512.0f;
	            this.SelectedModel.UpdateScale(amount);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
            if (mmbDown && !altDown && InterfaceHelper.GetCursorPosition() != this.mLastCursorPosition) // Moving
            {
                Vector3 delta;

                delta.X = EditManager.Instance.MousePosition.X - this.mLastPos.X;
                delta.Y = EditManager.Instance.MousePosition.Y - this.mLastPos.Y;
                delta.Z = -(InterfaceHelper.GetCursorPosition().Y - this.mLastCursorPosition.Y); //Better to use the 2d screen pos of the mouse.

                var position = new Vector3(!shiftDown ? delta.X/ Slowness : 0, !shiftDown ? delta.Y/ Slowness : 0, shiftDown ? delta.Z/ Slowness : 0);

	            this.SelectedModel.SetPosition(position);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }

	        this.mLastCursorPosition = curPos;
	        this.mLastPos = EditManager.Instance.MousePosition;

            if(delDown) // Delete model
            {
	            this.SelectedModel.Remove();
            }

            if(ctrlDown && rDown) // Reset rotation
            {
                var newRotation = this.SelectedModel.GetRotation() * -1;
	            this.SelectedModel.Rotate(newRotation.X, newRotation.Y, newRotation.Z);
                WorldFrame.Instance.UpdateSelectedBoundingBox();

            }

            if (ctrlDown && mDown) // Move to cursor pos.
            {
	            this.SelectedModel.SetPosition(EditManager.Instance.MousePosition);
                WorldFrame.Instance.UpdateSelectedBoundingBox();

            }

            if(pagedownDown) // Snap model to ground.
            {
                var curPosition = this.SelectedModel.GetPosition();
                WorldFrame.Instance.MapManager.GetLandHeight(curPosition.X, curPosition.Y, out curPosition.Z);
	            var delta = curPosition - this.SelectedModel.GetPosition();
	            this.SelectedModel.SetPosition(delta);
                WorldFrame.Instance.UpdateSelectedBoundingBox();
            }
        }
    }
}



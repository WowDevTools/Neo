using System;
using Neo.Scene;
using Neo.Utils;
using Neo.UI.Dialogs;
using OpenTK;
using OpenTK.Input;

namespace Neo.Editing
{
	public enum TerrainChangeType
    {
        Elevate,
        Flatten,
        Blur,
        Shading
    }

	public enum TerrainAlgorithm
    {
        Flat,
        Linear,
        Quadratic,
        Trigonometric
    }

	public class TerrainChangeParameters
    {
        public TerrainChangeType Method;
        public TerrainAlgorithm Algorithm;
        public Vector3 Center;
        public float OuterRadius;
        public float InnerRadius;
        public TimeSpan TimeDiff;
        public Vector3 Shading;
        public float Amount;
        public bool Inverted;
        public bool AlignModels;
    }

	public class TerrainChangeManager
    {
        public static TerrainChangeManager Instance { get; private set; }

        public TerrainChangeType ChangeType { get; set; }
        public TerrainAlgorithm ChangeAlgorithm { get; set; }
        public Vector3 ShadingMultiplier { get; set; }
        public float Amount { get; set; }
        public bool AlignModelsToGround { get; set; }

        static TerrainChangeManager()
        {
            Instance = new TerrainChangeManager();
        }

        public TerrainChangeManager()
        {
	        this.ChangeType = TerrainChangeType.Elevate;
	        this.ChangeAlgorithm = TerrainAlgorithm.Linear;
	        this.ShadingMultiplier = Vector3.One;
	        this.Amount = 15.0f;
	        this.AlignModelsToGround = false;
        }

        public void OnChange(TimeSpan diff)
        {
            bool inverted;
            if (CheckRequirements(out inverted) == false)
            {
	            return;
            }

	        var parameters = new TerrainChangeParameters() {
                Algorithm = this.ChangeAlgorithm,
                Center = EditManager.Instance.MousePosition,
                InnerRadius = EditManager.Instance.InnerRadius,
                OuterRadius = EditManager.Instance.OuterRadius,
                Method = EditManager.Instance.ChangeType,
                TimeDiff = diff,
                Shading = this.ShadingMultiplier,
                // if tablet is connected override the amount set in thee menus
                Amount = this.Amount,
                Inverted = inverted,
                AlignModels = this.AlignModelsToGround
            };

            WorldFrame.Instance.MapManager.OnEditTerrain(parameters);
        }

        private bool CheckRequirements(out bool isInverted)
        {
            isInverted = false;
            if (EditManager.Instance.IsTerrainHovered == false)
            {
	            return false;
            }

	        var bindings = Settings.KeyBindings.Instance;

	        MouseState mouseState = Mouse.GetState();
            if (!mouseState.IsButtonDown(MouseButton.Left))
            {
                return false;
            }

	        if (!InputHelper.AreKeysDown(bindings.InteractionKeys.Edit) &&
	            !InputHelper.AreKeysDown(bindings.InteractionKeys.EditInverse))
	        {
		        return false;
	        }

	        if (InputHelper.AreKeysDown(bindings.InteractionKeys.EditInverse))
	        {
		        isInverted = true;
	        }

            return true;
        }
    }
}

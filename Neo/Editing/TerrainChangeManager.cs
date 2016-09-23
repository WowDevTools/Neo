using System;
using Neo.Scene;
using Neo.Utils;
using Neo.UI.Dialogs;
using OpenTK;

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
            ChangeType = TerrainChangeType.Elevate;
            ChangeAlgorithm = TerrainAlgorithm.Linear;
            ShadingMultiplier = Vector3.One;
            Amount = 15.0f;
            AlignModelsToGround = false;
        }

        public void OnChange(TimeSpan diff)
        {
            bool inverted;
            if (CheckRequirements(out inverted) == false)
                return;

            var parameters = new TerrainChangeParameters() {
                Algorithm = ChangeAlgorithm,
                Center = EditManager.Instance.MousePosition,
                InnerRadius = EditManager.Instance.InnerRadius,
                OuterRadius = EditManager.Instance.OuterRadius,
                Method = EditManager.Instance.ChangeType,
                TimeDiff = diff,
                Shading = ShadingMultiplier,
                // if tablet is connected override the amount set in thee menus
                Amount = Amount,
                Inverted = inverted,
                AlignModels = AlignModelsToGround
            };

            WorldFrame.Instance.MapManager.OnEditTerrain(parameters);
        }

        private bool CheckRequirements(out bool isInverted)
        {
            isInverted = false;
            if (EditManager.Instance.IsTerrainHovered == false)
                return false;

            var bindings = Settings.KeyBindings.Instance;

	        // TODO: Replace with GDK keyboard monitoring
            var state = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(state);

            if (!KeyHelper.IsKeyDown(state, Key.LButton))
            {
                return false;
            }

                if (KeyHelper.AreKeysDown(state, bindings.InteractionKeys.Edit) == false &&
                    KeyHelper.AreKeysDown(state, bindings.InteractionKeys.EditInverse) == false)
                    return false;

                if (KeyHelper.AreKeysDown(state, bindings.InteractionKeys.EditInverse))
                    isInverted = true;

            return true;
        }
    }
}

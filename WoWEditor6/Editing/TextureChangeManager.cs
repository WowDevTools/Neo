using System;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.Utils;

namespace WoWEditor6.Editing
{
    class TextureChangeParameters
    {
        public Vector3 Center;
        public string Texture;
        public float InnerRadius;
        public float OuterRadius;
        public float Amount;
    }

    class TextureChangeManager
    {
        public static TextureChangeManager Instance { get; private set; }
        
        public float Amount { get; set; }
        public string SelectedTexture { get; set; }

        static TextureChangeManager()
        {
            Instance = new TextureChangeManager();
        }

        public void OnChange(TimeSpan diff)
        {
            bool inverted;
            if (CheckRequirements(out inverted) == false)
                return;

            var parameters = new TextureChangeParameters
            {
                Center = EditManager.Instance.MousePosition,
                InnerRadius = EditManager.Instance.InnerRadius,
                OuterRadius = EditManager.Instance.OuterRadius,
                Texture = SelectedTexture,
                Amount = Amount
            };

            WorldFrame.Instance.MapManager.OnTextureTerrain(parameters);
        }

        private bool CheckRequirements(out bool isInverted)
        {
            isInverted = false;
            if (EditManager.Instance.IsTerrainHovered == false)
                return false;

            var bindings = Settings.KeyBindings.Instance;
            var state = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(state);

            if (KeyHelper.AreKeysDown(state, bindings.Interaction.Edit) == false &&
                KeyHelper.AreKeysDown(state, bindings.Interaction.EditInverse) == false)
                return false;

            if (KeyHelper.AreKeysDown(state, bindings.Interaction.EditInverse))
                isInverted = true;

            return true;
        }
    }
}

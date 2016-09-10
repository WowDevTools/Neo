using System;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.Utils;

namespace WoWEditor6.Editing
{
    enum TextureFalloffMode
    {
        Flat,
        Linear,
        Trigonometric
    }

    class TextureChangeParameters
    {
        public Vector3 Center;
        public string Texture;
        public float InnerRadius;
        public float OuterRadius;
        public float Amount;
        public float TargetValue;
        public TextureFalloffMode FalloffMode;
        public bool IsInverted;
    }

    class TextureChangeManager
    {
        public static TextureChangeManager Instance { get; private set; }
        
        public float Amount { get; set; }
        public float TargetValue { get; set; }
        public string SelectedTexture { get; set; }

        public TextureFalloffMode FalloffMode { get; set; }

        static TextureChangeManager()
        {
            Instance = new TextureChangeManager();
        }

        private TextureChangeManager()
        {
            FalloffMode = TextureFalloffMode.Linear;
            Amount = 0.0f;
            TargetValue = 255.0f;
            SelectedTexture = "TILESET\\GENERIC\\black.blp";
        }

        public void OnChange(TimeSpan diff)
        {
            bool inverted;
            if (CheckRequirements(out inverted) == false)
                return;

            if (EditManager.Instance.IsSprayOn) // Spray Texturing.
            {
                if (EditManager.Instance.IsSpraySolidInnerRadius)
                {
                    var parameters = new TextureChangeParameters
                    {
                        Center = EditManager.Instance.MousePosition,
                        InnerRadius = EditManager.Instance.InnerRadius * (EditManager.Instance.InnerRadius / EditManager.Instance.OuterRadius),
                        OuterRadius = EditManager.Instance.InnerRadius,
                        Texture = SelectedTexture,
                        Amount = Amount / 40,
                        FalloffMode = FalloffMode,
                        TargetValue = TargetValue,
                        IsInverted = inverted
                    };

                    WorldFrame.Instance.MapManager.OnTextureTerrain(parameters);
                }

                for (var i = 0; i < EditManager.Instance.SprayParticleAmount; ++i)
                {
                    var r = new Random();
                    Vector3 rCenter;

                        rCenter.X =(float)r.NextDouble(EditManager.Instance.MousePosition.X - EditManager.Instance.OuterRadius,
                                EditManager.Instance.OuterRadius + EditManager.Instance.MousePosition.X);

                        rCenter.Y =(float)r.NextDouble(EditManager.Instance.MousePosition.Y - EditManager.Instance.OuterRadius,
                                EditManager.Instance.OuterRadius + EditManager.Instance.MousePosition.Y);

                        rCenter.Z = EditManager.Instance.MousePosition.Z;


                    var parameters = new TextureChangeParameters
                    {
                        Center = rCenter,
                        InnerRadius = EditManager.Instance.SprayParticleSize,
                        OuterRadius = EditManager.Instance.SprayParticleSize,
                        Texture = SelectedTexture,
                        Amount = Amount / 20,
                        FalloffMode = FalloffMode,
                        TargetValue = TargetValue,
                        IsInverted = inverted
                    };

                    WorldFrame.Instance.MapManager.OnTextureTerrain(parameters);
                }
            }

            else // Normal texturing.
            {
                var parameters = new TextureChangeParameters
                {
                    Center = EditManager.Instance.MousePosition,
                    InnerRadius = EditManager.Instance.InnerRadius,
                    OuterRadius = EditManager.Instance.OuterRadius,
                    Texture = SelectedTexture,
                    //Amount = 4 + Amount,
                    // if tablet is connected override the amount set in thee menus
                    Amount = Amount / 40,
                    FalloffMode = FalloffMode,
                    TargetValue = TargetValue,
                    IsInverted = inverted
                };

                WorldFrame.Instance.MapManager.OnTextureTerrain(parameters);

            }


        }

        private bool CheckRequirements(out bool isInverted)
        {
            isInverted = false;

            if (EditManager.Instance.IsTerrainHovered == false)
                return false;

            var bindings = Settings.KeyBindings.Instance;
            var state = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(state);

            if (!KeyHelper.IsKeyDown(state, System.Windows.Forms.Keys.LButton))
            {
                return false;
            }

            if (KeyHelper.AreKeysDown(state, bindings.Interaction.Edit) == false &&
                KeyHelper.AreKeysDown(state, bindings.Interaction.EditInverse) == false)
                return false;

            if (KeyHelper.AreKeysDown(state, bindings.Interaction.EditInverse))
                isInverted = true;

            return true;
        }
    }
}

using System;
using System.Web.UI.Design.WebControls.WebParts;
using System.Windows.Controls;
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

        public Random r;

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
            r = new Random();
        }

        public void OnChange(TimeSpan diff)
        {
            bool inverted;
            if (CheckRequirements(out inverted) == false)
                return;

            var curPos = EditManager.Instance.MousePosition;
            var innerRadius = EditManager.Instance.InnerRadius;
            var outerRadius = EditManager.Instance.OuterRadius;

            if (EditManager.Instance.IsSprayOn) // Spray Texturing.
            {

                if (EditManager.Instance.IsSpraySolidInnerRadius) // Paint solid inner radius if required.
                {
                    var parameters = new TextureChangeParameters
                    {
                        Center = curPos,
                        InnerRadius = innerRadius * (innerRadius / outerRadius),
                        OuterRadius = innerRadius,
                        Texture = SelectedTexture,
                        Amount = Amount / 40,
                        FalloffMode = FalloffMode,
                        TargetValue = TargetValue,
                        IsInverted = inverted
                    };

                    WorldFrame.Instance.MapManager.OnTextureTerrain(parameters);
                }

                // The spray itself.

                var sprayParticleSize = EditManager.Instance.SprayParticleSize * Metrics.ChunkSize / 2.0f;
                var inc = (float)r.NextDouble(sprayParticleSize / 4.0f, sprayParticleSize / 3.0f);

                for (var py = curPos.Y - outerRadius; py < curPos.Y + outerRadius; py += inc)
                {
                    for (var px = curPos.X - outerRadius; px < curPos.X + outerRadius; px += inc)
                    {

                        if ((Math.Sqrt(Math.Pow(py - curPos.Y, 2) + Math.Pow(px - curPos.X, 2)) <= outerRadius) &&
                            (r.Next(0, 110) < EditManager.Instance.SprayParticleAmount))
                        {
                            var parameters = new TextureChangeParameters
                            {
                                Center = new Vector3(px, py, EditManager.Instance.MousePosition.Z),
                                InnerRadius = sprayParticleSize / 20.0f,
                                OuterRadius = sprayParticleSize / 20.0f,
                                Texture = SelectedTexture,
                                Amount = Amount / 20,
                                FalloffMode = FalloffMode,
                                TargetValue = TargetValue,
                                IsInverted = inverted
                            };

                            WorldFrame.Instance.MapManager.OnTextureTerrain(parameters);
                        }
                    }
                }


            }

            else // Normal texturing.
            {
                var parameters = new TextureChangeParameters
                {
                    Center = curPos,
                    InnerRadius = innerRadius,
                    OuterRadius = outerRadius,
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

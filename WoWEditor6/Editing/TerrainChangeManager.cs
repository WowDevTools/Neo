using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.Utils;

namespace WoWEditor6.Editing
{
    enum TerrainChangeType
    {
        Elevate,
        Flatten,
        Blur,
        Shading
    }

    enum TerrainAlgorithm
    {
        Flat,
        Linear,
        Quadratic,
        Trigonometric
    }

    class TerrainChangeParameters
    {
        public TerrainChangeType Method;
        public TerrainAlgorithm Algorithm;
        public Vector3 Center;
        public float OuterRadius;
        public float InnerRadius;
        public TimeSpan TimeDiff;
    }

    class TerrainChangeManager
    {
        public static TerrainChangeManager Instance { get; } = new TerrainChangeManager();

        private float mInnerRadius = 45.0f;
        private float mOuterRadius = 55.0f;

        public TerrainChangeType ChangeType { get; set; } = TerrainChangeType.Elevate;
        public TerrainAlgorithm ChangeAlgorithm { get; set; } = TerrainAlgorithm.Linear;
        public Vector3 MousePosition { get; set; }
        public bool IsTerrainHovered { get; set; }

        public float InnerRadius
        {
            get { return mInnerRadius; }
            set
            {
                mInnerRadius = value;
                WorldFrame.Instance.UpdateTerrainBrush(mInnerRadius, mOuterRadius);
            }
        }

        public float OuterRadius
        {
            get { return mOuterRadius; }
            set
            {
                mOuterRadius = value;
                WorldFrame.Instance.UpdateTerrainBrush(mInnerRadius, mOuterRadius);
            }
        }

        public void OnChange(TimeSpan diff)
        {
            if (CheckRequirements() == false)
                return;

            var parameters = new TerrainChangeParameters()
            {
                Algorithm = ChangeAlgorithm,
                Center = MousePosition,
                InnerRadius = mInnerRadius,
                OuterRadius = mOuterRadius,
                Method = ChangeType,
                TimeDiff = diff
            };

            WorldFrame.Instance.MapManager.OnEditTerrain(parameters);
        }

        private bool CheckRequirements()
        {
            if (IsTerrainHovered == false)
                return false;

            var bindings = Settings.KeyBindings.Instance;
            var state = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(state);

            if (KeyHelper.AreKeysDown(state, bindings.Interaction.Edit) == false &&
                KeyHelper.AreKeysDown(state, bindings.Interaction.EditInverse) == false)
                return false;

            return true;
        }
    }
}

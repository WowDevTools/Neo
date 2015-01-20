using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Sky
{
    enum LightColor
    {
        Diffuse = 0,
        Ambient = 1,
        Top = 2,
        Middle,
        MiddleLower,
        Lower,
        Horizon,
        Fog,
        Sun,
        Halo,
        Cloud,
        MaxLightType
    }

    enum LightFloat
    {
        FogEnd,
        FogDensity,
        FogScale,
        MaxLightFloat
    }

    abstract class SkyManager
    {
        public abstract void OnEnterWorld(int mapId);
        public abstract void AsyncUpdate();
        public abstract void UpdatePosition(Vector3 position);
        public abstract void SyncUpdate();
        public abstract void Initialize();

        public static SkyManager Instance { get; private set; }

        public static void InitVersion(FileDataVersion version)
        {
            switch(version)
            {
                case FileDataVersion.Warlords:
                    Instance = new WoD.LightManager();
                    break;

                default:
                    throw new NotSupportedException("Sorry, version not supported yet");
            }
        }
    }
}

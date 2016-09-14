using System;
using System.Numerics;

namespace Neo.IO.Files.Sky
{
	public enum LightColor
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

	public enum LightFloat
    {
        FogEnd,
        FogDensity,
        FogScale,
        MaxLightFloat
    }

	public abstract class SkyManager
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

                case FileDataVersion.Lichking:
                    Instance = new Wotlk.LightManager();
                    break;

                default:
                    throw new NotSupportedException("Sorry, version not supported yet");
            }
        }
    }
}

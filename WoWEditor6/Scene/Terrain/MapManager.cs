using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Scene.Terrain
{
    class MapManager
    {
        public string Continent { get; private set; }
        public int MapId { get; private set; }

        public void Initialize()
        {

        }

        public void EnterWorld(int mapId, string continent)
        {
            MapId = mapId;
            Continent = continent;
            WorldFrame.Instance.State = AppState.LoadingScreen;
        }
    }
}

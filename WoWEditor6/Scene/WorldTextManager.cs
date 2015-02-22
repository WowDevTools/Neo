
using System.Collections.Generic;
using System.Drawing;
using SharpDX;

namespace WoWEditor6.Scene
{
    class WorldTextManager
    {
        private readonly List<WorldText> mWorldTexts = new List<WorldText>();

        public void Initialize()
        {
            /*var font = new Font("Arial", 200);
            var worldText = new WorldText(font, Brushes.White)
            {
                Position = new Vector3(17011.38f, 15896.24f, 192.6388f),
                Text = "Testing Testing BLAA!"
            };

            AddText(worldText);*/
        }

        public void Shutdown()
        {

        }

        public void OnFrame()
        {
            WorldText.BeginDraw();

            lock (mWorldTexts)
            {
                foreach (var text in mWorldTexts)
                    text.OnFrame();
            }
        }

        public void AddText(WorldText text)
        {
            lock (mWorldTexts)
            {
                mWorldTexts.Add(text);
            }
        }
    }
}

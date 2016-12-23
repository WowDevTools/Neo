using System.Collections.Generic;

namespace Neo.Scene
{
	internal class WorldTextManager
    {
        private readonly List<WorldText> mWorldTexts = new List<WorldText>();

        public void Initialize()
        {

        }

        public void Shutdown()
        {
            DisposeAll();
        }

        public void OnFrame(Camera camera)
        {
            WorldText.BeginDraw();

            lock (this.mWorldTexts)
            {
                foreach (var text in this.mWorldTexts)
                {
	                text.OnFrame(camera);
                }
            }
        }

        public void AddText(WorldText text)
        {
            lock (this.mWorldTexts)
            {
	            this.mWorldTexts.Add(text);
            }
        }

        public void RemoveText(WorldText text)
        {
            lock (this.mWorldTexts)
            {
	            this.mWorldTexts.Remove(text);
            }
        }

        private void DisposeAll()
        {
            lock (this.mWorldTexts)
            {
                foreach (var text in this.mWorldTexts)
                {
	                text.Dispose();
                }

	            this.mWorldTexts.Clear();
            }
        }
    }
}

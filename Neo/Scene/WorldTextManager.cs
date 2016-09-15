using System.Collections.Generic;

namespace Neo.Scene
{
    class WorldTextManager
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

            lock (mWorldTexts)
            {
                foreach (var text in mWorldTexts)
                    text.OnFrame(camera);
            }
        }

        public void AddText(WorldText text)
        {
            lock (mWorldTexts)
            {
                mWorldTexts.Add(text);
            }
        }

        public void RemoveText(WorldText text)
        {
            lock (mWorldTexts)
            {
                mWorldTexts.Remove(text);
            }
        }

        private void DisposeAll()
        {
            lock (mWorldTexts)
            {
                foreach (var text in mWorldTexts)
                    text.Dispose();

                mWorldTexts.Clear();
            }
        }
    }
}

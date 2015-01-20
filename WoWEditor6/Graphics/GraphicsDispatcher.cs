using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Graphics
{
    class GraphicsDispatcher
    {
        private readonly List<Action> mFrames = new List<Action>();

        public void ProcessFrame()
        {
            var start = Environment.TickCount;
            var numFrames = 0;

            do
            {
                Action curFrame;
                lock(mFrames)
                {
                    if (mFrames.Count == 0)
                        return;

                    curFrame = mFrames[0];
                    mFrames.RemoveAt(0);
                }

                curFrame();
                ++numFrames;
            } while (Environment.TickCount - start < 30 && numFrames < 10);
        }

        public void BeginInvoke(Action frame)
        {
            lock (mFrames)
                mFrames.Add(frame);
        }
    }
}

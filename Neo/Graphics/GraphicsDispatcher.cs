using System;
using System.Collections.Generic;
using System.Threading;

namespace Neo.Graphics
{
    public class GraphicsDispatcher
    {
        private readonly List<Action> mFrames = new List<Action>();
        private int mAssignedThread;

        public bool InvokeRequired { get { return Thread.CurrentThread.ManagedThreadId != mAssignedThread; } }

        public void AssignToThread()
        {
            mAssignedThread = Thread.CurrentThread.ManagedThreadId;
        }

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
            } while (Environment.TickCount - start < 30 && numFrames < 15);
        }

        public object BeginInvoke(Action frame)
        {
            lock (mFrames)
                mFrames.Add(frame);
            return frame;
        }

        public void Remove(object token)
        {
            lock (mFrames)
                mFrames.Remove((Action)token);
        }
    }
}

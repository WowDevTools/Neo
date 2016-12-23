using System;
using System.Collections.Generic;
using System.Threading;

namespace Neo.Graphics
{
    public class GraphicsDispatcher
    {
        private readonly List<Action> mFrames = new List<Action>();
        private int mAssignedThread;

        public bool InvokeRequired { get { return Thread.CurrentThread.ManagedThreadId != this.mAssignedThread; } }

        public void AssignToThread()
        {
	        this.mAssignedThread = Thread.CurrentThread.ManagedThreadId;
        }

        public void ProcessFrame()
        {
            var start = Environment.TickCount;
            var numFrames = 0;

            do
            {
                Action curFrame;
                lock(this.mFrames)
                {
                    if (this.mFrames.Count == 0)
                    {
	                    return;
                    }

	                curFrame = this.mFrames[0];
	                this.mFrames.RemoveAt(0);
                }

                curFrame();
                ++numFrames;
            } while (Environment.TickCount - start < 30 && numFrames < 15);
        }

        public object BeginInvoke(Action frame)
        {
            lock (this.mFrames)
            {
	            this.mFrames.Add(frame);
            }
	        return frame;
        }

        public void Remove(object token)
        {
            lock (this.mFrames)
            {
	            this.mFrames.Remove((Action)token);
            }
        }
    }
}

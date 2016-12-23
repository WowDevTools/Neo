using System;
using System.Diagnostics;

namespace Neo.Utils
{
	internal class TimerScope : IDisposable
    {
        private Stopwatch mWatch;
        private string mTask;
        private readonly int mDelay;

        public TimerScope(string task, int minTimeForOutput = 0)
        {
            mDelay = minTimeForOutput;
            mTask = task;
            mWatch = Stopwatch.StartNew();
        }

        ~TimerScope()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mWatch != null)
            {
                mWatch.Stop();
                if (mWatch.ElapsedMilliseconds >= mDelay)
                {
	                Log.Debug(string.Format("Task {0} finished in {1} milliseconds", this.mTask, this.mWatch.ElapsedMilliseconds));
                }

	            mWatch = null;
                mTask = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

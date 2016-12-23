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
	        this.mDelay = minTimeForOutput;
	        this.mTask = task;
	        this.mWatch = Stopwatch.StartNew();
        }

        ~TimerScope()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this.mWatch != null)
            {
	            this.mWatch.Stop();
                if (this.mWatch.ElapsedMilliseconds >= this.mDelay)
                {
	                Log.Debug(string.Format("Task {0} finished in {1} milliseconds", this.mTask, this.mWatch.ElapsedMilliseconds));
                }

	            this.mWatch = null;
	            this.mTask = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

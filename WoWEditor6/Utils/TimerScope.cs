using System;
using System.Diagnostics;

namespace WoWEditor6.Utils
{
    class TimerScope : IDisposable
    {
        private Stopwatch mWatch;
        private string mTask;

        public TimerScope(string task)
        {
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
                Log.Debug(string.Format("Task {0} finished in {1} milliseconds", mTask, mWatch.ElapsedMilliseconds));

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

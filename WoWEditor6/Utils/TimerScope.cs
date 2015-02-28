using System;
using System.Diagnostics;

namespace WoWEditor6.Utils
{
    class TimerScope : IDisposable
    {
        private readonly Stopwatch mWatch;
        private readonly string mTask;
        private readonly int mDelay;

        public TimerScope(string task, int minTimeForOutput = 0)
        {
            mDelay = minTimeForOutput;
            mTask = task;
            mWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            mWatch.Stop();
            if (mWatch.ElapsedMilliseconds >= mDelay)
                Log.Debug(string.Format("Task {0} finished in {1} milliseconds", mTask, mWatch.ElapsedMilliseconds));
        }
    }
}

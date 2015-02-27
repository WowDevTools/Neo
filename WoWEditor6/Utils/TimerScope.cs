using System;
using System.Diagnostics;

namespace WoWEditor6.Utils
{
    class TimerScope : IDisposable
    {
        private readonly Stopwatch mWatch;
        private readonly string mTask;

        public TimerScope(string task)
        {
            mTask = task;
            mWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            mWatch.Stop();
            Log.Debug(string.Format("Task {0} finished in {1} milliseconds", mTask, mWatch.ElapsedMilliseconds));
        }
    }
}

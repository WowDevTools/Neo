using System;
using System.Diagnostics;

namespace WoWEditor6.Utils
{
    class TimeManager
    {
        public static TimeManager Instance { get; private set; }

        private readonly Stopwatch mTimer = Stopwatch.StartNew();

        static TimeManager()
        {
            Instance = new TimeManager();
        }

        public TimeSpan GetTime()
        {
            return mTimer.Elapsed;
        }

        public void Reset()
        {
            mTimer.Restart();
        }
    }
}

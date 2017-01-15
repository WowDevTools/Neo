using System;
using System.Diagnostics;

namespace Neo.Utils
{
	internal class TimeManager
    {
        public static TimeManager Instance { get; private set; }

        private readonly Stopwatch mTimer = Stopwatch.StartNew();

        static TimeManager()
        {
            Instance = new TimeManager();
        }

        public TimeSpan GetTime()
        {
            return this.mTimer.Elapsed;
        }

        public void Reset()
        {
	        this.mTimer.Restart();
        }
    }
}

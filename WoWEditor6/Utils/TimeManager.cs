using System;

namespace WoWEditor6.Utils
{
    class TimeManager
    {
        public static TimeManager Instance { get; private set; }

        private DateTime mStartTime = DateTime.Now;

        static TimeManager()
        {
            Instance = new TimeManager();
        }

        public TimeSpan GetTime()
        {
            return DateTime.Now - mStartTime;
        }

        public void Reset()
        {
            mStartTime = DateTime.Now;
        }
    }
}

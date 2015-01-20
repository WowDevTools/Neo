using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Utils
{
    class TimeManager
    {
        public static TimeManager Instance { get; } = new TimeManager();

        private DateTime mStartTime = DateTime.Now;

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

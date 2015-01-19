using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6
{
    public class Singleton<T> where T : new()
    {
        public static T Instance { get { return mInstance; } }

        static Singleton()
        {
            mInstance = new T();
        }

        private static T mInstance = default(T);
    }
}

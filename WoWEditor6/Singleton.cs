
namespace WoWEditor6
{
    public class Singleton<T> where T : new()
    {
        private static T gInstance;
        public static T Instance
        {
            get
            {
                if (gInstance != null) return gInstance;
                gInstance = new T();
                return gInstance;
            }
        }
    }
}


namespace WoWEditor6
{
    public class Singleton<T> where T : new()
    {
        public static T Instance { get; } = new T();
    }
}

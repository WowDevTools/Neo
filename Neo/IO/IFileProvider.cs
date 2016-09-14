using System.IO;

namespace WoWEditor6.IO
{
    interface IFileProvider
    {
        Stream OpenFile(string path);
        bool Exists(string path);
    }
}

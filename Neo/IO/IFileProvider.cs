using System.IO;

namespace Neo.IO
{
    interface IFileProvider
    {
        Stream OpenFile(string path);
        bool Exists(string path);
    }
}

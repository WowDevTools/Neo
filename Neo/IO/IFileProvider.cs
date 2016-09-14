using System.IO;

namespace Neo.IO
{
    public interface IFileProvider
    {
        Stream OpenFile(string path);
        bool Exists(string path);
    }
}

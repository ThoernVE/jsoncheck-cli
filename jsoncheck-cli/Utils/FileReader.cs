
namespace JsonCheck.Utils
{

    public sealed class FileReader : IFileReader
    {
        public bool Exists(string path) => File.Exists(path);
        public string ReadAllText(string path) => File.ReadAllText(path);
    }

    public interface IFileReader
    {
        string ReadAllText(string path);
        bool Exists(string path);
    }
}
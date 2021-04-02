using System.IO;

namespace MSyics.Traceyi
{
    /// <summary>
    /// FileStream を作成するクラスです。
    /// </summary>
    internal class FileStreamFactory : IFileStreamFactory
    {
        public FileStream Create(string path) => new(path, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.None);

        public void Dispose(FileStream stream) => stream.Dispose();
    }
}

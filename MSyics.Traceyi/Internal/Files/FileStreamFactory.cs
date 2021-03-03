﻿using System.IO;

namespace MSyics.Traceyi
{
    /// <summary>
    /// FileStream を作成するクラスです。
    /// </summary>
    internal class FileStreamFactory : IFileStreamFactory
    {
        public FileStream Create(string path) => new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 40960, FileOptions.None);

        public void Dispose(FileStream stream)
        {
            stream.Dispose();
        }
    }
}
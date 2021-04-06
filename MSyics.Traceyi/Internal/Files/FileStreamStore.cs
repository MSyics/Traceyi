using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MSyics.Traceyi
{
    /// <summary>
    /// FileStream を管理するクラスです。
    /// </summary>
    internal class FileStreamStore
    {
        readonly IFileStreamFactory factory;
        readonly Dictionary<string, FileStream> streams = new();

        public FileStreamStore(IFileStreamFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// 指定したパスの FileStream が存在するかどうかを判定します。
        /// </summary>
        public bool Exists(string path) => streams.ContainsKey(path);

        /// <summary>
        /// 指定したパスの FileStream を取得します。
        /// </summary>
        public bool TryGet(string path, out FileStream stream) => streams.TryGetValue(path, out stream);

        /// <summary>
        /// 指定したパスのファイルサイズを取得します。
        /// </summary>
        public bool TryGetLength(string path, out long length)
        {
            lock (((ICollection)streams).SyncRoot)
            {
                if (streams.TryGetValue(path, out var stream) && stream.CanWrite)
                {
                    length = stream.Length;
                    return true;
                }
            }

            var file = new FileInfo(path);
            if (file.Exists)
            {
                length = file.Length;
                return true;
            }

            length = 0;
            return false;
        }

        /// <summary>
        /// 指定したパスの FileStream を取得します。存在しない場合は作成して登録します。
        /// </summary>
        public FileStream GetOrAdd(string path)
        {
            lock (((ICollection)streams).SyncRoot)
            {
                if (streams.TryGetValue(path, out var stream) && stream.CanWrite)
                {
                    stream.Position = stream.Length;
                }
                else
                {
                    stream = factory.Create(path);
                    streams[path] = stream;
                }
                return stream;
            }
        }

        /// <summary>
        /// 指定したパスの FileStream を破棄して管理対象から外します。
        /// </summary>
        public void Remove(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            lock (((ICollection)streams).SyncRoot)
            {
                if (!streams.TryGetValue(path, out var stream)) return;

                factory.Dispose(stream);
                streams.Remove(path);
            }
        }

        /// <summary>
        /// 管理しているすべての FileStream を破棄して管理対象から外します。
        /// </summary>
        public void Clear()
        {
            if (streams.Count == 0) return;

            lock (((ICollection)streams).SyncRoot)
            {
                foreach (var stream in streams.Values)
                {
                    factory.Dispose(stream);
                }

                streams.Clear();
            }
        }
    }
}

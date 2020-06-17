using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MSyics.Traceyi
{
    internal class ReuseFileStreamManager
    {
        private static readonly Dictionary<int, string> paths = new Dictionary<int, string>();
        private static readonly Dictionary<string, ReuseFileStream> streams = new Dictionary<string, ReuseFileStream>();

        public bool Exists(string path) => streams.ContainsKey(path);

        public bool TryGet(string path, out ReuseFileStream stream) => streams.TryGetValue(path, out stream);

        public FileStream GetOrAdd(int threadId, string path)
        {
            if (paths.TryGetValue(threadId, out string currentPath))
            {
                if (!string.IsNullOrEmpty(currentPath) && currentPath != path)
                {
                    Remove(currentPath);
                }
            }
            paths[threadId] = path;

            if (streams.TryGetValue(path, out var stream))
            {
                stream.Position = stream.Length;
            }
            else
            {
                stream = new ReuseFileStream(path);
                streams[path] = stream;
            }

            return stream;
        }

        public void Remove(string path)
        {
            if (string.IsNullOrEmpty(path)) { return; }
            if (streams.TryGetValue(path, out var stream))
            {
                stream.Clean();
                streams.Remove(path);
            }
        }

        public void Clear()
        {
            if (streams.Count == 0) { return; }
            lock (((ICollection)streams).SyncRoot)
            {
                foreach (var item in streams.ToArray())
                {
                    item.Value.Clean();
                    streams.Remove(item.Key);
                }
            }
        }

        public ReuseFileStream this[string path] { get => streams[path]; set => streams[path] = value; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSyics.Traceyi
{
    internal class ReuseFileStreamManager
    {
        [ThreadStatic]
        private static string CurrentPath;
        private static readonly Dictionary<string, ReuseFileStream> Streams = new Dictionary<string, ReuseFileStream>();

        public bool Exists(string path) => Streams.ContainsKey(path);

        public bool TryGet(string path, out ReuseFileStream stream) => Streams.TryGetValue(path, out stream);

        public FileStream GetOrAdd(string path)
        {
            if ((!string.IsNullOrEmpty(CurrentPath)) && CurrentPath != path)
            {
                Remove(CurrentPath);
            }
            CurrentPath = path;

            if (Streams.TryGetValue(path, out var stream))
            {
                stream.Position = stream.Length;
            }
            else
            {
                stream = new ReuseFileStream(path);
                Streams[path] = stream;
            }

            return stream;
        }

        public void Remove(string path)
        {
            if (string.IsNullOrEmpty(path)) { return; }
            if (Streams.TryGetValue(path, out var stream))
            {
                stream.Clean();
                Streams.Remove(path);
            }
        }

        public void Clear()
        {
            if (Streams.Count == 0) { return; }
            lock (((ICollection)Streams).SyncRoot)
            {
                foreach (var item in Streams.ToArray())
                {
                    item.Value.Clean();
                    Streams.Remove(item.Key);
                }
            }
        }

        public ReuseFileStream this[string path] { get => Streams[path]; set => Streams[path] = value; }
    }
}

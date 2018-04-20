/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSyics.Traceyi
{
    internal class ReuseFileStreamManager
    {
        [ThreadStatic]
        private static string CurrentPath;
        private static ConcurrentDictionary<string, ReuseFileStream> Streams { get; } = new ConcurrentDictionary<string, ReuseFileStream>();

        public bool Exists(string path) => Streams.ContainsKey(path);

        public bool TryGet(string path, out ReuseFileStream stream) => Streams.TryGetValue(path, out stream);

        public FileStream GetOrAdd(string path)
        {
            if ((!string.IsNullOrWhiteSpace(CurrentPath)) && CurrentPath != path)
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
            if (string.IsNullOrEmpty(path)) return;

            if (Streams.TryRemove(path, out var stream))
            {
                stream.Clean();
            }
        }

        public void Clear()
        {
            if (Streams.Count == 0) { return; }

            foreach (var item in Streams.ToArray())
            {
                Remove(item.Key);
            }
        }

        public ReuseFileStream this[string path]
        {
            get { return Streams[path]; }
            set { Streams[path] = value; }
        }
    }
}

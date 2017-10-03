/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
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
        private static string m_currentPath;
        private static Dictionary<string, ReuseFileStream> m_streams = new Dictionary<string, ReuseFileStream>();

        public bool Exists(string path) => m_streams.ContainsKey(path);

        public bool TryGet(string path, out ReuseFileStream stream) => m_streams.TryGetValue(path, out stream);

        public FileStream AddOrUpdate(string path)
        {
            if ((!string.IsNullOrEmpty(m_currentPath)) && m_currentPath != path)
            {
                Remove(m_currentPath);
            }
            m_currentPath = path;

            if (m_streams.TryGetValue(path, out var stream))
            {
                stream.Position = stream.Length;
            }
            else
            {
                stream = new ReuseFileStream(path);
                m_streams[path] = stream;
            }

            return stream;
        }

        public void Remove(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (m_streams.TryGetValue(path, out var stream))
            {
                stream.Clean();
                m_streams.Remove(path);
            }
        }

        public void Clear()
        {
            if (m_streams.Count == 0) { return; }

            lock (((ICollection)m_streams).SyncRoot)
            {
                foreach (var item in m_streams.ToArray())
                {
                    item.Value.Clean();
                    m_streams.Remove(item.Key);
                }
            }
        }

        public ReuseFileStream this[string path]
        {
            get { return m_streams[path]; }
            set { m_streams[path] = value; }
        }
    }
}

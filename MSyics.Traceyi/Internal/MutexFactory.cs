using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// 名前付き Mutex を作成するクラスです。
    /// </summary>
    internal sealed class MutexFactory
    {
        public static MutexFactory Create() => new MutexFactory();

        const int maxLength = 260;
        readonly string prefix = @"Global\";
        private (string key, string name) created;

        private MutexFactory()
        {
        }

        /// <summary>
        /// 指定した文字列に対する Mutex を取得します。
        /// </summary>
        public Mutex Get(string source)
        {
            var key = source.ToUpperInvariant();
            if (created.key != key)
            {
                using var hash = new HashRingDeriveBytes<SHA256Managed>(Encoding.UTF8.GetBytes(key));
                var length = maxLength - prefix.Length;
                var name = Convert.ToBase64String(hash.GetBytes(length)).Substring(0, length);
                created = (key, name);
            }

            return new Mutex(false, $@"{prefix}{created.name}");
        }
    }
}

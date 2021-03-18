using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ハッシュアルゴリズムとバイト配列を基にしてバイトシーケンスを派生するクラスです。
    /// </summary>
    /// <typeparam name="T">ハッシュアルゴリズム</typeparam>
    internal class HashRingDeriveBytes<T> : HashRingDeriveBytes
        where T : HashAlgorithm, new()
    {
        /// <summary>
        /// このクラスのインスタンスを生成します。
        /// </summary>
        public HashRingDeriveBytes(byte[] buffer) : base(new T(), buffer) { }
    }

    /// <summary>
    /// ハッシュアルゴリズムとバイト配列を基にしてバイトシーケンスを派生するクラスです。
    /// </summary>
    internal class HashRingDeriveBytes : DeriveBytes
    {
        private readonly HashAlgorithm m_hashAlgo;
        private readonly byte[] m_buffer;
        private int m_startIndex;

        /// <summary>
        /// 指定したハッシュアルゴリズムでこのクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="hash">ハッシュアルゴリズム</param>
        /// <param name="buffer">バッファー</param>
        public HashRingDeriveBytes(HashAlgorithm hash, byte[] buffer)
        {
            m_startIndex = 0;
            m_hashAlgo = hash;
            m_buffer = buffer;
        }

        /// <summary>
        /// 擬似ランダムキーバイトを返します。
        /// </summary>
        /// <param name="cb">生成する擬似ランダムキーバイトの数。 </param>
        /// <returns>擬似ランダムキーバイトを格納したバイト配列。</returns>
        public override byte[] GetBytes(int cb)
        {
            return GetHashBytes().
                Skip(m_startIndex).
                Take(cb).
                ToArray();
        }

        /// <summary>
        /// 指定したバイト数をスキップして擬似ランダムキーバイトを返します。
        /// </summary>
        /// <param name="cb">生成する擬似ランダムキーバイトの数。</param>
        /// <param name="skipCount">スキップするバイト数</param>
        /// <returns>擬似ランダムキーバイトを格納したバイト配列。</returns>
        public byte[] GetBytes(int cb, int skipCount)
        {
            if (skipCount <= 0)
            {
                return GetBytes(cb);
            }
         
            return GetHashBytes().
                Skip(m_startIndex + skipCount).
                Take(cb).
                ToArray();
        }

        /// <summary>
        /// 操作の状態をリセットします。
        /// </summary>
        public override void Reset() => m_startIndex = 0;

        /// <summary>
        /// 擬似ランダムキーバイトを生成します。
        /// </summary>
        private IEnumerable<byte> GetHashBytes()
        {
            var startIndex = 0;
            var offset = -1;
            var hash = m_hashAlgo.ComputeHash(CreateBuffer(startIndex));

            try
            {
                while (true)
                {
                    ++startIndex;
                    ++offset;

                    if (offset == hash.Length)
                    {
                        hash = m_hashAlgo.ComputeHash(CreateBuffer(startIndex));
                        offset = 0;
                    }

                    yield return m_hashAlgo.
                        ComputeHash(hash, offset, 1).
                        Aggregate(hash[offset], (x, y) => (byte)(x ^ y));
                }
            }
            finally
            {
                m_startIndex = startIndex;
            }
        }

        /// <summary>
        /// 指定されたバイト配列の後ろに10桁分のバイト配列を作成します。
        /// </summary>
        private byte[] CreateBuffer(int index)
        {
            /*
             * 指定されたバイト配列の後ろに10桁分のバイト配列を作成します。
             * [BUFFER][x1][x2]...[x10]
             * [xN]:Int32型の整数値を桁毎に設定する
             */
            const int digit = 10;

            var bytes = new byte[m_buffer.Length + digit];
            Array.Copy(m_buffer, bytes, m_buffer.Length);
            for (int i = digit; i > 0; i--)
            {
                var pow = (int)Math.Pow(digit, i - 1);
                bytes[bytes.Length - i] = (byte)(index / pow);
                index %= pow;
            }

            return bytes;
        }

        /// <summary>
        /// リソースを破棄します。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            m_hashAlgo.Clear();
            base.Dispose(disposing);
        }
    }
}

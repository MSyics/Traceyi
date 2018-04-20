/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System.IO;
using System.Text;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータをファイルに記録します。
    /// </summary>
    internal class BasicFileLogger : TextLogger
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="stream">ファイル用のストリーム</param>
        /// <param name="encoding">文字エンコーディング</param>
        /// <param name="layout">レイアウト</param>
        public BasicFileLogger(FileStream stream, Encoding encoding, ILogLayout layout)
            : base(new StreamWriter(stream, encoding) { AutoFlush = true }, layout)
        {
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="stream">ファイル用のストリーム</param>
        /// <param name="encoding">文字エンコーディング</param>
        public BasicFileLogger(FileStream stream, Encoding encoding)
            : base(new StreamWriter(stream, encoding) { AutoFlush = true })
        {
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="stream">ファイル用のストリーム</param>
        public BasicFileLogger(FileStream stream)
            : this(stream, Encoding.Default)
        {
        }

        /// <summary>
        /// アンマネージリソースを破棄します。
        /// </summary>
        protected override void DisposeUnmanagedResources()
        {
            try
            {
                TextWriter.Dispose();
            }
            finally
            {
                base.DisposeUnmanagedResources();
            }
        }
    }
}

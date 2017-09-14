﻿using MSyics.Traceyi.Layout;
using System.IO;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータをファイルに記録します。
    /// </summary>
    public class FileLog : TextWriterLog
    {
        /// <summary>
        /// FileLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="stream">ファイル用のストリーム</param>
        /// <param name="encoding">文字エンコーディング</param>
        /// <param name="layout">レイアウト</param>
        public FileLog(FileStream stream, Encoding encoding, ILogLayout layout)
            : base(new StreamWriter(stream, encoding) { AutoFlush = true }, layout)
        {
        }

        /// <summary>
        /// FileLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="stream">ファイル用のストリーム</param>
        /// <param name="encoding">文字エンコーディング</param>
        public FileLog(FileStream stream, Encoding encoding)
            : base(new StreamWriter(stream, encoding) { AutoFlush = true })
        {
        }

        /// <summary>
        /// FileLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="stream">ファイル用のストリーム</param>
        public FileLog(FileStream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        /// <summary>
        /// アンマネージリソースを破棄します。
        /// </summary>
        protected override void DisposeUnmanagedResources()
        {
            try
            {
                this.TextWriter.Dispose();
            }
            finally
            {
                base.DisposeUnmanagedResources();
            }
        }
    }
}
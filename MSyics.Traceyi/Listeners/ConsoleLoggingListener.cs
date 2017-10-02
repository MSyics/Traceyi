using MSyics.Traceyi.Layout;
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータをコンソールに記録します。
    /// </summary>
    public class ConsoleLoggingListener : TextWriterLoggingListener
    {
        /// <summary>
        /// ConsoleLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        /// <param name="layout">レイアウト</param>
        public ConsoleLoggingListener(bool useErrorStream, ILogFormatter layout)
            : base(useErrorStream ? Console.Out : Console.Error, layout)
        {
        }

        /// <summary>
        /// ConsoleLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        public ConsoleLoggingListener(bool useErrorStream)
            : base(useErrorStream ? Console.Out : Console.Error)
        {
        }

        /// <summary>
        /// ConsoleLog クラスのインスタンスを初期化します。
        /// </summary>
        public ConsoleLoggingListener()
            : this(false)
        {
        }
    }
}

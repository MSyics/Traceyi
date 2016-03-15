using MSyics.Traceyi.Layout;
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータをコンソールに記録します。
    /// </summary>
    public class ConsoleLog : TextWriterLog
    {
        /// <summary>
        /// ConsoleLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        /// <param name="layout">レイアウト</param>
        public ConsoleLog(bool useErrorStream, ILogLayout layout)
            : base(useErrorStream ? Console.Out : Console.Error, layout)
        {
        }

        /// <summary>
        /// ConsoleLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        public ConsoleLog(bool useErrorStream)
            : base(useErrorStream ? Console.Out : Console.Error)
        {
        }

        /// <summary>
        /// ConsoleLog クラスのインスタンスを初期化します。
        /// </summary>
        public ConsoleLog()
            : this(false)
        {
        }
    }
}

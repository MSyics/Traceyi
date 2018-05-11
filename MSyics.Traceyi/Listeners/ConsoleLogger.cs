/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System;
using System.Text;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータをコンソールに記録します。
    /// </summary>
    public class ConsoleLogger : TextLogger
    {
        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        /// <param name="layout">レイアウト</param>
        public ConsoleLogger(bool useErrorStream, ILogLayout layout)
            : base(useErrorStream ? Console.Out : Console.Error, layout)
        {
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        public ConsoleLogger(bool useErrorStream)
            : base(useErrorStream ? Console.Out : Console.Error)
        {
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public ConsoleLogger()
            : this(false)
        {
        }

        public override void WriteCore(TraceEventArg e)
        {
            var color = Console.ForegroundColor;
            switch (e.Action)
            {
                case TraceAction.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case TraceAction.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case TraceAction.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case TraceAction.Start:
                case TraceAction.Stop:
                case TraceAction.Elapsed:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case TraceAction.Info:
                default:
                    break;
            }
            base.WriteCore(e);
            Console.ForegroundColor = color;
        }

        public override Encoding Encoding
        {
            get => base.Encoding;
            set
            {
                base.Encoding = value;
                Console.InputEncoding = value;
                Console.OutputEncoding = value;
            }
        }
    }
}

using MSyics.Traceyi.Layout;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータをコンソールに記録します。
    /// </summary>
    public class ConsoleLogger : TextLogger
    {
        readonly ConsoleColor defaultColor = Console.ForegroundColor;

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        /// <param name="layout">レイアウト</param>
        public ConsoleLogger(bool useErrorStream, ILogLayout layout)
            : base(useErrorStream ? Console.Error : Console.Out, layout)
        {
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        public ConsoleLogger(bool useErrorStream)
            : base(useErrorStream ? Console.Error : Console.Out)
        {
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public ConsoleLogger()
            : this(false)
        {
        }

        protected internal override void WriteCore(TraceEventArgs e)
        {
            SetConsoleColor(e.Action);
            base.WriteCore(e);
            Console.ForegroundColor = defaultColor;
        }

        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            Console.ForegroundColor = defaultColor;
        }

        protected void SetConsoleColor(TraceAction traceAction)
        {
            Console.ForegroundColor = traceAction switch
            {
                TraceAction.Trace or TraceAction.Debug => ConsoleColor.DarkGreen,
                TraceAction.Warning => ConsoleColor.DarkYellow,
                TraceAction.Error or TraceAction.Critical => ConsoleColor.DarkRed,
                TraceAction.Start or TraceAction.Stop => ConsoleColor.DarkCyan,
                _ => defaultColor,
            };
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

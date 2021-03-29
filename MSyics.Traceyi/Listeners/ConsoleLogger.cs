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
        readonly bool useErrorStream = false;

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        /// <param name="layout">レイアウト</param>
        public ConsoleLogger(bool useErrorStream, ILogLayout layout)
            : base(System.IO.TextWriter.Null, layout)
        {
            this.useErrorStream = useErrorStream;
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useErrorStream">標準出力ストリームと標準エラーストリームのどちらを使うかを示す値</param>
        public ConsoleLogger(bool useErrorStream)
            : base(System.IO.TextWriter.Null)
        {
            this.useErrorStream = useErrorStream;
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public ConsoleLogger()
            : this(false)
        {
        }

        public (int start, int length) ColoringPosition { get; set; }

        private bool TryGetColoringSettings(ReadOnlySpan<char> span, out int start, out int length, out bool toLast)
        {
            start = 0;
            length = 0;
            toLast = false;

            if (ColoringPosition.start < 0)
            {
                if (ColoringPosition.length < 0)
                {
                    return false;
                }
                length = ColoringPosition.start + ColoringPosition.length;
            }
            else
            {
                if (ColoringPosition.length < 0)
                {
                    double i = ColoringPosition.start + ColoringPosition.length;
                    if (i > span.Length)
                    {
                        return false;
                    }
                    if (i < 0)
                    {
                        length = ColoringPosition.start;
                    }
                    else
                    {
                        start = ColoringPosition.start + ColoringPosition.length;
                        length = Math.Abs(ColoringPosition.length);
                    }
                }
                else
                {
                    if (ColoringPosition.start > span.Length)
                    {
                        return false;
                    }
                    start = ColoringPosition.start;
                    double i = ColoringPosition.start + ColoringPosition.length;
                    if (i > span.Length - start)
                    {
                        length = span.Length - start;
                    }
                    else
                    {
                        length = ColoringPosition.length;
                    }
                }
            }

            if (length > span.Length)
            {
                length = span.Length;
            }
            toLast = start + length >= span.Length;
            return length > 0;
        }

        protected internal override void WriteCore(TraceEventArgs e)
        {
            var log = Layout.GetLog(e);
            if (string.IsNullOrEmpty(log))
            {
                return;
            }

            Console.OutputEncoding = Encoding;
            TextWriter = useErrorStream ? Console.Error : Console.Out;

            try
            {
                var span = log.AsSpan();
                if (!TryGetColoringSettings(span, out var start, out var length, out var toLast))
                {
                    TextWriter.WriteLine(log);
                    return;
                }

                if (start > 0)
                {
                    TextWriter.Write(span.Slice(0, start).ToString());
                }

                try
                {
                    SetConsoleColor(e.Action);
                    if (toLast)
                    {
                        TextWriter.WriteLine(span.Slice(start, length).ToString());
                        return;
                    }

                    TextWriter.Write(span.Slice(start, length).ToString());
                }
                finally
                {
                    Console.ForegroundColor = defaultColor;
                }

                TextWriter.WriteLine(span.Slice(start + length).ToString());
            }
            catch (FormatException)
            {
                TextWriter.WriteLine($"Can't write in because the layout is in the wrong format.");
            }
            catch (Exception ex)
            {
                TextWriter.WriteLine($"Can't write in.{NewLine}{ex}");
            }
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
                TraceAction.Trace => ConsoleColor.DarkGreen,
                TraceAction.Debug => ConsoleColor.DarkGray,
                TraceAction.Warning => ConsoleColor.DarkYellow,
                TraceAction.Error => ConsoleColor.Red,
                TraceAction.Critical => ConsoleColor.DarkRed,
                TraceAction.Start or TraceAction.Stop => ConsoleColor.DarkCyan,
                _ => defaultColor,
            };
        }
    }
}

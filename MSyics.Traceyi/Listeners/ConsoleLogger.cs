using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;

namespace MSyics.Traceyi.Listeners;

/// <summary>
/// トレースデータをコンソールに記録します。
/// </summary>
public class ConsoleLogger : TextLogger
{
    readonly ConsoleColor defaultColor = Console.ForegroundColor;

    /// <summary>
    /// クラスのインスタンスを初期化します。
    /// </summary>
    public ConsoleLogger(ILogLayout layout) :
        base(TextWriter.Null, layout)
    {
    }

    /// <summary>
    /// クラスのインスタンスを初期化します。
    /// </summary>
    public ConsoleLogger() : base(TextWriter.Null)
    {
    }

    /// <summary>
    /// 標準出力ストリームと標準エラーストリームのどちらを使うかを示す値を取得または設定します。
    /// </summary>
    public bool UseErrorStream { get; set; }

    /// <summary>
    /// 文字の着色位置を取得または設定します。
    /// </summary>
    public ConsoleColoringSettings Coloring { get; set; } = new ConsoleColoringSettings
    {
        Start = 0,
        Length = 1,
        ForTrace = ConsoleColor.DarkGreen,
        ForDebug = ConsoleColor.DarkGray,
        ForInfo = ConsoleColor.White,
        ForWarning = ConsoleColor.DarkYellow,
        ForError = ConsoleColor.Red,
        ForCritical = ConsoleColor.DarkRed,
        ForStart = ConsoleColor.DarkCyan,
        ForStop = ConsoleColor.DarkCyan,
    };

    private bool TryGetColoringPosition(ReadOnlySpan<char> span, out int start, out int length, out bool toLast)
    {
        start = 0;
        length = 0;
        toLast = false;

        if (Coloring.Start < 0)
        {
            if (Coloring.Length < 0) return false;

            length = Coloring.Start + Coloring.Length;
        }
        else
        {
            if (Coloring.Length < 0)
            {
                double i = Coloring.Start + Coloring.Length;
                if (i > span.Length) return false;

                if (i < 0)
                {
                    length = Coloring.Start;
                }
                else
                {
                    start = Coloring.Start + Coloring.Length;
                    length = Math.Abs(Coloring.Start);
                }
            }
            else
            {
                if (Coloring.Start > span.Length) return false;

                start = Coloring.Start;
                double i = Coloring.Start + Coloring.Length;
                if (i > span.Length - start)
                {
                    length = span.Length - start;
                }
                else
                {
                    length = Coloring.Length;
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

    protected internal override void WriteCore(TraceEventArgs e, int index)
    {
        var log = Layout.GetLog(e);
        if (string.IsNullOrEmpty(log)) return;

        Console.OutputEncoding = Encoding;
        TextWriter = UseErrorStream ? Console.Error : Console.Out;

        var span = log.AsSpan();
        if (!TryGetColoringPosition(span, out var start, out var length, out var toLast))
        {
            TextWriter.WriteLine(log);
            return;
        }

        if (start > 0)
        {
#if NETCOREAPP
            TextWriter.Write(span[..start].ToString());
#else
            TextWriter.Write(span.Slice(0, start).ToString());
#endif
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

#if NETCOREAPP
        TextWriter.WriteLine(span[(start + length)..].ToString());
#else
        TextWriter.WriteLine(span.Slice(start + length).ToString());
#endif
    }

    protected override void DisposeUnmanagedResources()
    {
        base.DisposeUnmanagedResources();
        Console.ForegroundColor = defaultColor;
    }

    protected void SetConsoleColor(TraceAction traceAction)
    {
        var color = traceAction switch
        {
            TraceAction.Trace => Coloring.ForTrace,
            TraceAction.Debug => Coloring.ForDebug,
            TraceAction.Info => Coloring.ForInfo,
            TraceAction.Warning => Coloring.ForWarning,
            TraceAction.Error => Coloring.ForError,
            TraceAction.Critical => Coloring.ForCritical,
            TraceAction.Start => Coloring.ForStart,
            TraceAction.Stop => Coloring.ForStop,
            _ => defaultColor,
        };

        if (!Enum.IsDefined(typeof(ConsoleColor), color)) return;

        Console.ForegroundColor = color;
    }
}

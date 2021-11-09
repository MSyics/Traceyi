using MSyics.Traceyi.Listeners;

namespace MSyics.Traceyi.Configration;

/// <summary>
/// Console 要素を表します。
/// </summary>
public class ConsoleLoggerElement : TextLoggerElement
{
    /// <summary>
    /// エラーストリームを使用するかどうかを示す値を取得または設定します。
    /// </summary>
    public bool UseErrorStream { get; set; } = false;

    /// <summary>
    /// 出力文字の色付け設定を取得または設定します。
    /// </summary>
    public ConsoleColoringSettings Coloring { get; set; } = new() { Start = 0, Length = 1 };

    /// <summary>
    /// 実行オブジェクトを取得します。
    /// </summary>
    public override ITraceEventListener GetRuntimeObject() =>
         new ConsoleLogger(Layout.GetRuntimeObject())
         {
             CloseTimeout = CloseTimeout,
             Coloring = (Coloring.Start, Coloring.Length),
             Encoding = GetEncoding(),
             Name = Name,
             NewLine = NewLine,
             UseAsync = UseAsync,
             UseErrorStream = UseErrorStream,
             UseLock = UseLock,
         };
}

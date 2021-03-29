using MSyics.Traceyi.Listeners;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Console 要素を表します。
    /// </summary>
    public class ConsoleElement : LoggerElement
    {
        /// <summary>
        /// エラーストリームを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseErrorStream { get; set; } = false;

        public ConsoleColoringSettings Coloring { get; set; } = new() { Start = 0, Length = 1 };

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceListener GetRuntimeObject() =>
             new ConsoleLogger(UseErrorStream, Layout.GetRuntimeObject())
             {
                 Name = Name,
                 NewLine = NewLine,
                 UseLock = UseLock,
                 UseAsync = UseAsync,
                 CloseTimeout = CloseTimeout,
                 Encoding = GetEncoding(),
                 Coloring = (Coloring.Start, Coloring.Length),
             };
    }

    public record ConsoleColoringSettings()
    {
        public int Start { get; set; }
        public int Length { get; set; }
    }
}
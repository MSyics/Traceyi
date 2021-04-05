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

        /// <summary>
        /// 出力文字の色付け設定を取得または設定します。
        /// </summary>
        public ConsoleColoringSettings Coloring { get; set; } = new() { Start = 0, Length = 1 };

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceListener GetRuntimeObject() =>
             new ConsoleLogger(Layout.GetRuntimeObject(), Concurrency)
             {
                 Name = Name,
                 NewLine = NewLine,
                 UseErrorStream = UseErrorStream,
                 UseLock = UseLock,
                 UseAsync = UseAsync,
                 CloseTimeout = CloseTimeout,
                 Encoding = GetEncoding(),
                 Coloring = (Coloring.Start, Coloring.Length),
             };
    }

    /// <summary>
    /// Console 出力文字の色付け設定を表します。
    /// </summary>
    public record ConsoleColoringSettings()
    {
        /// <summary>
        /// 色付け開始位置を取得または設定します。
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 色付けする文字の長さを取得または設定します。
        /// </summary>
        public int Length { get; set; }
    }
}
using MSyics.Traceyi.Listeners;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// File 要素を表します。
    /// </summary>
    public class FileLoggerElement : TextLoggerElement
    {
        /// <summary>
        /// パスを取得または設定します。
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        public long MaxLength { get; set; } = 0;

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルの最大アーカイブ数を取得または設定します。
        /// </summary>
        public int MaxArchiveCount { get; set; } = 0;

        /// <summary>
        /// プロセス間同期を使用するかどうか示す値を取得または設定します。
        /// </summary>
        public bool UseMutex { get; set; } = false;

        /// <summary>
        /// ファイルを開いたままにしておくかどうかを示す値を取得または設定します。
        /// </summary>
        public bool KeepFilesOpen { get; set; } = true;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceEventListener GetRuntimeObject() =>
            new FileLogger(Path, UseLock, UseAsync, Concurrency, UseMutex, KeepFilesOpen)
            {
                Encoding = GetEncoding(),
                Layout = Layout.GetRuntimeObject(),
                Name = Name,
                NewLine = NewLine,
                CloseTimeout = CloseTimeout,
                MaxLength = MaxLength,
                MaxArchiveCount = MaxArchiveCount,
            };
    }
}
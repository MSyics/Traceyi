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
        /// ファイルを開いたままにしておくかどうかを示す値を取得または設定します。
        /// </summary>
        public bool KeepFilesOpen { get; set; } = true;

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルの最大アーカイブ数を取得または設定します。
        /// </summary>
        public int MaxArchiveCount { get; set; } = 0;

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        public long MaxLength { get; set; } = 0;

        /// <summary>
        /// プロセス間同期を使用するかどうか示す値を取得または設定します。
        /// </summary>
        public bool UseMutex { get; set; } = false;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceEventListener GetRuntimeObject() =>
            new FileLogger(Path, KeepFilesOpen, Demux)
            {
                CloseTimeout = CloseTimeout,
                Encoding = GetEncoding(),
                Layout = Layout.GetRuntimeObject(),
                MaxLength = MaxLength,
                MaxArchiveCount = MaxArchiveCount,
                Name = Name,
                NewLine = NewLine,
                UseAsync = UseAsync,
                UseLock = UseLock,
                UseMutex = UseMutex,
            };
    }
}
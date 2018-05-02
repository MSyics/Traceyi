/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Listeners;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// File 要素を表します。
    /// </summary>
    public class FileElement : LoggerElement
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
        /// 書き込み上限バイト数を超えたファイルを残しておくのかどうかを示す値を取得または設定します。
        /// </summary>
        public bool LeaveFiles { get; set; } = false;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceListener GetRuntimeObject() =>
            new FileLogger(Path)
            {
                Encoding = GetEncoding(),
                Layout = Layout.GetRuntimeObject(),
                Name = Name,
                NewLine = NewLine,
                UseLock = UseLock,
                UseAsync = UseAsync,
                CloseTimeout = CloseTimeout,
                MaxLength = MaxLength,
                LeaveFiles = LeaveFiles,
            };
    }
}
/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// rotateFile 要素を表します。
    /// </summary>
    public class RotateFileLoggingListenerElement : TextWriterListenerElement
    {
        /// <summary>
        /// パスのレイアウトを取得または設定します。
        /// </summary>
        public string PathLayout { get; set; }

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        public long MaxLength { get; set; }

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルを残しておくのかどうかを示す値を取得または設定します。
        /// </summary>
        public bool LeaveFiles { get; set; }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceListener GetRuntimeObject()
        {
            return new RotateFileLoggingListener(this.PathLayout)
            {
                Encoding = this.GetEncoding(),
                Layout = this.Layout.GetRuntimeObject(),
                Name = this.Name,
                NewLine = this.NewLine,
                UseGlobalLock = this.UseGlobalLock,
                MaxLength = this.MaxLength,
                LeaveFiles = this.LeaveFiles,
            };
        }
    }
}
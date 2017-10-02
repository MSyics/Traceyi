/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// console 要素を表します。
    /// </summary>
    public class ConsoleLoggingListenerElement : TextWriterListenerElement
    {
        /// <summary>
        /// エラーストリームを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseErrorStream { get; set; }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceListener GetRuntimeObject() =>
             new ConsoleLoggingListener(this.UseErrorStream, this.Layout.GetRuntimeObject())
             {
                 Name = this.Name,
                 NewLine = this.NewLine,
                 UseGlobalLock = this.UseGlobalLock,
             };
    }

}
/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// layout 要素を表します。
    /// </summary>
    public class LayoutElement
    {
        /// <summary>
        /// レイアウト形式を取得または設定します。
        /// </summary>
        public string Format { get; set; } = LogFormatter.DefaultLayout;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public LogFormatter GetRuntimeObject() => new LogFormatter(this.Format);
    }
}
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースイベントデータを格納します。
    /// </summary>
    public sealed class TraceEventArg : EventArgs
    {
        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// トレースした日時を取得または設定します。
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// トレースの動作を取得または設定します。
        /// </summary>
        public TraceAction Action { get; set; }
    }
}

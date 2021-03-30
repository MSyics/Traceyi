using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ILogger 実装 TraceyiLogger のパラメーターを表します。
    /// </summary>
    internal sealed class TraceyiLoggerParameters
    {
        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// 拡張プロパティ設定オブジェクトを取得または設定します。
        /// </summary>
        public Action<dynamic> Extensions { get; set; }

        /// <summary>
        /// スコープラベルを取得または設定します。
        /// </summary>
        public object ScopeLabel { get; set; }
    }
}
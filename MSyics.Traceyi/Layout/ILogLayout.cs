using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// ログの記録形式を表します。
    /// </summary>
    public interface ILogLayout
    {
        /// <summary>
        /// 書式設定されたログデータを取得します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="dateTime">日時</param>
        /// <param name="action">トレース動作</param>
        /// <param name="cacheData">トレースイベントデータ</param>
        /// <returns>ログデータ</returns>
        string Format(object message, DateTime dateTime, TraceAction action, TraceEventCacheData cacheData);
    }
}

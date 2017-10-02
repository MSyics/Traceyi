using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 書式設定されたログを取得する機能を提供します。
    /// </summary>
    public interface ILogFormatter
    {
        /// <summary>
        /// 書式設定されたログデータを取得します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="dateTime">日時</param>
        /// <param name="action">トレース動作</param>
        /// <param name="cacheData">トレースイベントデータ</param>
        /// <returns>ログデータ</returns>
        string Format(TraceEventArg e);
    }
}

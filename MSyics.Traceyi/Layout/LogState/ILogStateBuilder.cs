using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 記録データを構築します。
    /// </summary>
    public interface ILogStateBuilder
    {
        /// <summary>
        /// 記録データを構築します。
        /// </summary>
        LogState Build();

        /// <summary>
        /// トレースイベントデータを記録データに設定します。
        /// </summary>
        ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembers members = LogStateMembers.All);

        /// <summary>
        /// 指定した値を記録データに設定します。
        /// </summary>
        ILogStateBuilder Set<T>(string member, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct;

        /// <summary>
        /// Null 許容型の値を記録データに設定します。
        /// </summary>
        ILogStateBuilder SetNullable<T>(string member, T value, bool enabled) where T : class;

        /// <summary>
        /// 拡張データを記録データに設定します。
        /// </summary>
        ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled);
    }
}

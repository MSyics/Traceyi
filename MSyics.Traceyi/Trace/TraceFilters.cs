using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// 選別するトレース動作を表します。
    /// </summary>
    [Flags]
    public enum TraceFilters
    {
        /// <summary>
        /// トレース動作
        /// </summary>
        Trace = 0x0001,

        /// <summary>
        /// デバッグ動作
        /// </summary>
        Debug = 0x0002,

        /// <summary>
        /// 通知動作
        /// </summary>
        Info = 0x0004,

        /// <summary>
        /// 注意動作
        /// </summary>
        Warning = 0x0008,

        /// <summary>
        /// エラー動作
        /// </summary>
        Error = 0x0010,

        /// <summary>
        /// 重大動作
        /// </summary>
        Critical = 0x0020,

        /// <summary>
        /// 開始動作
        /// </summary>
        Start = 0x0100,

        /// <summary>
        /// 停止動作
        /// </summary>
        Stop = 0x0200,

        /// <summary>
        /// Info | Start | Stop | Warning | Error
        /// </summary>
        Actions = Info | Warning | Error | Critical | Start | Stop,

        /// <summary>
        /// すべて
        /// </summary>
        All = Trace | Debug | Actions,

        /// <summary>
        /// 無し
        /// </summary>
        None = 0x1000,
    }
}

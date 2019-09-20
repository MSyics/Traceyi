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
        /// 無し
        /// </summary>
        None = 0x000,

        /// <summary>
        /// 無し
        /// </summary>
        Trace = 0x001,

        /// <summary>
        /// デバッグ動作
        /// </summary>
        Debug = 0x002,

        /// <summary>
        /// 通知動作
        /// </summary>
        Info = 0x004,

        /// <summary>
        /// 注意動作
        /// </summary>
        Warning = 0x008,

        /// <summary>
        /// エラー動作
        /// </summary>
        Error = 0x010,

        /// <summary>
        /// 重大動作
        /// </summary>
        Critical = 0x020,

        /// <summary>
        /// 開始動作
        /// </summary>
        Start = 0x100,

        /// <summary>
        /// 停止動作
        /// </summary>
        Stop = 0x200,

        /// <summary>
        /// 経過時間
        /// </summary>
        Elapsed = 0x400,

        /// <summary>
        /// Info | Start | Stop | Warning | Error
        /// </summary>
        Actions = Info | Warning | Error | Critical | Start | Stop,

        /// <summary>
        /// すべて
        /// </summary>
        All = Trace | Debug | Actions | Elapsed,
    }
}

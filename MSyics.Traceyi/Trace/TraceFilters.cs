/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
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
        None = 0x00,

        /// <summary>
        /// デバッグ動作
        /// </summary>
        Debug = 0x01,

        /// <summary>
        /// 通知動作
        /// </summary>
        Info = 0x02,

        /// <summary>
        /// 開始動作
        /// </summary>
        Start = 0x04,

        /// <summary>
        /// 停止動作
        /// </summary>
        Stop = 0x08,

        /// <summary>
        /// 注意動作
        /// </summary>
        Warning = 0x10,

        /// <summary>
        /// エラー動作
        /// </summary>
        Error = 0x20,

        /// <summary>
        /// 実行中の操作
        /// </summary>
        Calling = 0x40,

        /// <summary>
        /// 経過時間
        /// </summary>
        Elapsed = 0x80,

        /// <summary>
        /// Info | Start | Stop | Warning | Error
        /// </summary>
        Actions = Info | Start | Stop | Warning | Error,

        /// <summary>
        /// すべて
        /// </summary>
        All = Debug | Actions | Calling | Elapsed,
    }
}

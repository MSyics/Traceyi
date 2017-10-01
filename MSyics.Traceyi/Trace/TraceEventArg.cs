using System;
using System.Diagnostics;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースイベントデータを格納します。
    /// </summary>
    public sealed class TraceEventArg : EventArgs
    {
        /// <summary>
        /// トレースした日時を取得または設定します。
        /// </summary>
        public DateTime Traced { get; internal set; }

        /// <summary>
        /// トレースの動作を取得または設定します。
        /// </summary>
        public TraceAction Action { get; internal set; }

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public object Message { get; internal set; }

        /// <summary>
        /// スレッドに関連付けられた一意な識別子を取得します。
        /// </summary>
        public object ActivityId { get; internal set; }

        /// <summary>
        /// 操作識別子を取得します。
        /// </summary>
        public object OperationId { get; internal set; }

        /// <summary>
        /// トレースしたクラス名を取得します。
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// トレースしたメソッド情報を取得します。
        /// </summary>
        public string Member { get; internal set; }

        /// <summary>
        /// マネージスレッドの一意な識別子を取得します。
        /// </summary>
        public int ThreadId { get; internal set; }

        /// <summary>
        /// プロセスの一意な識別子を取得します。
        /// </summary>
        public int ProcessId { get; internal set; }

        /// <summary>
        /// プロセスの名前を取得します。
        /// </summary>
        public string ProcessName { get; internal set; }

        /// <summary>
        /// マシン名を取得します。
        /// </summary>
        public string MachineName { get; internal set; }
    }
}

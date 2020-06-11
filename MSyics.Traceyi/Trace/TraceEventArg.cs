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
        private static readonly int processId;
        private static readonly string processName;
        private static readonly string machineName = Environment.MachineName;

        static TraceEventArg()
        {
            using var process = Process.GetCurrentProcess();
            processId = process.Id;
            processName = process.ProcessName;
        }

        public TraceEventArg(DateTime traced, TraceAction action, object message)
        {
            Traced = traced;
            Action = action;
            Message = message;
        }

        /// <summary>
        /// トレースした日時を取得または設定します。
        /// </summary>
        public DateTime Traced { get; private set; }

        /// <summary>
        /// トレースの動作を取得または設定します。
        /// </summary>
        public TraceAction Action { get; private set; }

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public object Message { get; private set; }

        /// <summary>
        /// スレッドに関連付けられた一意な識別子を取得します。
        /// </summary>
        public object ActivityId { get; } = Traceable.Context.ActivityId;

        /// <summary>
        /// 操作識別子を取得します。
        /// </summary>
        public object OperationId { get; } = Traceable.Context.CurrentOperation.Id;

        /// <summary>
        /// マネージスレッドの一意な識別子を取得します。
        /// </summary>
        public int ThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// プロセスの一意な識別子を取得します。
        /// </summary>
        public int ProcessId { get; } = processId;

        /// <summary>
        /// プロセスの名前を取得します。
        /// </summary>
        public string ProcessName { get; } = processName;

        /// <summary>
        /// マシン名を取得します。
        /// </summary>
        public string MachineName { get; } = machineName;

        public override string ToString()
        {
            return $"{Traced}\t{Action}\t{OperationId}\t{ActivityId}\t{ThreadId}\t{ProcessId}\t{ProcessName}\t{MachineName}\t{Message}";
        }
    }
}

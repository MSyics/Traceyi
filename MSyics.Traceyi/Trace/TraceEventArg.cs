using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.Logging;

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
            using (var process = Process.GetCurrentProcess())
            {
                processId = process.Id;
                processName = process.ProcessName;
            }
        }

        public TraceEventArg(DateTime traced, TraceAction action, object message, TraceOperation operation)
        {
            Elapsed = operation.ScopeNumber == 0 ? TimeSpan.Zero : traced - operation.Started;
            Traced = traced;
            Action = action;
            Message = message;
            Operation = operation;
        }

        /// <summary>
        /// トレース操作を取得します。
        /// </summary>
        public TraceOperation Operation { get; private set; }

        /// <summary>
        /// トレースした日時を取得または設定します。
        /// </summary>
        public DateTime Traced { get; private set; }

        /// <summary>
        /// トレースの動作を取得または設定します。
        /// </summary>
        public TraceAction Action { get; private set; }

        /// <summary>
        /// 経過時間を取得または設定します。
        /// </summary>
        public TimeSpan Elapsed { get; private set; }

        /// <summary>
        /// スレッドに関連付けられた一意な識別子を取得します。
        /// </summary>
        public object ActivityId { get; } = Traceable.Context.ActivityId;

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

        /// <summary>
        /// メッセージを取得します。
        /// </summary>
        public object Message { get; private set; }

        /// <summary>
        /// メッセージが TraceyiLoggerMessage かどうかを示す値を取得します。
        /// </summary>
        public bool HasTraceyiLoggerMessage => Message is TraceyiLoggerMessage;

        /// <summary>
        /// ILogger で指定された EventId を取得します。
        /// </summary>
        public EventId EventId => TryGetTraceyiLoggerMessage(out var message) ? message.EventId : default;

        /// <summary>
        /// ILogger で指定されたメッセージを取得します。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool TryGetTraceyiLoggerMessage(out TraceyiLoggerMessage message)
        {
            message = null;
            if (Message is TraceyiLoggerMessage value)
            {
                message = value;
            }
            return message != null;
        }

        public override string ToString()
        {
            return $"{Traced}\t{Action}\t{Elapsed}\t{Operation.Id}\t{ActivityId}\t{ThreadId}\t{ProcessId}\t{ProcessName}\t{MachineName}\t{Message}";
        }
    }
}

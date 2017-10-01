using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースイベントのキャッシュを表します。
    /// </summary>
    public sealed class TraceEventCacheData
    {
        private static int m_processId;
        private static string m_processName;

        static TraceEventCacheData()
        {
            //using (var process = Process.GetCurrentProcess())
            //{
            //    m_processId = process.Id;
            //    m_processName = process.ProcessName;
            //}
        }

        /// <summary>
        /// トレースしたメソッド情報を取得します。
        /// </summary>
        public MemberInfo Member { get; } = TraceUtility.GetTracedMemberInfo();

        /// <summary>
        /// スレッドに関連付けられた一意な識別子を取得します。
        /// </summary>
        public object ActivityId { get; } = Traceable.Context.ActivityId;

        /// <summary>
        /// マシン名を取得します。
        /// </summary>
        public string MachineName { get; } = Environment.MachineName;

        /// <summary>
        /// プロセスの一意な識別子を取得します。
        /// </summary>
        public int ProcessId { get; } = TraceEventCacheData.m_processId;

        /// <summary>
        /// プロセスの名前を取得します。
        /// </summary>
        public string ProcessName { get; } = TraceEventCacheData.m_processName;

        /// <summary>
        /// マネージスレッドの一意な識別子を取得します。
        /// </summary>
        public int ThreadId { get; } //= Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// 操作識別子を取得します。
        /// </summary>
        public object OperationId { get; } //= Traceable.Context.CurrentOperation.OperationId;
    }
}

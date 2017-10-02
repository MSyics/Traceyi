/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
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
        private static int m_processId;
        private static string m_processName;
        private static string m_machineName = Environment.MachineName;

        static TraceEventArg()
        {
            using (var process = Process.GetCurrentProcess())
            {
                m_processId = process.Id;
                m_processName = process.ProcessName;
            }
        }

        public TraceEventArg(DateTime traced, TraceAction action, object message, bool useMemberInfo = false)
        {
            Traced = traced;
            Action = action;
            Message = message;

            if (useMemberInfo)
            {
                var memberInfo = TraceUtility.GetTracedMemberInfo();
                ClassName = memberInfo.ReflectedType.FullName;
                MemberName = memberInfo.Name;
            }
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
        public object OperationId { get; } = Traceable.Context.CurrentOperation.OperationId;

        /// <summary>
        /// トレースしたクラス名を取得します。
        /// </summary>
        public string ClassName { get; internal set; }

        /// <summary>
        /// トレースしたメンバー名を取得します。
        /// </summary>
        public string MemberName { get; internal set; }

        /// <summary>
        /// マネージスレッドの一意な識別子を取得します。
        /// </summary>
        public int ThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// プロセスの一意な識別子を取得します。
        /// </summary>
        public int ProcessId { get; } = m_processId;

        /// <summary>
        /// プロセスの名前を取得します。
        /// </summary>
        public string ProcessName { get; } = m_processName;

        /// <summary>
        /// マシン名を取得します。
        /// </summary>
        public string MachineName { get; } = m_machineName;
    }
}

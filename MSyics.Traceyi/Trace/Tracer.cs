/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Diagnostics;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースするクラスです。
    /// </summary>
    public sealed class Tracer
    {
        private static int ProcessId { get; set; }
        private static string ProcessName { get; set; }
        private static string MachineName { get; } = Environment.MachineName;

        static Tracer()
        {
            using (var process = Process.GetCurrentProcess())
            {
                ProcessId = process.Id;
                ProcessName = process.ProcessName;
            }
        }

        /// <summary>
        /// スレッドに関連付いたトレース基本情報を取得します。
        /// </summary>
        public TraceContext Context => Traceable.Context;

        public TracerSettings Settings { get; } = new TracerSettings();

        /// <summary>
        /// トレースイベントを設定します。
        /// </summary>
        public event EventHandler<TraceEventArg> Tracing;

        private void RaiseTracing(DateTime traced, TraceAction action, object message)
        {
            var eh = Tracing;
            if (eh == null) { return; }

            eh(this, new TraceEventArg(traced, action, message, Settings.UseMemberInfo));
        }

        #region Debug
        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(object message)
        {
            if (Settings.Filter.Contains(TraceAction.Debug))
            {
                RaiseTracing(DateTime.Now, TraceAction.Debug, message);
            }
        }
        #endregion

        #region Information
        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(object message)
        {
            if (Settings.Filter.Contains(TraceAction.Info))
            {
                RaiseTracing(DateTime.Now, TraceAction.Info, message);
            }
        }
        #endregion

        #region Warning
        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(object message)
        {
            if (Settings.Filter.Contains(TraceAction.Warning))
            {
                RaiseTracing(DateTime.Now, TraceAction.Warning, message);
            }
        }
        #endregion

        #region Error
        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(object message)
        {
            if (Settings.Filter.Contains(TraceAction.Error))
            {
                RaiseTracing(DateTime.Now, TraceAction.Error, message);
            }
        }
        #endregion

        #region Start
        internal void Start(object operationId, object message, Guid scopeId)
        {
            var operation = new TraceOperation()
            {
                OperationId = operationId ?? $"{new String('+', Context.OperationStack.Count)}", //TraceUtility.GetOperationId(),
                ScopeId = scopeId,
                StartedDate = DateTime.Now,
            };
            Context.OperationStack.Push(operation);

            if (Settings.Filter.Contains(TraceAction.Start))
            {
                RaiseTracing(operation.StartedDate, TraceAction.Start, message ?? operation.OperationId);
            }

            if (Settings.Filter.Contains(TraceAction.Calling))
            {
                var sb = new StringBuilder(Context.CurrentOperation.OperationId.ToString());
                //Context.Operations.Skip(1).Aggregate(sb, (x, y) => x.Insert(0, y.OperationId.ToString() + ">"));
                RaiseTracing(operation.StartedDate, TraceAction.Calling, sb);
            }
        }

        /// <summary>
        /// 操作の開始メッセージを残します。
        /// </summary>
        public void Start(object message)
        {
            Start(null, message, Guid.Empty);
        }

        /// <summary>
        /// 操作の開始メッセージを残します。
        /// </summary>
        public void Start()
        {
            Start(null, null, Guid.Empty);
        }
        #endregion

        #region Stop
        internal void Stop(object message, Guid scopeId)
        {
            var byScope = !scopeId.Equals(Guid.Empty);
            for (; ; )
            {
                if (Context.OperationStack.Count == 0) { break; }

                var currentOperation = Context.OperationStack.Peek();
                if (!byScope)
                {
                    if (currentOperation.UseScope) { break; }
                }

                var stopedDateTime = DateTime.Now;
                if (Settings.Filter.Contains(TraceAction.Elapsed))
                {
                    RaiseTracing(stopedDateTime, TraceAction.Elapsed, (stopedDateTime - currentOperation.StartedDate));
                }
                if (Settings.Filter.Contains(TraceAction.Stop))
                {
                    RaiseTracing(stopedDateTime, TraceAction.Stop, message ?? currentOperation.OperationId);
                }

                var popOperation = Context.OperationStack.Pop();
                if (scopeId.Equals(popOperation.ScopeId)) { break; }
            }
        }

        /// <summary>
        /// 操作の終了メッセージを残します。
        /// </summary>
        public void Stop(object message)
        {
            Stop(message, Guid.Empty);
        }

        /// <summary>
        /// 操作の終了メッセージを残します。
        /// </summary>
        public void Stop()
        {
            Stop(null, Guid.Empty);
        }
        #endregion
    }
}

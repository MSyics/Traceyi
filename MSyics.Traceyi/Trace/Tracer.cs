using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースするクラスです。
    /// </summary>
    public sealed class Tracer
    {
        internal Tracer() { }

        /// <summary>
        /// スレッドに関連付いたトレース基本情報を取得します。
        /// </summary>
        public TraceContext Context => Traceable.Context;

        /// <summary>
        /// 名前を取得します。
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 選別するトレース動作を取得または設定します。
        /// </summary>
        public TraceFilters Filters { get; set; } = TraceFilters.All;

        /// <summary>
        /// トレースする際にクラスメンバー情報を取得するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseMemberInfo { get; set; } = true;

        /// <summary>
        /// トレースイベントを設定します。
        /// </summary>
        public event EventHandler<TraceEventArg> Tracing;

        private void RaiseTracing(DateTime traced, TraceAction action, object message) =>
            Tracing?.Invoke(this, new TraceEventArg(traced, action, message, UseMemberInfo));

        #region Trace
        /// <summary>
        /// トレースに必要なメッセージを残します。
        /// </summary>
        public void Trace(object message)
        {
            if (Filters.Contains(TraceAction.Trace))
            {
                RaiseTracing(DateTime.Now, TraceAction.Trace, message);
            }
        }
        #endregion

        #region Debug
        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(object message)
        {
            if (Filters.Contains(TraceAction.Debug))
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
            if (Filters.Contains(TraceAction.Info))
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
            if (Filters.Contains(TraceAction.Warning))
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
            if (Filters.Contains(TraceAction.Error))
            {
                RaiseTracing(DateTime.Now, TraceAction.Error, message);
            }
        }
        #endregion

        #region Critical
        /// <summary>
        /// 重大メッセージを残します。
        /// </summary>
        public void Critical(object message)
        {
            if (Filters.Contains(TraceAction.Critical))
            {
                RaiseTracing(DateTime.Now, TraceAction.Critical, message);
            }
        }
        #endregion

        #region Start
        internal void Start(object operationId, object message, string scopeId)
        {
            var operation = new TraceOperation()
            {
                //OperationId = operationId,
                //OperationId = operationId ?? $"{TraceUtility.GetOperationId()}",
                OperationId = operationId ?? $"{new String('+', Context.OperationStack.Count)}", //TraceUtility.GetOperationId(),
                ScopeId = scopeId,
                StartedDate = DateTime.Now,
            };
            Context.OperationStack.Push(operation);

            if (Filters.Contains(TraceAction.Start))
            {
                RaiseTracing(operation.StartedDate, TraceAction.Start, message);
            }
        }

        /// <summary>
        /// 操作の開始メッセージを残します。
        /// </summary>
        public void Start(object message) => Start(null, message, null);

        /// <summary>
        /// 操作の開始メッセージを残します。
        /// </summary>
        public void Start() => Start(null, null, null);
        #endregion

        #region Stop
        internal void Stop(object message, string scopeId)
        {
            var byScope = scopeId != null;
            for (; ; )
            {
                if (Context.OperationStack.Count == 0) { break; }

                var currentOperation = Context.OperationStack.Peek();
                if (!byScope)
                {
                    if (currentOperation.UseScope) { break; }
                }

                var stopedDateTime = DateTime.Now;
                if (Filters.Contains(TraceAction.Elapsed))
                {
                    RaiseTracing(stopedDateTime, TraceAction.Elapsed, (stopedDateTime - currentOperation.StartedDate));
                }
                if (Filters.Contains(TraceAction.Stop))
                {
                    RaiseTracing(stopedDateTime, TraceAction.Stop, message);
                }

                var popOperation = Context.OperationStack.Pop();
                if (scopeId == popOperation.ScopeId) { break; }
            }
        }

        /// <summary>
        /// 操作の終了メッセージを残します。
        /// </summary>
        public void Stop(object message) => Stop(message, null);

        /// <summary>
        /// 操作の終了メッセージを残します。
        /// </summary>
        public void Stop() => Stop(null, null);
        #endregion
    }
}

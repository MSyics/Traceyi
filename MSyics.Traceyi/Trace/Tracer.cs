using System;
using System.Threading.Tasks;

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
        /// トレースイベントを設定します。
        /// </summary>
        public event EventHandler<TraceEventArg> Tracing;

        private void RaiseTracing(DateTime traced, TraceAction action, object message, TraceOperation operation)
        {
            if (!Filters.Contains(action)) { return; }

            Tracing?.Invoke(this, new TraceEventArg(traced, action, message, operation));
        }

        private void RaiseTracing(TraceAction action, object message)
        {
            if (!Filters.Contains(action)) return;

            Tracing?.Invoke(this, new TraceEventArg(DateTime.Now, action, message, Context.CurrentOperation));
        }

        #region Trace
        /// <summary>
        /// トレースに必要なメッセージを残します。
        /// </summary>
        public void Trace(object message) => RaiseTracing(TraceAction.Trace, message);
        #endregion

        #region Debug
        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(object message) => RaiseTracing(TraceAction.Debug, message);
        #endregion

        #region Information
        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(object message) => RaiseTracing(TraceAction.Info, message);
        #endregion

        #region Warning
        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(object message) => RaiseTracing(TraceAction.Warning, message);
        #endregion

        #region Error
        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(object message) => RaiseTracing(TraceAction.Error, message);
        #endregion

        #region Critical
        /// <summary>
        /// 重大メッセージを残します。
        /// </summary>
        public void Critical(object message) => RaiseTracing(TraceAction.Critical, message);
        #endregion

        #region Start
        internal string StartCore(object operationId, object message)
        {
            var operation = new TraceOperation()
            {
                Id = operationId,
                ScopeId = $"{ DateTime.Now.Ticks:x16}",
                ParentId = Context.CurrentOperation.ScopeId,
                ScopeNumber = Context.OperationStack.Count + 1,
                Started = DateTime.Now,
            };

            Context.OperationStack.Push(operation);
            RaiseTracing(operation.Started, TraceAction.Start, message, operation);

            return operation.ScopeId;
        }

        /// <summary>
        /// 操作の開始メッセージを残します。
        /// </summary>
        public void Start(object operationId = null, object message = null) => StartCore(operationId, message);
        #endregion

        #region Stop
        internal void Stop(object message, string scopeId, DateTime stopped)
        {
            for (; ; )
            {
                var operation = Context.CurrentOperation;
                if (operation.UseScope && scopeId == null) { break; }

                RaiseTracing(stopped, TraceAction.Stop, scopeId == operation.ScopeId ? message : default, operation);
                Context.OperationStack.Pop();

                if (scopeId == operation.ScopeId) { break; }
            }
        }

        internal void Stop(object message, string scopeId) => Stop(message, scopeId, DateTime.Now);

        /// <summary>
        /// 操作の終了メッセージを残します。
        /// </summary>
        public void Stop(object message = null) => Stop(message, null, DateTime.Now);
        #endregion
    }
}

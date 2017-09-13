using System;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースするクラスです。
    /// </summary>
    public sealed class Tracer
    {
        /// <summary>
        /// スレッドに関連付いたトレース基本情報を取得します。
        /// </summary>
        public TraceContext Context => Traceable.Context;

        /// <summary>
        /// 名前を取得します。
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// フィルターを取得します。
        /// </summary>
        public TraceFilters Filter { get; set; } = TraceFilters.All;

        /// <summary>
        /// トレースイベントを設定します。
        /// </summary>
        public event EventHandler<TraceEventArg> OnTrace;

        private void RaiseTrace(TraceAction action, DateTime dateTime, object message)
        {
            var eh = OnTrace;
            if (eh != null)
            {
                var data = new TraceEventArg()
                {
                    Message = message,
                    DateTime = dateTime,
                    Action = action,
                };
                
                eh(this, data);
            }
        }

        #region Debug
        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(object message)
        {
            if (this.Filter.Contains(TraceAction.Debug))
            {
                RaiseTrace(TraceAction.Debug, DateTime.Now, message);
            }
        }

        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(string message)
        {
            if (this.Filter.Contains(TraceAction.Debug))
            {
                RaiseTrace(TraceAction.Debug, DateTime.Now, message);
            }
        }

        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(string format, params object[] args)
        {
            if (this.Filter.Contains(TraceAction.Debug))
            {
                RaiseTrace(TraceAction.Debug, DateTime.Now, string.Format(format, args));
            }
        }
        #endregion

        #region Information
        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(object message)
        {
            if (this.Filter.Contains(TraceAction.Info))
            {
                RaiseTrace(TraceAction.Info, DateTime.Now, message);
            }
        }

        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(string message)
        {
            if (this.Filter.Contains(TraceAction.Info))
            {
                RaiseTrace(TraceAction.Info, DateTime.Now, message);
            }
        }

        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(string format, params object[] args)
        {
            if (this.Filter.Contains(TraceAction.Info))
            {
                RaiseTrace(TraceAction.Info, DateTime.Now, string.Format(format, args));
            }
        }
        #endregion

        #region Warning
        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(object message)
        {
            if (this.Filter.Contains(TraceAction.Warning))
            {
                RaiseTrace(TraceAction.Warning, DateTime.Now, message);
            }
        }

        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(string message)
        {
            if (this.Filter.Contains(TraceAction.Warning))
            {
                RaiseTrace(TraceAction.Warning, DateTime.Now, message);
            }
        }

        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(string format, params object[] args)
        {
            if (this.Filter.Contains(TraceAction.Warning))
            {
                RaiseTrace(TraceAction.Warning, DateTime.Now, string.Format(format, args));
            }
        }
        #endregion

        #region Error
        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(object message)
        {
            if (this.Filter.Contains(TraceAction.Error))
            {
                RaiseTrace(TraceAction.Error, DateTime.Now, message);
            }
        }

        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(string message)
        {
            if (this.Filter.Contains(TraceAction.Error))
            {
                RaiseTrace(TraceAction.Error, DateTime.Now, message);
            }
        }

        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(string format, params object[] args)
        {
            if (this.Filter.Contains(TraceAction.Error))
            {
                RaiseTrace(TraceAction.Error, DateTime.Now, string.Format(format, args));
            }
        }

        #endregion

        #region Start

        internal void Start(object operationId, object message, Guid scopeId)
        {
            var operation = new TraceOperation()
            {
                OperationId = operationId ?? TraceUtility.GetOperationId(),
                ScopeId = scopeId,
                StartedDate = DateTime.Now,
            };
            this.Context.OperationStack.Push(operation);

            if (this.Filter.Contains(TraceAction.Start))
            {
                RaiseTrace(TraceAction.Start, operation.StartedDate, message ?? operation.OperationId);
            }

            if (this.Filter.Contains(TraceAction.Calling))
            {
                var sb = new StringBuilder(this.Context.CurrentOperation.OperationId.ToString());
                this.Context.Operations.Skip(1).Aggregate(sb, (x, y) => x.Insert(0, y.OperationId.ToString() + "|"));
                RaiseTrace(TraceAction.Calling, operation.StartedDate, sb);
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
        public void Start(string message)
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
                if (this.Context.OperationStack.Count == 0) { break; }

                var currentOperation = this.Context.OperationStack.Peek();
                if (!byScope)
                {
                    if (currentOperation.UseScope) { break; }
                }

                var stopedDateTime = DateTime.Now;
                if (this.Filter.Contains(TraceAction.Elapsed))
                {
                    RaiseTrace(TraceAction.Elapsed, stopedDateTime, (stopedDateTime - currentOperation.StartedDate));
                }
                if (this.Filter.Contains(TraceAction.Stop))
                {
                    RaiseTrace(TraceAction.Stop, stopedDateTime, message ?? currentOperation.OperationId);
                }

                var popOperation = this.Context.OperationStack.Pop();
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
        public void Stop(string message)
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

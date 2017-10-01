using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

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

        /// <summary>
        /// 設定値を取得します。
        /// </summary>
        public TracerSettings Settings { get; } = new TracerSettings();

        /// <summary>
        /// トレースイベントを設定します。
        /// </summary>
        public event EventHandler<TraceEventArg> OnTrace;

        private void RaiseTrace(DateTime traced, TraceAction action, object message)
        {
            var eh = OnTrace;
            if (eh == null) { return; }

            var data = new TraceEventArg();
            if (Settings.CanListens.Traced) { data.Traced = traced; }
            if (Settings.CanListens.Action) { data.Action = action; }
            if (Settings.CanListens.Message) { data.Message = message; }
            if (Settings.CanListens.ActivityId) { data.ActivityId = Traceable.Context.ActivityId; }
            if (Settings.CanListens.OperationId) { data.OperationId = Traceable.Context.CurrentOperation.OperationId; }
            if (Settings.CanListens.ClassName || Settings.CanListens.MemberName)
            {
                var memberInfo = TraceUtility.GetTracedMemberInfo();
                if (Settings.CanListens.ClassName) { data.Class = memberInfo.ReflectedType.FullName; }
                if (Settings.CanListens.MemberName) { data.Member = memberInfo.Name; }
            }
            if (Settings.CanListens.ThreadId) { data.ThreadId = Thread.CurrentThread.ManagedThreadId; }
            if (Settings.CanListens.ProcessId || Settings.CanListens.ProcessName)
            {
                if (Settings.CanListens.ProcessId) { data.ProcessId = ProcessId; }
                if (Settings.CanListens.ProcessName) { data.ProcessName = ProcessName; }
            }
            if (Settings.CanListens.MachineName) { data.MachineName = MachineName; }

            eh(this, data);
        }

        #region Debug
        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(object message)
        {
            if (this.Settings.Filter.Contains(TraceAction.Debug))
            {
                RaiseTrace(DateTime.Now, TraceAction.Debug, message);
            }
        }

        /// <summary>
        /// デバッグに必要なメッセージを残します。
        /// </summary>
        public void Debug(string format, params object[] args)
        {
            if (this.Settings.Filter.Contains(TraceAction.Debug))
            {
                RaiseTrace(DateTime.Now, TraceAction.Debug, string.Format(format, args));
            }
        }
        #endregion

        #region Information
        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(object message)
        {
            if (this.Settings.Filter.Contains(TraceAction.Info))
            {
                RaiseTrace(DateTime.Now, TraceAction.Info, message);
            }
        }

        /// <summary>
        /// 通知メッセージを残します。
        /// </summary>
        public void Information(string format, params object[] args)
        {
            if (this.Settings.Filter.Contains(TraceAction.Info))
            {
                RaiseTrace(DateTime.Now, TraceAction.Info, string.Format(format, args));
            }
        }
        #endregion

        #region Warning
        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(object message)
        {
            if (this.Settings.Filter.Contains(TraceAction.Warning))
            {
                RaiseTrace(DateTime.Now, TraceAction.Warning, message);
            }
        }

        /// <summary>
        /// 注意メッセージを残します。
        /// </summary>
        public void Warning(string format, params object[] args)
        {
            if (this.Settings.Filter.Contains(TraceAction.Warning))
            {
                RaiseTrace(DateTime.Now, TraceAction.Warning, string.Format(format, args));
            }
        }
        #endregion

        #region Error
        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(object message)
        {
            if (this.Settings.Filter.Contains(TraceAction.Error))
            {
                RaiseTrace(DateTime.Now, TraceAction.Error, message);
            }
        }

        /// <summary>
        /// エラーメッセージを残します。
        /// </summary>
        public void Error(string format, params object[] args)
        {
            if (this.Settings.Filter.Contains(TraceAction.Error))
            {
                RaiseTrace(DateTime.Now, TraceAction.Error, string.Format(format, args));
            }
        }

        #endregion

        #region Start

        internal void Start(object operationId, object message, Guid scopeId)
        {
            var operation = new TraceOperation()
            {
                OperationId = operationId ?? $"{this.Context.OperationStack.Count}", //TraceUtility.GetOperationId(),
                ScopeId = scopeId,
                StartedDate = DateTime.Now,
            };
            this.Context.OperationStack.Push(operation);

            if (this.Settings.Filter.Contains(TraceAction.Start))
            {
                RaiseTrace(operation.StartedDate, TraceAction.Start, message ?? operation.OperationId);
            }

            if (this.Settings.Filter.Contains(TraceAction.Calling))
            {
                var sb = new StringBuilder(this.Context.CurrentOperation.OperationId.ToString());
                this.Context.Operations.Skip(1).Aggregate(sb, (x, y) => x.Insert(0, y.OperationId.ToString() + ">"));
                RaiseTrace(operation.StartedDate, TraceAction.Calling, sb);
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
                if (this.Context.OperationStack.Count == 0) { break; }

                var currentOperation = this.Context.OperationStack.Peek();
                if (!byScope)
                {
                    if (currentOperation.UseScope) { break; }
                }

                var stopedDateTime = DateTime.Now;
                if (this.Settings.Filter.Contains(TraceAction.Elapsed))
                {
                    RaiseTrace(stopedDateTime, TraceAction.Elapsed, (stopedDateTime - currentOperation.StartedDate));
                }
                if (this.Settings.Filter.Contains(TraceAction.Stop))
                {
                    RaiseTrace(stopedDateTime, TraceAction.Stop, message ?? currentOperation.OperationId);
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
        public void Stop()
        {
            Stop(null, Guid.Empty);
        }

        #endregion
    }
}

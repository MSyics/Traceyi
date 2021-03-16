using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースイベントデータを格納します。
    /// </summary>
    public sealed class TraceEventArgs : EventArgs
    {
        #region Static members
        private static readonly int processId;
        private static readonly string processName;
        private static readonly string machineName = Environment.MachineName;

        static TraceEventArgs()
        {
            using var process = Process.GetCurrentProcess();
            processId = process.Id;
            processName = process.ProcessName;
        }
        #endregion

        private readonly Action<dynamic> extensions;
        private readonly object messageLayout;

        public TraceEventArgs(TraceOperation operation, DateTimeOffset traced, TraceAction action, object message, Action<dynamic> extensions = default)
        {
            Traced = traced;
            Action = action;
            Operation = operation;
            Elapsed = operation.ScopeNumber == 0 || action == TraceAction.Start ? TimeSpan.Zero : traced - operation.Started;

            this.extensions = extensions;
            if (extensions is null)
            {
                Message = message;
            }
            else
            {
                messageLayout = message;
            }
        }

        sealed class ExtensionsObject : DynamicObject
        {
            public readonly Dictionary<string, object> Items = new();

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return Items.TryGetValue(binder.Name, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                Items[binder.Name] = value;
                return true;
            }
        }

        [JsonExtensionData]
        public IDictionary<string, object> Extensions
        {
            get
            {
                if (_extensions == null)
                {
                    var obj = new ExtensionsObject();
                    _extensions = obj.Items;
                    try
                    {
                        extensions?.Invoke(obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"{ex}");
                    }
                }
                return _extensions;
            }
        }
        private IDictionary<string, object> _extensions;

        /// <summary>
        /// トレース操作を取得します。
        /// </summary>
        //[JsonIgnore]
        public TraceOperation Operation { get; }

        /// <summary>
        /// トレースした日時を取得または設定します。
        /// </summary>
        public DateTimeOffset Traced { get; }

        /// <summary>
        /// トレースの動作を取得または設定します。
        /// </summary>
        public TraceAction Action { get; }

        /// <summary>
        /// メッセージを取得します。
        /// </summary>
        public object Message
        {
            get
            {
                if (_message == null && messageLayout != null)
                {
                    var parts = Extensions.
                        Select(x => new LogLayoutPart
                        {
                            Name = x.Key,
                            CanFormat = true
                        }).
                        ToArray();

                    try
                    {
                        var format = new LogLayoutConverter(parts).Convert(messageLayout.ToString());
                        _message = string.Format(
                            new LogLayoutFormatProvider(),
                            format,
                            Extensions.Values.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"{ex}");
                        _message = messageLayout;
                    }
                }
                return _message;
            }
            set => _message = value;
        }
        private object _message;

        /// <summary>
        /// 経過時間を取得または設定します。
        /// </summary>
        public TimeSpan Elapsed { get; }

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
    }
}

using MSyics.Traceyi.Layout;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi;

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

    private readonly TraceScope scope;
    private readonly Action<dynamic> extensions;
    private readonly object messageLayout;

    public TraceEventArgs(TraceScope scope, DateTimeOffset traced, TraceAction action, object message, Action<dynamic> extensions = default)
    {
        Traced = traced;
        Action = action;
        this.scope = scope;
        Elapsed = scope.Depth is 0 || action is TraceAction.Start ? TimeSpan.Zero : traced - scope.Started;

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

    /// <summary>
    /// トレースの動作を取得または設定します。
    /// </summary>
    public TraceAction Action { get; }

    /// <summary>
    /// トレースした日時を取得または設定します。
    /// </summary>
    public DateTimeOffset Traced { get; }

    /// <summary>
    /// 経過時間を取得または設定します。
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// スレッドに関連付けられた一意な識別子を取得します。
    /// </summary>
    public object ActivityId { get; } = Traceable.Context.ActivityId;

    /// <summary>
    /// スコープ ID を取得します。
    /// </summary>
    public string ScopeId => scope?.Id;

    /// <summary>
    /// 親スコープ ID を取得します。
    /// </summary>
    public string ScopeParentId => scope?.ParentId;

    /// <summary>
    /// スコープの深さを取得します。
    /// </summary>
    public int ScopeDepth => scope?.Depth ?? default;

    /// <summary>
    /// スコープラベルを取得します。
    /// </summary>
    public object ScopeLabel => scope?.Label;

    /// <summary>
    /// マネージスレッドの一意な識別子を取得します。
    /// </summary>
    public int ThreadId { get; } = Environment.CurrentManagedThreadId;

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
    public object Message
    {
        get
        {
            if (_message is null && messageLayout is not null)
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
                    Debug.WriteLine(ex);
                    _message = messageLayout;
                }
            }
            return _message;
        }
        set => _message = value;
    }
    private object _message;

    /// <summary>
    /// 拡張プロパティを取得します。
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object> Extensions
    {
        get
        {
            if (_extensions is null)
            {
                var obj = new DictionaryedDynamicObject();
                try
                {
                    extensions?.Invoke(obj);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                _extensions = obj.Members;
            }
            return _extensions;
        }
    }
    private IDictionary<string, object> _extensions;
}

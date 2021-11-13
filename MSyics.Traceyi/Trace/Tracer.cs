namespace MSyics.Traceyi;

/// <summary>
/// トレースするクラスです。
/// </summary>
public sealed class Tracer
{
    internal Tracer() { }

    /// <summary>
    /// スレッドに関連付いたトレース基本情報を取得します。
    /// </summary>
    public TraceContext Context => _context;
    readonly TraceContext _context = Traceable.Context;

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
    public event EventHandler<TraceEventArgs> Tracing;

    internal void RaiseTracing(TraceAction action, object message, Action<dynamic> extensions)
    {
        if (!Filters.Contains(action)) return;

        Tracing?.Invoke(this, new TraceEventArgs(Context.CurrentScope, DateTimeOffset.Now, action, message, extensions));
    }

    internal void RaiseTracing(TraceScope scope, DateTimeOffset traced, TraceAction action, object message, Action<dynamic> extensions)
    {
        if (!Filters.Contains(action)) return;

        Tracing?.Invoke(this, new TraceEventArgs(scope, traced, action, message, extensions));
    }

    #region Trace
    /// <summary>
    /// トレースに必要なメッセージを残します。
    /// </summary>
    public void Trace(object message, Action<dynamic> extensions = null) => RaiseTracing(TraceAction.Trace, message, extensions);

    /// <summary>
    /// トレースに必要なメッセージを残します。
    /// </summary>
    public void Trace(Action<dynamic> extensions) => Trace(null, extensions);
    #endregion

    #region Debug
    /// <summary>
    /// デバッグに必要なメッセージを残します。
    /// </summary>
    public void Debug(object message, Action<dynamic> extensions = null) => RaiseTracing(TraceAction.Debug, message, extensions);

    /// <summary>
    /// デバッグに必要なメッセージを残します。
    /// </summary>
    public void Debug(Action<dynamic> extensions) => Debug(null, extensions);
    #endregion

    #region Information
    /// <summary>
    /// 通知メッセージを残します。
    /// </summary>
    public void Information(object message, Action<dynamic> extensions = null) => RaiseTracing(TraceAction.Info, message, extensions);

    /// <summary>
    /// 通知メッセージを残します。
    /// </summary>
    public void Information(Action<dynamic> extensions) => Information(null, extensions);
    #endregion

    #region Warning
    /// <summary>
    /// 注意メッセージを残します。
    /// </summary>
    public void Warning(object message, Action<dynamic> extensions = null) => RaiseTracing(TraceAction.Warning, message, extensions);

    /// <summary>
    /// 注意メッセージを残します。
    /// </summary>
    public void Warning(Action<dynamic> extensions) => Warning(null, extensions);
    #endregion

    #region Error
    /// <summary>
    /// エラーメッセージを残します。
    /// </summary>
    public void Error(object message, Action<dynamic> extensions = null) => RaiseTracing(TraceAction.Error, message, extensions);

    /// <summary>
    /// エラーメッセージを残します。
    /// </summary>
    public void Error(Action<dynamic> extensions) => Error(null, extensions);
    #endregion

    #region Critical
    /// <summary>
    /// 重大メッセージを残します。
    /// </summary>
    public void Critical(object message, Action<dynamic> extensions = null) => RaiseTracing(TraceAction.Critical, message, extensions);

    /// <summary>
    /// 重大メッセージを残します。
    /// </summary>
    public void Critical(Action<dynamic> extensions) => Critical(null, extensions);
    #endregion

    #region Start
    /// <summary>
    /// 操作スコープの開始メッセージを残します。
    /// </summary>
    public void Start(object message, Action<dynamic> extensions = null, object label = null) => Start(message, extensions, label, false);

    /// <summary>
    /// 操作スコープの開始メッセージを残します。
    /// </summary>
    public void Start(Action<dynamic> extensions = null, object label = null) => Start(null, extensions, label, false);

    internal string Start(object message, Action<dynamic> extensions, object label, bool withEntry)
    {
        var scope = new TraceScope(withEntry)
        {
            Label = label ?? Context.CurrentScope.Label,
            Id = $"{DateTimeOffset.Now.Ticks:x16}",
            ParentId = Context.CurrentScope.Id,
            Depth = Context.ScopeStack.Count + 1,
            Started = DateTimeOffset.Now,
        };

        Context.ScopeStack.Push(scope);
        RaiseTracing(scope, scope.Started, TraceAction.Start, message, extensions);
        return scope.Id;
    }
    #endregion

    #region Stop
    /// <summary>
    /// 操作スコープの終了メッセージを残します。
    /// </summary>
    public void Stop(object message, Action<dynamic> extensions = null)
    {
        var scope = Context.CurrentScope;

        RaiseTracing(scope, DateTimeOffset.Now, TraceAction.Stop, message, extensions);

        if (scope.WithEntry) return;
        if (Context.ScopeStack.Count == 0) return;

        Context.ScopeStack.TryPop();
    }

    /// <summary>
    /// 操作スコープの終了メッセージを残します。
    /// </summary>
    public void Stop(Action<dynamic> extensions = null) => Stop(null, extensions);

    internal void Stop(string scopeId, DateTimeOffset stopped, object message, Action<dynamic> extensions)
    {
        while (true)
        {
            var scope = Context.CurrentScope;

            if (scopeId == scope.Id)
            {
                RaiseTracing(scope, stopped, TraceAction.Stop, message, extensions);
            }
            else
            {
                RaiseTracing(scope, stopped, TraceAction.Stop, null, null);
            }

            if (Context.ScopeStack.Count == 0) break;
            
            Context.ScopeStack.TryPop();
            
            if (scopeId == scope.Id) break;
        }
    }
    #endregion
}

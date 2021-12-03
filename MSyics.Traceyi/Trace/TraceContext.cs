namespace MSyics.Traceyi;

/// <summary>
/// トレース基本情報を表します。
/// </summary>
public sealed class TraceContext
{
    readonly AsyncLocal<AsyncLocalStackNode<TraceScope>> scopeStackNode = new();
    readonly AsyncLocal<object> activityId = new();

    internal TraceContext()
    {
        ScopeStack = new AsyncLocalStack<TraceScope>(scopeStackNode);
    }

    internal AsyncLocalStack<TraceScope> ScopeStack { get; private set; }

    /// <summary>
    /// 現在の活動識別子を取得または設定します。
    /// </summary>
    public object ActivityId { get => activityId.Value; set => activityId.Value = value; }

    /// <summary>
    /// 現在の操作スコープ情報を取得します。
    /// </summary>
    public TraceScope CurrentScope => ScopeStack.Count is 0 ? TraceScope.NullScope : ScopeStack.Peek();

    /// <summary>
    /// トレース操作スコープの一覧を取得します。
    /// </summary>
    public TraceScope[] Scopes => ScopeStack.ToArray();

    /// <summary>
    /// トレース基本情報をリフレッシュします。
    /// </summary>
    public void Refresh()
    {
        ActivityId = null;
        ScopeStack.Clear();
    }
}

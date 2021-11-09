namespace MSyics.Traceyi.Layout;

/// <summary>
/// ログの記録項目を表します。
/// </summary>
public sealed class LogLayoutPart
{
    /// <summary>
    /// 項目名称を取得または設定します。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// フォーマット書式を使用できるかどうかを示す値を取得または設定します。
    /// </summary>
    public bool CanFormat { get; set; }
}

﻿namespace MSyics.Traceyi.Configration;

/// <summary>
/// Tracer セクションの要素を表します。
/// </summary>
internal class TracerElement
{
    /// <summary>
    /// 名称を取得または設定します。
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 選別するトレース動作を取得または設定します。
    /// </summary>
    public TraceFilters Filters { get; set; } = TraceFilters.All;

    /// <summary>
    /// Tracer に紐づける Listener の名前一覧を取得または設定します。
    /// </summary>
    public List<string> Listeners { get; set; } = new List<string>();
}

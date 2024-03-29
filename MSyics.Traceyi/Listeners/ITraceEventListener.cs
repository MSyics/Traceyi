﻿namespace MSyics.Traceyi.Listeners;

/// <summary>
/// トレースイベントを処理する機能を提供します。
/// </summary>
public interface ITraceEventListener : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// トレースイベントを処理します。
    /// </summary>
    void OnTracing(object sender, TraceEventArgs e);
}

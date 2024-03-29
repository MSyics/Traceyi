﻿using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System.Diagnostics;

namespace MSyics.Traceyi;

/// <summary>
/// ロギングのための一連の静的メソッドを提供します。
/// </summary>
public static class Traceable
{
    private readonly static HashSet<ITraceEventListener> Listeners = new();
    private readonly static Dictionary<string, Tracer> Tracers = new();
    private readonly static TraceEventListenerElementConfiguration ListenerConfig = new();

    internal static TraceContext Context => _context.Value;
    private static readonly Lazy<TraceContext> _context = new(() => new TraceContext(), true);

    /// <summary>
    /// 構成ファイルで設定した Tracer オブジェクトを取得します。
    /// </summary>
    /// <param name="name">取得する Tracer オブジェクトの名前</param>
    public static Tracer Get(string name = "")
    {
        if (Tracers.TryGetValue(name.ToUpperInvariant(), out var value)) return value;
        if (Tracers.TryGetValue("", out var @default)) return @default;
        return new Tracer();
    }

    /// <summary>
    /// 終了処理を行います。
    /// </summary>
    public static void Shutdown()
    {
        try
        {
            Task.WhenAll(Listeners.Select(x => x.DisposeAsync().AsTask())).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        Listeners.Clear();
        Tracers.Clear();
        Context.Refresh();
    }

    #region Configuration
    /// <summary>
    /// Tracer オブジェクトを登録します。
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="filters">選別するトレース動作</param>
    /// <param name="listeners">トレース情報のリスナー</param>
    public static void Add(string name = "", TraceFilters filters = TraceFilters.All, params ITraceEventListener[] listeners)
    {
        var tracer = new Tracer
        {
            Name = name,
            Filters = filters,
        };

        foreach (var item in listeners)
        {
            if (!Listeners.Contains(item))
            {
                Listeners.Add(item);
            }

            tracer.Tracing += item.OnTracing;
        }

        Tracers.Add(name.ToUpperInvariant(), tracer);
    }

    /// <summary>
    /// Tracer オブジェクトを登録します。
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="filters">選別するトレース動作</param>
    /// <param name="listeners">トレース情報のリスナー</param>
    public static void Add(string name = "", TraceFilters filters = TraceFilters.All, params Action<TraceEventArgs>[] listeners) =>
        Add(name, filters, listeners.Select(x => new ActionTraceEventListener(x)).ToArray());

    /// <summary>
    /// Tracer オブジェクトを構成情報から登録します。
    /// </summary>
    /// <param name="configuration">構成情報</param>
    /// <param name="usable">カスタムリスナーを登録することで構成情報からリスナーオブジェクトを取得できるようにします。</param>
    public static void Add(IConfiguration configuration, Action<ITraceEventListenerElementConfiguration> usable = null)
    {
        if (configuration is null) return;

        var tracerSection = configuration.GetSection("Traceyi:Tracer");
        if (!tracerSection.Exists()) return;

        var listenerSection = configuration.GetSection("Traceyi:Listener");
        if (!listenerSection.Exists()) return;

        usable?.Invoke(ListenerConfig);

        var listenerSource = listenerSection.GetChildren().
            SelectMany(section => ListenerConfig[section.Key.ToUpperInvariant()](section)).
            ToDictionary(listener => listener.Name.ToUpperInvariant(), listener => listener.GetRuntimeObject());

        foreach (var element in tracerSection.Get<List<TracerElement>>())
        {
            var listeners = element.Listeners.
                Select(name => name.ToUpperInvariant()).
                Where(name => listenerSource.ContainsKey(name)).
                Select(name => listenerSource[name]).
                ToArray();

            Add(element.Name, element.Filters, listeners);
        }
    }

    /// <summary>
    /// Tracer オブジェクトを構成情報から登録します。
    /// </summary>
    /// <param name="jsonFile">JSON ファイルのパス</param>
    /// <param name="usable">カスタムリスナーを登録することで構成情報からリスナーオブジェクトを取得できるようにします。</param>
    public static void Add(string jsonFile, Action<ITraceEventListenerElementConfiguration> usable = null)
    {
        try
        {
            Add(new ConfigurationBuilder().AddJsonFile(jsonFile, false, true).Build(), usable);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
    #endregion
}

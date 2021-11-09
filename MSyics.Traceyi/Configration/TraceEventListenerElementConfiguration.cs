using Microsoft.Extensions.Configuration;

namespace MSyics.Traceyi.Configration;

/// <summary>
/// トレースリスナー要素を構成する機能を提供します。
/// </summary>
public interface ITraceEventListenerElementConfiguration
{
    void In<T>(string name) where T : TraceEventListenerElement;
}

internal sealed class TraceEventListenerElementConfiguration : Dictionary<string, Func<IConfiguration, IEnumerable<TraceEventListenerElement>>>, ITraceEventListenerElementConfiguration
{
    public TraceEventListenerElementConfiguration()
    {
        In<ConsoleLoggerElement>("Console");
        In<FileLoggerElement>("File");
    }

    /// <summary>
    /// カスタム Listener 要素を登録します。
    /// </summary>
    /// <typeparam name="T">カスタム要素の型</typeparam>
    /// <param name="name">セクション名</param>
    public void In<T>(string name) where T : TraceEventListenerElement
    {
        var upperName = name.ToUpperInvariant();
        if (ContainsKey(upperName)) return;
        Add(upperName, config => config.Get<List<T>>());
    }
}

using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ロギングのための一連の静的メソッドを提供します。
    /// </summary>
    public static class Traceable
    {
        [ThreadStatic]
        private static TraceContext ThreadContext;
        private readonly static Dictionary<string, (Tracer tracer, ITraceListener[] listeners)> Tracers = new Dictionary<string, (Tracer tracer, ITraceListener[] listeners)>();
        private readonly static TraceListenerElementConfiguration TraceListenerElementConfiguration = new TraceListenerElementConfiguration();

        internal static TraceContext Context => ThreadContext ?? (ThreadContext = new TraceContext());

        /// <summary>
        /// 構成ファイルで設定した Tracer オブジェクトを取得します。
        /// </summary>
        /// <param name="name">取得する Tracer オブジェクトの名前</param>
        public static Tracer Get(string name = "") => Tracers.TryGetValue(name.ToUpper(), out var x) ? x.tracer : new Tracer();

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public static void Shutdown()
        {
            Task.WhenAll(Tracers.SelectMany(x => x.Value.listeners)
                                .Select(x => Task.Run(() => x.Dispose()))
                                .ToArray())
                .Wait();
            Tracers.Clear();
        }

        #region Configuration

        /// <summary>
        /// Tracer オブジェクトを登録します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="filters">選別するトレース動作</param>
        /// <param name="useMemberInfo">クラスメンバー情報を取得するかどうかを示す値</param>
        /// <param name="listeners">トレース情報のリスナー</param>
        public static void Add(string name = "", TraceFilters filters = TraceFilters.All, bool useMemberInfo = true, params ITraceListener[] listeners)
        {
            var tracer = new Tracer
            {
                Name = name,
                Filters = filters,
                UseMemberInfo = useMemberInfo
            };
            foreach (var item in listeners)
            {
                tracer.Tracing += item.OnTracing;
            }
            Tracers[name.ToUpper()] = (tracer, listeners);
        }

        /// <summary>
        /// Tracer オブジェクトを登録します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="filters">選別するトレース動作</param>
        /// <param name="useMemberInfo">クラスメンバー情報を取得するかどうかを示す値</param>
        /// <param name="listeners">トレース情報のリスナー</param>
        public static void Add(string name = "", TraceFilters filters = TraceFilters.All, bool useMemberInfo = true, params Action<TraceEventArg>[] listeners)
        {
            Add(name, filters, useMemberInfo, listeners.Select(x => new ActionTraceListener(x)).ToArray());
        }

        /// <summary>
        /// Tracer オブジェクトを構成情報から登録します。
        /// </summary>
        /// <param name="configuration">構成情報</param>
        /// <param name="usable">カスタムリスナーを登録することで構成情報からリスナーオブジェクトを取得できるようにします。</param>
        public static void Add(IConfiguration configuration, Action<ITraceListenerElementConfiguration> usable = null)
        {
            if (configuration == null) return;

            var ts = configuration.GetSection("Traceyi:Tracer");
            if (!ts.Exists()) return;

            var ls = configuration.GetSection("Traceyi:Listener");
            if (!ls.Exists()) return;

            usable?.Invoke(TraceListenerElementConfiguration);

            foreach (var te in ts.Get<List<TracerElement>>())
            {
                Add(te.Name,
                    te.Filters,
                    te.UseMemberInfo,
                    ls.GetChildren()
                      .Select(x => new { Name = x.Key.ToUpper(), Value = x })
                      .Where(x => TraceListenerElementConfiguration.ContainsKey(x.Name))
                      .SelectMany(x => TraceListenerElementConfiguration[x.Name](x.Value))
                      .Where(x => te.Listeners.Exists(y => x.Name?.ToUpper() == y.ToUpper()))
                      .Select(x => x.GetRuntimeObject())
                      .ToArray());
            }
        }

        /// <summary>
        /// Tracer オブジェクトを構成情報から登録します。
        /// </summary>
        /// <param name="jsonFile">JSON ファイルのパス</param>
        /// <param name="usable">カスタムリスナーを登録することで構成情報からリスナーオブジェクトを取得できるようにします。</param>
        public static void Add(string jsonFile, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Add(new ConfigurationBuilder().AddJsonFile(jsonFile, false, true).Build(), usable);
        }

        #endregion
    }
}

/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ロギングのための一連の静的メソッドを提供します。
    /// </summary>
    public static class Traceable
    {
        /// <summary>
        /// トレース基本情報を取得します。
        /// </summary>
        internal static TraceContext Context => _context ?? (_context = new TraceContext());
        [ThreadStatic]
        private static TraceContext _context;

        private static Dictionary<string, Tracer> Tracers { get; } = new Dictionary<string, Tracer>();

        /// <summary>
        /// 構成ファイルで設定した Tracer オブジェクトを取得します。
        /// </summary>
        /// <param name="name">取得する Tracer オブジェクトの名前</param>
        public static Tracer Get(string name = "")
        {
            return Tracers.TryGetValue(name.ToUpper(), out var tracer) ? tracer : new Tracer();
        }

        #region Configuration

        private static TraceListenerElementConfiguration TraceListenerElementConfiguration { get; } = new TraceListenerElementConfiguration();

        /// <summary>
        /// Tracer オブジェクトを登録します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="filters">選別するトレース動作</param>
        /// <param name="useMemberInfo">クラスメンバー情報を取得するかどうかを示す値</param>
        /// <param name="listeners">トレース情報のリスナー</param>
        public static void Add(string name = "", TraceFilters filters = TraceFilters.All, bool useMemberInfo = true, params ITraceListener[] listeners)
        {
            var tracer = new Tracer();
            tracer.Name = name;
            tracer.Filters = filters;
            tracer.UseMemberInfo = useMemberInfo;
            foreach (var item in listeners)
            {
                tracer.Tracing += item.OnTracing;
            }
            Tracers[name.ToUpper()] = tracer;
        }

        /// <summary>
        /// Tracer オブジェクトを登録します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="filters">選別するトレース動作</param>
        /// <param name="useMemberInfo">クラスメンバー情報を取得するかどうかを示す値</param>
        /// <param name="listeners">トレース情報のリスナー/param>
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
                      .Select(x => x.GetRuntimeObject()).ToArray());
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

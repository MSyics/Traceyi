/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using MSyics.Traceyi.Listeners;

namespace MSyics.Traceyi
{
    public sealed class TraceListenerConfiguration
    {
        internal Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>> SectionedListenerElements { get; } =
            new Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>>();

        public TraceListenerConfiguration()
        {
            AddSectionedListenerElement<ConsoleElement>("Console");
            AddSectionedListenerElement<FileElement>("File");
        }

        /// <summary>
        /// カスタム Listener 要素を登録します。
        /// </summary>
        /// <typeparam name="T">カスタム Log 要素の型</typeparam>
        /// <param name="section">セクション名</param>
        public void AddSectionedListenerElement<T>(string section)
            where T : ListenerElement
        {
            SectionedListenerElements.Add(
                section.ToUpper(),
                config => config.Get<List<T>>());
        }
    }

    /// <summary>
    /// ロギングのための一連の静的メソッドを提供します。
    /// </summary>
    public static class Traceable
    {
        private static ConcurrentDictionary<string, Tracer> Tracers { get; } = new ConcurrentDictionary<string, Tracer>();

        #region Configuration

        public static TraceListenerConfiguration Config { get; } = new TraceListenerConfiguration();

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
            Tracers.AddOrUpdate(name.ToUpper(), tracer, (x, y) => tracer);
        }

        public static void Add(string name = "", TraceFilters filters = TraceFilters.All, bool useMemberInfo = true, params Action<TraceEventArg>[] listeners)
        {
            Add(name, filters, useMemberInfo, listeners.Select(x => new ActionTraceListener(x)).ToArray());
        }

        public static void Add(IConfiguration configuration)
        {
            if (configuration == null) return;

            var ts = configuration.GetSection("Traceyi:Tracer");
            if (!ts.Exists()) return;

            var ls = configuration.GetSection("Traceyi:Listener");
            if (!ls.Exists()) return;

            foreach (var te in ts.Get<List<TracerElement>>())
            {
                Add(te.Name,
                    te.Filters,
                    te.UseMemberInfo,
                    ls.GetChildren()
                      .Select(x => new { Name = x.Key.ToUpper(), Value = x })
                      .Where(x => Config.SectionedListenerElements.ContainsKey(x.Name))
                      .SelectMany(x => Config.SectionedListenerElements[x.Name](x.Value))
                      .Where(x => te.Listeners.Exists(y => x.Name?.ToUpper() == y.ToUpper()))
                      .Select(x => x.GetRuntimeObject()).ToArray());
            }
        }

        public static void Add(string jsonFile)
        {
            var config = new ConfigurationBuilder().AddJsonFile(jsonFile, false, true)
                                                   .Build();
            Add(config);
        }

        #endregion

        /// <summary>
        /// 構成ファイルで設定した Tracer オブジェクトを取得します。
        /// </summary>
        /// <param name="name">取得する Tracer オブジェクトの名前</param>
        public static Tracer Get(string name = "")
        {
            return Tracers.TryGetValue(name.ToUpper(), out var tracer) ? tracer : new Tracer();
        }

        #region TraceContext

        /// <summary>
        /// トレース基本情報を取得します。
        /// </summary>
        internal static TraceContext Context => _context ?? (_context = new TraceContext());
        [ThreadStatic]
        private static TraceContext _context;

        #endregion
    }
}

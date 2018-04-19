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

namespace MSyics.Traceyi
{
    /// <summary>
    /// ロギングのための一連の静的メソッドを提供します。
    /// </summary>
    public static class Traceable
    {
        private static ConcurrentDictionary<string, Tracer> Tracers { get; } = new ConcurrentDictionary<string, Tracer>();
        private static Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>> SectionedListenersElements { get; } = new Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>>();

        static Traceable()
        {
            AddSectionedListenerElement<ConsoleElement>("Console");
            AddSectionedListenerElement<FileElement>("File");
        }

        #region Configuration

        /// <summary>
        /// カスタム Listener 要素を登録します。
        /// </summary>
        /// <typeparam name="T">カスタム Log 要素の型</typeparam>
        /// <param name="section">セクション名</param>
        public static void AddSectionedListenerElement<T>(string section)
            where T : ListenerElement
        {
            SectionedListenersElements.Add(section.ToUpper(), (config) => config.Get<List<T>>());
        }

        public static void AddConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
            var i = Configuration.GetReloadToken();
            i.RegisterChangeCallback(_ =>
            {
            }, null);
        }

        private static IConfiguration Configuration { get; set; }

        #endregion

        #region Creation

        /// <summary>
        /// Tracer オブジェクトを構築します。
        /// </summary>
        public static IBuildTracerSettings Build() => new TracerCreation();

        /// <summary>
        /// 構成ファイルで設定した Tracer オブジェクトを取得します。
        /// </summary>
        /// <param name="name">取得する Tracer オブジェクトの名前</param>
        public static Tracer Get(string name = "")
        {
            return Tracers.GetOrAdd(name, Create(name));
        }

        private static Tracer Create(string name)
        {
            if (Configuration == null) return CreateNullTracer();

            var tracerSection = Configuration.GetSection("Traceyi:Tracer");
            if (!tracerSection.Exists()) return CreateNullTracer();

            var listenerSection = Configuration.GetSection("Traceyi:Listener");
            if (!listenerSection.Exists()) return CreateNullTracer();

            // Get Tracer Element
            var tracerElement = tracerSection.Get<List<TracerElement>>()
                                             .FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());
            if (tracerElement == null) return CreateNullTracer();
            // Add Listener RuntimeObject
            var listeners = listenerSection.GetChildren()
                                           .Where(x => SectionedListenersElements.ContainsKey(x.Key.ToUpper()))
                                           .SelectMany(x => SectionedListenersElements[x.Key.ToUpper()](x))
                                           .Where(x => tracerElement.Listeners.Exists(y => x.Name?.ToUpper() == y.ToUpper()))
                                           .Select(x => x.GetRuntimeObject());
            // Build Tracer
            return Build().Settings(x =>
                          {
                              x.Name = name;
                              x.Filter = tracerElement.Filters;
                              x.UseMemberInfo = tracerElement.UseMemberInfo;
                          })
                          .Attach(listeners.ToArray())
                          .Get();
        }

        private static Tracer CreateNullTracer() => new Tracer();

        #endregion

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

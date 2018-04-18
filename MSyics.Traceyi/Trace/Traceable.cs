/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
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
        private static object LockObj { get; } = new object();
        private static Dictionary<string, Tracer> Tracers { get; } = new Dictionary<string, Tracer>();
        private static Dictionary<string, ListenerElement> Listeners { get; } = new Dictionary<string, ListenerElement>();
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
        }

        private static IConfiguration Configuration { get; set; }

        #endregion

        #region Creation

        /// <summary>
        /// Tracer オブジェクトを構築します。
        /// </summary>
        public static IBuildTracerSettings Build() => new BuildTracer();

        /// <summary>
        /// 構成ファイルで設定した Tracer オブジェクトを取得します。
        /// </summary>
        /// <param name="name">取得する Tracer オブジェクトの名前</param>
        public static Tracer Get(string name)
        {
            lock (LockObj)
            {
                Tracer tracer;
                if (!(Tracers.TryGetValue(name, out tracer)))
                {
                    tracer = Create(name);
                    Tracers[name] = tracer;
                }
                return tracer;
            }
        }

        /// <summary>
        /// 構成ファイルで設定した Default Tracer オブジェクトを取得します。
        /// </summary>
        public static Tracer Get() => Get("");

        private static Tracer Create(string name)
        {
            if (Configuration == null) return CreateNullTracer();
            if (!Configuration.GetSection("Traceyi").Exists()) return CreateNullTracer();
            if (!Configuration.GetSection("Traceyi:Tracer").Exists()) return CreateNullTracer();
            if (!Configuration.GetSection("Traceyi:Listener").Exists()) return CreateNullTracer();

            // Get Tracer Element
            var tracerElement = Configuration.GetSection("Traceyi:Tracer")
                                             .Get<List<TracerElement>>()
                                             .FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());
            if (tracerElement == null) return CreateNullTracer();

            // Add Listener RuntimeObject
            foreach (var listenersSection in Configuration.GetSection("Traceyi:Listener").GetChildren())
            {
                var listenersSectionName = listenersSection.Key.ToUpper();
                if (!SectionedListenersElements.ContainsKey(listenersSectionName)) continue;
                foreach (var listener in SectionedListenersElements[listenersSectionName](listenersSection))
                {
                    var listenerName = listener.Name.ToUpper();
                    if (string.IsNullOrWhiteSpace(listenerName)) continue;
                    if (Listeners.ContainsKey(listenerName)) continue;
                    Listeners.Add(listenerName, listener);
                }
            }

            return Build().Settings(x =>
                          {
                              x.Name = name;
                              x.Filter = tracerElement.Filters;
                              x.UseMemberInfo = tracerElement.UseMemberInfo;
                          })
                          .Attach(Listeners.Where(x => tracerElement.Listeners.Exists(y => x.Key.ToUpper() == y.ToUpper()))
                          .Select(x => x.Value.GetRuntimeObject()).ToArray())
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

using System;
using System.Collections.Generic;
using System.Linq;
using MSyics.Traceyi.Configuration;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ロギングのための一連の静的メソッドを提供します。
    /// </summary>
    public static class Traceable
    {
        private static readonly object m_thisLock = new object();
        private static Dictionary<string, Tracer> Tracers = new Dictionary<string, Tracer>();
        private static Dictionary<string, Log> Logs = new Dictionary<string, Log>();

        /// <summary>
        /// 構成ファイルで設定した Tracer オブジェクトを取得します。
        /// </summary>
        /// <param name="name">取得する Tracer オブジェクトの名前</param>
        public static Tracer Get(string name)
        {
            lock (m_thisLock)
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
        public static Tracer Get()
        {
            return Get("Default");
        }

        private static Tracer Create(string name)
        {
            if (!TraceyiConfiguration.HasRoot)
            {
                return CreateNullTracer();
            }
            else
            {
                var tracerElement = TraceyiConfiguration.Root.Tracers.SingleOrDefault(x => x.Name == name);
                if (tracerElement == default(TracerElementBase))
                {
                    return CreateNullTracer();
                }
                else
                {
                    return new Tracer().Settings(settings =>
                    {
                        settings.SetProperty(
                            tracerElement.Name,
                            tracerElement.HasFilterName ? TraceyiConfiguration.Root.Filters.Find(tracerElement.FilterName).Value : TraceFilters.All);

                        foreach (var item in tracerElement)
                        {
                            Log log;
                            if (!Logs.TryGetValue(item.LogName, out log))
                            {
                                var logElement = TraceyiConfiguration.Root.Logs.Find(item.LogName);
                                log = logElement.GetRuntimeObject();
                                if (string.IsNullOrEmpty(item.FilterName))
                                {
                                    log.Filter = TraceFilters.All;
                                }
                                else
                                {
                                    log.Filter = TraceyiConfiguration.Root.Filters.Find(item.FilterName).Value;
                                }
                                Logs.Add(item.LogName, log);
                            }
                            settings.SetListener(log);
                        }
                    });
                }
            }
        }

        private static Tracer CreateNullTracer()
        {
            return new Tracer() { Name = "NullTracer", Filter = TraceFilters.None };
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

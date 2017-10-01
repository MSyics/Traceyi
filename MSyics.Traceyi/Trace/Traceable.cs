using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ロギングのための一連の静的メソッドを提供します。
    /// </summary>
    public static class Traceable
    {
        private static readonly object m_thisLock = new object();
        private static Dictionary<string, Tracer> Tracers = new Dictionary<string, Tracer>();
        private static Dictionary<string, LoggingListener> Logs = new Dictionary<string, LoggingListener>();
        private static Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>> SectionedLogElements = new Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>>();

        static Traceable()
        {
            AddSectionedLogElement<ConsoleLoggingListenerElement>("Consoles");
            AddSectionedLogElement<FileLoggingListenerElement>("Files");
            AddSectionedLogElement<RotateFileLoggingListenerElement>("RotateFiles");
        }

        /// <summary>
        /// カスタム Log 要素を登録します。
        /// </summary>
        /// <typeparam name="T">カスタム Log 要素の型</typeparam>
        /// <param name="section">セクション名</param>
        public static void AddSectionedLogElement<T>(string section)
            where T : ListenerElement
        {
            SectionedLogElements.Add(section.ToUpper(), (config) => config.Get<List<T>>());
        }

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
        public static Tracer Get() => Get("Default");

        private static Tracer Create(string name)
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("Traceyi.json", false, true);

            var config = builder.Build();

            if (!config.GetSection("Tracers").Exists()) return CreateNullTracer();
            if (!config.GetSection("Log").Exists()) return CreateNullTracer();

            // Get Tracer Element
            var tracerElement = config.GetSection("Tracers").Get<List<TracerElement>>().FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());
            if (tracerElement == null) return CreateNullTracer();

            // Add Log RuntimeObject
            foreach (var logSection in config.GetSection("Log").GetChildren())
            {
                var logSectionName = logSection.Key.ToUpper();
                if (!SectionedLogElements.ContainsKey(logSectionName)) continue;
                foreach (var log in SectionedLogElements[logSectionName](config.GetSection(logSection.Path)))
                {
                    var logName = log.Name.ToUpper();
                    if (string.IsNullOrWhiteSpace(logName)) continue;
                    if (Logs.ContainsKey(logName)) continue;
                    Logs.Add(logName, log.GetRuntimeObject());
                }
            }

            // Create Tracer
            return new Tracer()
            .Configure(settings =>
            {
                settings.Settings(s =>
                {
                    s.Name = name;
                    s.Filter = tracerElement.Filter;
                });
                foreach (var key in tracerElement.Logs.Select(x => x.ToUpper()))
                {
                    if (!Logs.ContainsKey(key)) { continue; }
                    settings.AddListener(Logs[key]);
                }
            });
        }

        private static Tracer CreateNullTracer() => new Tracer(); 

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

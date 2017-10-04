﻿/****************************************************************
© 2017 MSyics
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
        private static readonly object m_thisLock = new object();
        private static Dictionary<string, Tracer> Tracers = new Dictionary<string, Tracer>();
        private static Dictionary<string, ListenerElement> Listeners = new Dictionary<string, ListenerElement>();
        private static Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>> SectionedListenersElements = new Dictionary<string, Func<IConfiguration, IEnumerable<ListenerElement>>>();

        static Traceable()
        {
            AddSectionedListenerElement<ConsoleLoggingListenerElement>("ConsoleLogging");
            AddSectionedListenerElement<FileLoggingListenerElement>("FileLogging");
            AddSectionedListenerElement<RotateFileLoggingListenerElement>("RotateFileLogging");
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

        #endregion

        #region Creation

        /// <summary>
        /// Tracer オブジェクトを構築します。
        /// </summary>
        public static IBuildSettings Build() => new TracerBuildable();

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

            if (!config.GetSection("Tracer").Exists()) return CreateNullTracer();
            if (!config.GetSection("Listener").Exists()) return CreateNullTracer();

            // Get Tracer Element
            var tracerElement = config.GetSection("Tracer").Get<List<TracerElement>>().FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());
            if (tracerElement == null) return CreateNullTracer();

            // Add Listener RuntimeObject
            foreach (var listenersSection in config.GetSection("Listener").GetChildren())
            {
                var listenersSectionName = listenersSection.Key.ToUpper();
                if (!SectionedListenersElements.ContainsKey(listenersSectionName)) continue;
                foreach (var listener in SectionedListenersElements[listenersSectionName](config.GetSection(listenersSection.Path)))
                {
                    var listenerName = listener.Name.ToUpper();
                    if (string.IsNullOrWhiteSpace(listenerName)) continue;
                    if (Listeners.ContainsKey(listenerName)) continue;
                    Listeners.Add(listenerName, listener);
                }
            }

            return Build()
                .Settings(x =>
                {
                    x.Name = name;
                    x.Filter = tracerElement.Filter;
                    x.UseMemberInfo = tracerElement.UseMemberInfo;
                })
                .Attach(Listeners.Where(x => tracerElement.Listeners.Exists(y => x.Key.ToUpper() == y.ToUpper())).Select(x => x.Value.GetRuntimeObject()).ToArray())
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

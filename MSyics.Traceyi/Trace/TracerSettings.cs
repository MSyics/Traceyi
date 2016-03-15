using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// Tracer オブジェクトの設定を行います。
    /// </summary>
    public sealed class TracerSettings
    {
        Tracer m_source;

        internal TracerSettings()
        {
        }

        /// <summary>
        /// TracerSetting クラスのインスタンスを初期化します。
        /// </summary>
        public TracerSettings(Tracer source)
        {
            m_source = source;
        }

        /// <summary>
        /// トレースイベントに Log オブジェクトを関連付けます。
        /// </summary>
        public TracerSettings SetListener(Log log)
        {
            m_source.OnTrace += log.OnTrace;
            return this;
        }

        /// <summary>
        /// トレースイベントに引数の Action デリゲートを関連付けます。 
        /// </summary>
        public TracerSettings SetListener(Action<TraceEventArg> action)
        {
            m_source.OnTrace += (sender, e) => action(e);
            return this;
        }

        /// <summary>
        /// 各種プロパティを設定します。
        /// </summary>
        public TracerSettings SetProperty(string name, TraceFilters filter)
        {
            m_source.Name = name;
            m_source.Filter = filter;
            return this;
        }
    }
}

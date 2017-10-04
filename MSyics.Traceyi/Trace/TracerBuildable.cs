/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// Tracer オブジェクトの設定を行います。
    /// </summary>
    public sealed class TracerBuildable: IBuildSettings, IBuildListener, IBuildTracer
    {
        Tracer m_source;

        /// <summary>
        /// TracerSetting クラスのインスタンスを初期化します。
        /// </summary>
        internal TracerBuildable() => m_source = new Tracer();

        /// <summary>
        /// トレースイベントに Listener オブジェクトを関連付けます。
        /// </summary>
        public IBuildTracer Attach(params ITraceListener[] listeners)
        {
            foreach (var item in listeners)
            {
                m_source.OnTrace += item.OnTrace;
            }
            return this;
        }

        /// <summary>
        /// トレースイベントに引数の Action デリゲートを関連付けます。 
        /// </summary>
        public IBuildTracer Attach(params Action<TraceEventArg>[] listener)
        {
            foreach (var item in listener)
            {
                m_source.OnTrace += (sender, e) => item(e);
            }
            return this;
        }

        /// <summary>
        /// 各種設定値を設定します。
        /// </summary>
        public IBuildListener Settings(Action<TracerSettings> settings)
        {
            if (settings == null) return this;

            settings(m_source.Settings);
            return this;
        }

        /// <summary>
        /// 構築した Tracer オブジェクトを取得します。
        /// </summary>
        public Tracer Get() => m_source;
    }

    public interface IBuildSettings
    {
        IBuildListener Settings(Action<TracerSettings> settings);
    }

    public interface IBuildListener
    {
        IBuildTracer Attach(params ITraceListener[] listeners);
        IBuildTracer Attach(params Action<TraceEventArg>[] listeners);
    }

    public interface IBuildTracer
    {
        Tracer Get();
    }
}

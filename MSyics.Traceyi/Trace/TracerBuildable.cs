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
    public sealed class TracerBuildable: IBuildSettings, IBuildListener
    {
        Tracer m_source;

        internal TracerBuildable()
        {
        }

        /// <summary>
        /// TracerSetting クラスのインスタンスを初期化します。
        /// </summary>
        public TracerBuildable(Tracer source) => m_source = source;

        /// <summary>
        /// トレースイベントに Listener オブジェクトを関連付けます。
        /// </summary>
        public IBuildListener AddListener(ITraceListener listener)
        {
            m_source.OnTrace += listener.OnTrace;
            return this;
        }

        /// <summary>
        /// トレースイベントに引数の Action デリゲートを関連付けます。 
        /// </summary>
        public IBuildListener AddListener(Action<TraceEventArg> listener)
        {
            m_source.OnTrace += (sender, e) => listener(e);
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
    }

    public interface IBuildSettings
    {
        IBuildListener Settings(Action<TracerSettings> settings);
    }

    public interface IBuildListener
    {
        IBuildListener AddListener(ITraceListener listener);
        IBuildListener AddListener(Action<TraceEventArg> listener);
    }
}

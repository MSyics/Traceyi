using MSyics.Traceyi.Listeners;
using System;
using System.Threading;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Listener 要素の基底クラスです。
    /// </summary>
    public abstract class TraceEventListenerElement
    {
        /// <summary>
        /// ロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = false;

        /// <summary>
        /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得または設定します。
        /// </summary>
        public bool UseAsync { get; set; } = true;

        /// <summary>
        /// 受信イベントの処理分割数を取得または設定します。
        /// </summary>
        public int Divide { get; set; } = 1;

        /// <summary>
        /// 終了を待機する時間間隔を取得または設定します。
        /// </summary>
        public TimeSpan CloseTimeout { get; set; } = Timeout.InfiniteTimeSpan;

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 派生クラスでオーバーライドされると実行オブジェクトを取得します。
        /// </summary>
        public abstract ITraceEventListener GetRuntimeObject();
    }
}
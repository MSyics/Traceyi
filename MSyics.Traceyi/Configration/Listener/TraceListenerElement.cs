using MSyics.Traceyi.Listeners;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Listener 要素の基底クラスです。
    /// </summary>
    public abstract class TraceListenerElement
    {
        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 同時実行数を取得または設定します。
        /// </summary>
        public int Concurrency { get; set; } = 1;

        /// <summary>
        /// 派生クラスでオーバーライドされると実行オブジェクトを取得します。
        /// </summary>
        public abstract ITraceListener GetRuntimeObject();
    }
}
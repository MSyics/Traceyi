namespace MSyics.Traceyi.Configration;

/// <summary>
/// 受信イベントをまとめてから処理する Listener 要素の基底クラスです。
/// </summary>
public abstract class ChunkTraceEventListenerElement : TraceEventListenerElement
{
    /// <summary>
    /// 受信イベントのまとめる量を取得または設定します。
    /// </summary>
    public int Chunk { get; set; } = 1;

    /// <summary>
    /// 収集タイムアウト時間を取得または設定します。
    /// </summary>
    public TimeSpan ChunkTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);
}

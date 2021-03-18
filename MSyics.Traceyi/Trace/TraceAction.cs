namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースの動作を示します。
    /// </summary>
    public enum TraceAction
    {
        /// <summary>
        /// トレース
        /// </summary>
        Trace,

        /// <summary>
        /// デバッグ
        /// </summary>
        Debug,

        /// <summary>
        /// 通知
        /// </summary>
        Info,

        /// <summary>
        /// 注意
        /// </summary>
        Warning,

        /// <summary>
        /// エラー
        /// </summary>
        Error,

        /// <summary>
        /// 重大
        /// </summary>
        Critical,

        /// <summary>
        /// 開始
        /// </summary>
        Start,

        /// <summary>
        /// 終了
        /// </summary>
        Stop,
    }
}

/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースの動作を示します。
    /// </summary>
    public enum TraceAction
    {
        /// <summary>
        /// デバッグ
        /// </summary>
        Debug,

        /// <summary>
        /// 通知
        /// </summary>
        Info,

        /// <summary>
        /// 操作開始
        /// </summary>
        Start,

        /// <summary>
        /// 操作停止
        /// </summary>
        Stop,

        /// <summary>
        /// 注意
        /// </summary>
        Warning,

        /// <summary>
        /// エラー
        /// </summary>
        Error,

        /// <summary>
        /// 実行中の操作
        /// </summary>
        Calling,

        /// <summary>
        /// 経過時間
        /// </summary>
        Elapsed,
    }
}

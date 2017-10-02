/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Listener クラスから派生するクラスを設定する要素を表します。これは抽象クラスです。
    /// </summary>
    public abstract class ListenerElement
    {
        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// グローバルロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseGlobalLock { get; set; }

        /// <summary>
        /// 派生クラスでオーバーライドされると実行オブジェクトを取得します。
        /// </summary>
        public abstract ITraceListener GetRuntimeObject();
    }
}
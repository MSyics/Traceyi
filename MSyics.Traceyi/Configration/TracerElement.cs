/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.Generic;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// tracer セクションの要素を表します。
    /// </summary>
    internal class TracerElement
    {
        /// <summary>
        /// 名称を取得または設定します。
        /// </summary>
        public string Name { get; set; } = "Default";

        /// <summary>
        /// 選別するトレース動作を取得または設定します。
        /// </summary>
        public TraceFilters Filter { get; set; } = TraceFilters.All;

        /// <summary>
        /// クラスメンバー情報を取得するかどうを示す値を取得または設定します。
        /// </summary>
        public bool UseMemberInfo { get; set; } = true;

        /// <summary>
        /// Tracer オブジェクトに紐づける Listener オブジェクトの名前一覧を取得または設定します。
        /// </summary>
        public List<string> Listeners { get; set; } = new List<string>();
    }
}
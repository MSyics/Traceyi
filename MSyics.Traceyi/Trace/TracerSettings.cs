/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// Tracer オブジェクトの設定値を表します。
    /// </summary>
    public sealed class TracerSettings
    {
        /// <summary>
        /// 名前を取得します。
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 選別するトレース動作を取得または設定します。
        /// </summary>
        public TraceFilters Filter { get; set; } = TraceFilters.All;

        /// <summary>
        /// トレースする際にクラスメンバー情報を取得するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseMemberInfo { get; set; } = true;
    }
}

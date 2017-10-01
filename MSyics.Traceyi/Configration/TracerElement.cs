using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        /// Tracer オブジェクトに紐づける Listener オブジェクトの名前一覧を取得または設定します。
        /// </summary>
        public List<string> Logs { get; set; } = new List<string>();
    }

    internal class TraceFigure
    {

    }
}
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
    /// layout 要素を表します。
    /// </summary>
    public class LayoutElement
    {
        /// <summary>
        /// レイアウト形式を取得または設定します。
        /// </summary>
        public string Format { get; set; } = TraceLogLayout.DefaultLayout;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public TraceLogLayout GetRuntimeObject() => new TraceLogLayout(this.Format);
    }
}
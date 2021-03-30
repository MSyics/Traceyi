using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 記録データを表します。
    /// </summary>
    public class LogState
    {
        /// <summary>
        /// 記録データのメンバー一覧を取得または設定します。
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> Members { get; set; }
    }
}

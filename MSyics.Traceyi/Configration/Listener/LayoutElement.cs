using MSyics.Traceyi.Layout;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Layout 要素を表します。
    /// </summary>
    public class LayoutElement
    {
        /// <summary>
        /// レイアウト形式を取得または設定します。
        /// </summary>
        public string Format { get; set; } = LogLayout.DefaultFormat;

        /// <summary>
        /// 記録データのメンバーを取得または設定します。
        /// </summary>
        public LogStateMembers StateMembers { get; set; } = LogStateMembers.All;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public ILogLayout GetRuntimeObject() => new LogLayout(Format)
        {
            StateMembers = StateMembers,
        };
    }
}
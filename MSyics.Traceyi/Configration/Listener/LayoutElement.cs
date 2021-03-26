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

        public bool UseTraced { get; set; } = true;
        public bool UseAction { get; set; } = true;
        public bool UseElapsed { get; set; } = true;
        public bool UseMessage { get; set; } = true;
        public bool UseActivityId { get; set; } = true;
        public bool UseScopeId { get; set; } = true;
        public bool UseScopeParentId { get; set; } = true;
        public bool UseScopeDepth { get; set; } = true;
        public bool UseScopeLabel { get; set; } = true;
        public bool UseThreadId { get; set; } = true;
        public bool UseProcessId { get; set; } = true;
        public bool UseProcessName { get; set; } = true;
        public bool UseMachineName { get; set; } = true;
        public bool UseExtensions { get; set; } = true;
        public bool UsePartValueSet { get; set; } = true;


        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public ILogLayout GetRuntimeObject() => new LogLayout(Format)
        {
            UseTraced = UseTraced,
            UseAction = UseAction,
            UseElapsed = UseElapsed,
            UseMessage = UseMessage,
            UseActivityId = UseActivityId,
            UseScopeId = UseScopeId,
            UseScopeParentId = UseScopeParentId,
            UseScopeDepth = UseScopeDepth,
            UseScopeLabel = UseScopeLabel,
            UseThreadId = UseThreadId,
            UseProcessId = UseProcessId,
            UseProcessName = UseProcessName,
            UseMachineName = UseMachineName,
            UseExtensions = UseExtensions,
            UsePartValueSet = UsePartValueSet,
        };
    }
}
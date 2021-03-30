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
        /// トレースした日時を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseTraced { get; set; } = true;

        /// <summary>
        /// トレースの動作を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseAction { get; set; } = true;

        /// <summary>
        /// 経過時間を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseElapsed { get; set; } = true;

        /// <summary>
        /// メッセージを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseMessage { get; set; } = true;

        /// <summary>
        /// スレッドに関連付けられた一意な識別子を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseActivityId { get; set; } = true;

        /// <summary>
        /// スコープ ID を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseScopeId { get; set; } = true;

        /// <summary>
        /// 親スコープ ID を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseScopeParentId { get; set; } = true;

        /// <summary>
        /// スコープの深さを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseScopeDepth { get; set; } = true;

        /// <summary>
        /// スコープラベルを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseScopeLabel { get; set; } = true;

        /// <summary>
        /// スレッド ID を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseThreadId { get; set; } = true;

        /// <summary>
        /// プロセス ID を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseProcessId { get; set; } = true;

        /// <summary>
        /// プロセス名を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseProcessName { get; set; } = true;

        /// <summary>
        /// マシン名を使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseMachineName { get; set; } = true;

        /// <summary>
        /// 拡張プロパティを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseExtensions { get; set; } = true;

        /// <summary>
        /// パーツセットを使用するかどうかを示す値を取得または設定します。
        /// </summary>
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
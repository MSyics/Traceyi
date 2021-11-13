namespace MSyics.Traceyi.Configration;

/// <summary>
/// Console 出力文字の色付け設定を表します。
/// </summary>
public record ConsoleColoringSettings()
{
    /// <summary>
    /// 色付け開始位置を取得または設定します。
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// 色付けする文字の長さを取得または設定します。
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Trace の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForTrace { get; set; } = ConsoleColor.DarkGreen;

    /// <summary>
    /// Debug の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForDebug { get; set; } = ConsoleColor.DarkGray;

    /// <summary>
    /// Information の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForInfo { get; set; } = ConsoleColor.White;

    /// <summary>
    /// Warning の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForWarning { get; set; } = ConsoleColor.DarkYellow;

    /// <summary>
    /// Error の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForError { get; set; } = ConsoleColor.Red;

    /// <summary>
    /// Critical の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForCritical { get; set; } = ConsoleColor.DarkRed;

    /// <summary>
    /// Start の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForStart { get; set; } = ConsoleColor.DarkCyan;

    /// <summary>
    /// Stop の色を取得または設定します。
    /// </summary>
    public ConsoleColor ForStop { get; set; } = ConsoleColor.DarkCyan;
}

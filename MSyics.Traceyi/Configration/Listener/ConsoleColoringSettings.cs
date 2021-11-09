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
}

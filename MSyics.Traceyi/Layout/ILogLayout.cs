namespace MSyics.Traceyi.Layout;

/// <summary>
/// 書式設定されたログを取得する機能を提供します。
/// </summary>
public interface ILogLayout
{
    /// <summary>
    /// トレースイベントデータを書式設定したログを取得します。
    /// </summary>
    /// <param name="e">トレースイベントデータ</param>
    /// <returns>ログ</returns>
    string GetLog(TraceEventArgs e);
}

namespace MSyics.Traceyi;

/// <summary>
/// FileStream を作成する機能を提供します。
/// </summary>
internal interface IFileStreamFactory
{
    /// <summary>
    /// 指定したパスの FileStream を作成します。
    /// </summary>
    FileStream Create(string path);

    /// <summary>
    /// 指定した FileStream を破棄します。
    /// </summary>
    void Dispose(FileStream stream);
}

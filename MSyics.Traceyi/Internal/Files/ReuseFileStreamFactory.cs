namespace MSyics.Traceyi;

/// <summary>
/// ReuseFileStream を作成するクラスです。
/// </summary>
internal class ReuseFileStreamFactory : IFileStreamFactory
{
    readonly FileShare share = FileShare.Read;

    public ReuseFileStreamFactory(bool canShareWrite = false)
    {
        if (canShareWrite)
        {
            share |= FileShare.Write;
        }
    }

    public FileStream Create(string path) => new ReuseFileStream(path, share);

    public void Dispose(FileStream stream)
    {
        if (stream is not ReuseFileStream reuse) return;

        reuse.Clean();
    }
}

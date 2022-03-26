using System.Security.Cryptography;
using System.Text;

namespace MSyics.Traceyi;

/// <summary>
/// 名前付き Mutex を作成するクラスです。
/// </summary>
internal sealed class MutexFactory
{
    public static MutexFactory Create() => new();

    const int maxLength = 260;
    readonly string prefix = @"Global\";
    private (string key, string name) created;

    private MutexFactory()
    {
    }

    /// <summary>
    /// 指定した文字列に対する Mutex を取得します。
    /// </summary>
    public Mutex Get(string source)
    {
        var key = source.ToUpperInvariant();
        if (created.key != key)
        {
            using var hash = HashRingDeriveBytes.Create(key);
            var length = maxLength - prefix.Length;
#if NETCOREAPP
            var name = Convert.ToBase64String(hash.GetBytes(length))[..length];
#else
            var name = Convert.ToBase64String(hash.GetBytes(length)).Substring(0, length);
#endif
            created = (key, name);
        }

        return new Mutex(false, $@"{prefix}{created.name}");
    }
}

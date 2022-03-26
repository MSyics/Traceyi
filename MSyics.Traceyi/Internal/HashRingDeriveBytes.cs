using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace MSyics.Traceyi;

internal class HashRingDeriveBytes : DeriveBytes
{
    private const int digit = 10;
    private readonly HashAlgorithm hashAlgorithm;
    private readonly int hashByteSize;
    private readonly byte[] buffer;
    private int position = 0;

    public static HashRingDeriveBytes Create(HashAlgorithm hashAlgorithm, byte[] source)
    {
        if (hashAlgorithm is null) throw new ArgumentNullException(nameof(hashAlgorithm));
        if (source is null) throw new ArgumentNullException(nameof(source));

        return new(hashAlgorithm, source);
    }

    public static HashRingDeriveBytes Create(string source)
    {
        return new(SHA512.Create(), Encoding.UTF8.GetBytes(source));
    }

    private HashRingDeriveBytes(HashAlgorithm hashAlgorithm, byte[] source)
    {
        this.hashAlgorithm = hashAlgorithm;
        hashByteSize = hashAlgorithm.HashSize / 8;
        buffer = new byte[source.Length + digit];
        Buffer.BlockCopy(source, 0, buffer, 0, source.Length);
    }

    public int Position
    {
        get => position;
        set => position = value >= 0 ? value : int.MaxValue + value + 1;
    }

    public void Move(int offset)
    {
        if (offset is 0) { return; }

        Position = unchecked(position + offset);
    }

    public override byte[] GetBytes(int cb) => Enumerate().Take(cb).ToArray();

    public override void Reset() => position = 0;

    public IEnumerable<byte> Enumerate(int offset, CancellationToken cancellationToken = default)
    {
        Move(offset);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IncrementBuffer();
            byte[] x = hashAlgorithm.ComputeHash(buffer);
            byte[] y = hashAlgorithm.ComputeHash(x);

            for (int i = position % hashByteSize; i < y.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Move(1);
                int capture = position;

                yield return unchecked((byte)(x[i] ^ y[i]));

                if (capture != position) { break; }
                if (position is 0) { break; }
            }
        }
    }

    public IEnumerable<byte> Enumerate(CancellationToken cancellationToken = default) => Enumerate(0, cancellationToken);

    private void IncrementBuffer()
    {
        Span<byte> bufferSpan = buffer.AsSpan(buffer.Length - digit);
        int p = position - (position % hashByteSize);
        for (int i = digit; i > 0; i--)
        {
            int pow = (int)Math.Pow(10, i - 1);
#if NETCOREAPP
            bufferSpan[^i] = unchecked((byte)(p / pow));
#else
            bufferSpan[digit - i] = unchecked((byte)(p / pow));
#endif
            p %= pow;
        }
    }

    protected override void Dispose(bool disposing)
    {
        hashAlgorithm.Clear();
        base.Dispose(disposing);
    }
}

using System.Text;

namespace MSyics.Traceyi.Layout;

/// <summary>
/// LogFormatter クラスで指定されたレイアウトを認識できるフォーマットに変換する機能を提供します。
/// </summary>
public sealed class LogLayoutConverter
{
    private readonly HashSet<string> partPlacements = new();
    private readonly LogLayoutPart[] parts;

    /// <summary>
    /// LogLayoutConverter クラスのインスタンスを初期化します。
    /// </summary>
    /// <param name="parts">ログの記録項目</param>
    public LogLayoutConverter(params LogLayoutPart[] parts) => this.parts = parts;

    public bool IsPartPlaced(string name) => partPlacements.Contains(name);

    /// <summary>
    /// 指定されたレイアウトを認識できるフォーマットに変換します。
    /// </summary>
    public string Convert(string layout)
    {
        partPlacements.Clear();
        StringBuilder sb = new();
        var span = layout.AsSpan();
        for (int layoutIndex = 0; layoutIndex < span.Length; layoutIndex++)
        {
            if (span[layoutIndex] is '{')
            {
                var isContinue = false;
                var startIndex = layoutIndex + 1;
#if NETCOREAPP
                var length = span[(startIndex + 1)..].IndexOf('}') + 1;
#else
                var length = span.Slice(startIndex + 1).IndexOf('}') + 1;
#endif
                if (length > 0)
                {
                    var template = span.Slice(startIndex, length);
                    for (int itemIndex = 0; itemIndex < parts.Length; itemIndex++)
                    {
                        var part = parts[itemIndex];
                        if (CanReplacePart(template, part))
                        {
                            if (part.CanFormat)
                            {
#if NETCOREAPP
                                var formatString = template[part.Name.Length..].Trim();
#else
                                var formatString = template.Slice(part.Name.Length).Trim();
#endif
                                var separator = GetSeparatorCharacter(formatString);
                                sb.Append($"{{{itemIndex}{separator}{formatString.ToString()}}}");
                            }
                            else
                            {
                                sb.Append($"{{{itemIndex}}}");
                            }
                            layoutIndex = startIndex + length;
                            isContinue = true;
                            partPlacements.Add(part.Name);
                            break;
                        }
                    }

                    if (isContinue) continue;
                }
                sb.Append('{');
            }
            else if (span[layoutIndex] is '}')
            {
                sb.Append('}');
            }

            sb.Append(span[layoutIndex]);
        }
        return sb.ToString();
    }

    private static bool CanReplacePart(ReadOnlySpan<char> value, LogLayoutPart part)
    {
        if (!value.StartsWith(part.Name.AsSpan(), StringComparison.OrdinalIgnoreCase)) return false;
#if NETCOREAPP
        value = value[part.Name.Length..].Trim();
#else
        value = value.Slice(part.Name.Length).Trim();
#endif
        if (value.Length is 0) return true;

        var c = value[0];
        if (c is ':' or ',' or '|' or '[') return true;
        // =>
        if (value[0] is '=' && value[1] is '>') return true;

        return false;
    }

    /// <summary>
    /// カスタム書式内の区切り文字を取得します。
    /// </summary>
    private static string GetSeparatorCharacter(ReadOnlySpan<char> format)
    {
        if (format.StartsWith(":".AsSpan())) return string.Empty;
        if (format.StartsWith(",".AsSpan())) return string.Empty;
        if (format.IsEmpty) return string.Empty;
        return ":";
    }
}

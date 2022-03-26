using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout;

/// <summary>
/// LogFormatter クラスで使用するカスタム書式を実装します。
/// </summary>
public sealed class LogLayoutFormatProvider : IFormatProvider, ICustomFormatter
{
    public const string JsonFormatSpecifier = "JSON";
    public const string IndentSpecifier = "INDENT";

    #region Static Members
    /// <summary>
    /// 書式内の区切り文字を示す固定値です。
    /// </summary>
    public static readonly string CustomFormatSpecifier = "|";
    public static readonly string SerializeFormatSpecifier = "=>";
    static readonly ILogStateBuilder logStateBuilder = new LogStateBuilder();
    static readonly JsonSerializerOptions IndentedOptions;
    static readonly JsonSerializerOptions NotIndentedOptions;

    static LogLayoutFormatProvider()
    {
        IndentedOptions = new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(),
                new JsonStringTimeSpanConverter(),
                new JsonStringPolymorphicConverter<Exception>(),
                new JsonStringPolymorphicConverter<MemberInfo>(),
                new JsonStringIfWriteFailureConverter(),
            }
        };
        NotIndentedOptions = new(IndentedOptions)
        {
            WriteIndented = false
        };
    }

    /// <summary>
    /// 指定した書式およびカルチャ固有の書式情報を使用して、指定したオブジェクトの値をそれと等価な文字列形式に変換します。
    /// </summary>
    /// <param name="format">書式指定を格納している書式指定文字列。</param>
    /// <param name="arg">書式指定するオブジェクト。</param>
    private static string Format(string format, object arg)
    {
        if (arg is null) return string.Empty;

        return arg switch
        {
            IFormattable formattable => formattable.ToString(format, CultureInfo.CurrentCulture),
            TraceEventArgs e => logStateBuilder.SetEvent(e).Build().ToString(),
            IDictionary<string, object> keyValuePairs => logStateBuilder.SetExtensions(keyValuePairs).Build()?.ToString() ?? "",
            _ => arg.ToString(),
        };
    }
    #endregion

    #region IFormatProvider Members
    /// <summary>
    /// 指定した型の書式指定サービスを提供するオブジェクトを返します。
    /// </summary>
    /// <param name="formatType">返す書式オブジェクトの型を指定するオブジェクト。 </param>
    public object GetFormat(Type formatType) => formatType == typeof(ICustomFormatter) ? this : null;
    #endregion

    #region ICustomFormatter Members
    /// <summary>
    /// 指定した書式およびカルチャ固有の書式情報を使用して、指定したオブジェクトの値をそれと等価な文字列形式に変換します。
    /// </summary>
    /// <param name="format">書式指定を格納している書式指定文字列。</param>
    /// <param name="arg">書式指定するオブジェクト。 </param>
    /// <param name="formatProvider">現在のインスタンスについての書式情報を提供するオブジェクト。</param>
    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            return Format(format, arg);
        }

        if (format.Contains(SerializeFormatSpecifier))
        {
            return BySerializing(format, arg);
        }

        if (format.Contains(CustomFormatSpecifier))
        {
            return ByCustom(format, arg);
        }

        if (arg is TraceEventArgs e)
        {
            return logStateBuilder.SetEvent(e, GetLogStateMembersOfTraceEvent(ref format, format.AsSpan())).Build()?.ToString() ?? "";
        }

        return Format(format, arg);
    }
    #endregion

    private string ByCustom(string format, object arg)
    {
        var formats = format.Split(new[] { CustomFormatSpecifier }, StringSplitOptions.None);

        // 標準書式の取得
        var standardFormat = formats[0];

        // カスタム書式の取得
        var customFormat = formats[1];
        var customFormats = customFormat.Split(',', ':');

        if (customFormats.Length is not 3)
        {
            Debug.WriteLine($"The input string [{format}] is not in the correct format.");
            return Format(format, arg);
        }

        // 文字数不足のときに埋める文字の取得
        char character;
        if (string.IsNullOrEmpty(customFormats[0]))
        {
            // {@@@|,N:L} 埋める文字が無いときは、空白を指定する。
            character = ' ';
        }
        else
        {
            character = customFormats[0][0];
        }

        // 文字数の取得
        if (!int.TryParse(customFormats[1], out var count))
        {
            Debug.WriteLine($"The input string [{format}] is not in the correct format.");
            return Format(format, arg);
        }

        if (count < 0)
        {
            count = Math.Abs(count);
        }

        // 文字位置の取得
        var position = customFormats[2].ToUpperInvariant();
        if (position.Length is not 1)
        {
            Debug.WriteLine($"The input string [{format}] is not in the correct format.");
            return Format(format, arg);
        }

        // 文字埋め
        var formatString = Format(standardFormat, arg);
        var length = Encoding.UTF8.GetByteCount(formatString);
        if (position is "L")
        {
            if (formatString.Length >= count)
            {
#if NETCOREAPP
                return formatString[..count];
#else
                return formatString.Substring(0, count);
#endif
            }
            else
            {
                return formatString + new string(character, count - length);
            }
        }
        else if (position is "R")
        {
            if (formatString.Length >= count)
            {
                return formatString.Substring(length - count, count);
            }
            else
            {
                return new string(character, count - length) + formatString;
            }
        }

        Debug.WriteLine($"The input string [{format}] is not in the correct format.");
        return Format(format, arg);
    }

    private string BySerializing(string format, object arg)
    {
        if (arg is null) { return ""; }

#if NETCOREAPP
        var formats = format.Split(SerializeFormatSpecifier, StringSplitOptions.None);
        var right = formats[1].Split(',', StringSplitOptions.None);
#else
        var formats = format.Split(new[] { SerializeFormatSpecifier }, StringSplitOptions.None);
        var right = formats[1].Split(new[] { "," }, StringSplitOptions.None);
#endif

        var specifier = right[0].ToUpperInvariant().Trim();
        if (specifier is JsonFormatSpecifier or "")
        {
            return ByJson(format, arg, formats[0].AsSpan(), right);
        }
        else
        {
            Debug.WriteLine("No format is specified.");
            return "";
        }
    }

    private string ByJson(string format, object arg, ReadOnlySpan<char> left, string[] right)
    {
        if (arg is TraceEventArgs e)
        {
            var logState = logStateBuilder.SetEvent(e, GetLogStateMembersOfTraceEvent(ref format, left)).Build();
            if (logState is null) { return ""; }
            arg = logState;
        }

        var options = right.Length > 1 && right[1].Trim().Equals(IndentSpecifier, StringComparison.OrdinalIgnoreCase)
            ? IndentedOptions
            : NotIndentedOptions;

        try
        {
            return JsonSerializer.Serialize(arg, options);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{arg}, {ex}");
            return JsonSerializer.Serialize(arg.ToString(), options);
        }
    }

    private LogStateMembersOfTraceEvent GetLogStateMembersOfTraceEvent(ref string format, ReadOnlySpan<char> span)
    {
        if (logStateMembers.TryGetValue(format, out LogStateMembersOfTraceEvent members))
        {
            return members;
        }

        members = LogStateMembersOfTraceEvent.All;

        var startIndex = span.IndexOf('[');
        if (startIndex >= 0)
        {
            var endIndex = span.LastIndexOf(']');
#if NETCOREAPP
            var value = (endIndex is -1 ? span[startIndex..] : span[startIndex..endIndex]);
#else
            var value = (endIndex is -1 ? span.Slice(startIndex) : span.Slice(startIndex, endIndex - startIndex));
#endif
            if (Enum.TryParse<LogStateMembersOfTraceEvent>(value.TrimStart('[').TrimEnd(']').ToString(), true, out var result))
            {
                members = result;
            }
        }

        logStateMembers[format] = members;
        return members;
    }

    private readonly Dictionary<string, LogStateMembersOfTraceEvent> logStateMembers = new();
}

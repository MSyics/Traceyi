using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// LogFormatter クラスで使用するカスタム書式を実装します。
    /// </summary>
    public sealed class LogLayoutFormatProvider : IFormatProvider, ICustomFormatter
    {
        #region Static Members
        /// <summary>
        /// 書式内の区切り文字を示す固定値です。
        /// </summary>
        public static readonly string FormatSpecifier = "|";
        public static readonly string SerializeFormatSpecifier = "=>";
        public static readonly string JsonFormatSpecifier = "JSON";
        public static readonly string IndentSpecifier = "INDENT";
        static readonly JsonSerializerOptions IndentedOptions;
        static readonly JsonSerializerOptions NotIndentedOptions;

        static LogLayoutFormatProvider()
        {
            IndentedOptions = new(JsonSerializerDefaults.Web)
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new TimeSpanToStringJsonConverter(),
                    new ExceptionToStringJsonConverter(),
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
            if (arg == null)
            {
                return string.Empty;
            }

            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }

            return arg.ToString();
        }
        #endregion

        private readonly ILogStateBuilder logStateBuilder = new LogStateBuilder();

        #region IFormatProvider Members
        /// <summary>
        /// 指定した型の書式指定サービスを提供するオブジェクトを返します。
        /// </summary>
        /// <param name="formatType">返す書式オブジェクトの型を指定するオブジェクト。 </param>
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }
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
            if (!string.IsNullOrEmpty(format))
            {
                if (format.Contains(SerializeFormatSpecifier))
                {
                    if (arg is null)
                    {
                        return Format(format, arg);
                    }

                    var formats = format.Split(new[] { SerializeFormatSpecifier }, StringSplitOptions.None);
                    var right = formats[1].Split(new[] { "," }, StringSplitOptions.None);

                    // JSON
                    if (right[0].ToUpperInvariant().Trim() == JsonFormatSpecifier)
                    {
                        if (arg is TraceEventArgs e)
                        {
                            LogStateMembers members = LogStateMembers.All;

                            // TODO: cache
                            var left = formats[0].AsSpan();
                            var startIndex = left.IndexOf('[');
                            if (startIndex >= 0)
                            {
                                var endIndex = left.LastIndexOf(']');
#if NETCOREAPP
                                var value = (endIndex == -1 ? left[startIndex..] : left[startIndex..endIndex]);
#else
                                var value = (endIndex == -1 ? left.Slice(startIndex) : left.Slice(startIndex, endIndex - startIndex));
#endif
                                if (Enum.TryParse<LogStateMembers>(value.TrimStart('[').TrimEnd(']').ToString(), true, out var result))
                                {
                                    members = result;
                                }
                            }
                            arg = logStateBuilder.SetEvent(e, members).Build();
                            if (arg is null)
                            {
                                return Format(format, arg);
                            }
                        }

                        try
                        {
                            return JsonSerializer.Serialize(arg, right.Length > 1 && right[1].ToUpperInvariant().Trim() == IndentSpecifier
                                ? IndentedOptions
                                : NotIndentedOptions);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print($"{ex}");
                        }
                    }

                    Debug.Print($"The input string [{format}] is not in the correct format.");
                    return arg.ToString();
                }

                if (format.Contains(FormatSpecifier))
                {
                    var formats = format.Split(new[] { FormatSpecifier }, StringSplitOptions.None);

                    // 標準書式の取得
                    var standardFormat = formats[0];

                    // カスタム書式の取得
                    var customFormat = formats[1];
                    var customFormats = customFormat.Split(',', ':');

                    if (customFormats.Length != 3)
                    {
                        Debug.Print($"The input string [{format}] is not in the correct format.");
                        return arg.ToString();
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
                        Debug.Print($"The input string [{format}] is not in the correct format.");
                        return arg.ToString();
                    }

                    if (count < 0)
                    {
                        count = Math.Abs(count);
                    }

                    // 文字位置の取得
                    var position = customFormats[2].ToUpperInvariant();
                    if (position.Length != 1)
                    {
                        Debug.Print($"The input string [{format}] is not in the correct format.");
                        return arg.ToString();
                    }

                    // 文字埋め
                    var formatString = Format(standardFormat, arg);
                    var length = Encoding.Default.GetByteCount(formatString);
                    if (position == "L")
                    {
                        if (formatString.Length >= count)
                        {
                            return formatString.Substring(0, count);
                        }
                        else
                        {
                            return formatString + new string(character, count - length);
                        }
                    }
                    else if (position == "R")
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

                    Debug.Print($"The input string [{format}] is not in the correct format.");
                    return arg.ToString();
                }
            }
            return Format(format, arg);
        }
        #endregion
    }
}

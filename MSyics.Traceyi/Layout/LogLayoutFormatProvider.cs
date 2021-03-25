using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// LogFormatter クラスで使用するカスタム書式を実装します。
    /// </summary>
    public sealed class LogLayoutFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// 書式内の区切り文字を示す固定値です。
        /// </summary>
        public static readonly string FormatSpecifier = "|";
        public static readonly string SerializeFormatSpecifier = "=>";
        public static readonly string JsonFormatSpecifier = "JSON";

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
                    var formats = format.Split(new[] { SerializeFormatSpecifier }, StringSplitOptions.None);

                    // JSON
                    if (formats.Length == 2 && formats[1].ToUpperInvariant().Trim() == JsonFormatSpecifier)
                    {
                        try
                        {
                            return JsonSerializer.Serialize(arg);
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

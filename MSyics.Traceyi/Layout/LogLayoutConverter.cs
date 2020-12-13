using System;
using System.Diagnostics;
using System.Text;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// LogFormatter クラスで指定されたレイアウトを認識できるフォーマットに変換する機能を提供します。
    /// </summary>
    internal sealed class LogLayoutConverter
    {
        /// <summary>
        /// LogLayoutConverter クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="parts">ログの記録項目</param>
        public LogLayoutConverter(params LogLayoutPart[] parts) => Parts = parts;

#if NETCOREAPP
        /// <summary>
        /// 指定されたレイアウトを認識できるフォーマットに変換します。
        /// </summary>
        public string Convert(string layout)
        {
            var span = layout.AsSpan();
            var sb = new StringBuilder();
            for (int layoutIndex = 0; layoutIndex < span.Length; layoutIndex++)
            {
                if (span[layoutIndex] == '{')
                {
                    var isContinue = false;
                    var startIndex = layoutIndex + 1;
                    var length = span[(startIndex + 1)..].IndexOf('}') + 1;

                    if (length > 0)
                    {
                        var convertString = span.Slice(startIndex, length);
                        for (int itemIndex = 0; itemIndex < Parts.Length; itemIndex++)
                        {
                            var part = Parts[itemIndex];
                            if (convertString.StartsWith(part.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                if (part.CanFormat)
                                {
                                    var formatString = convertString[part.Name.Length..];
                                    var separator = GetSeparatorCharacter(formatString);
                                    sb.Append($"{{{itemIndex}{separator}{formatString.ToString()}}}");
                                }
                                else
                                {
                                    sb.Append($"{{{itemIndex}}}");
                                }
                                layoutIndex = startIndex + length;
                                isContinue = true;
                                break;
                            }
                        }
                        if (isContinue) { continue; }
                    }
                    sb.Append('{');
                }
                else if (span[layoutIndex] == '}')
                {
                    sb.Append('}');
                }

                sb.Append(span[layoutIndex]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// カスタム書式内の区切り文字を取得します。
        /// </summary>
        private static string GetSeparatorCharacter(ReadOnlySpan<char> format)
        {
            if (format.StartsWith(":")) { return string.Empty; }
            if (format.StartsWith(",")) { return string.Empty; }
            if (format.IsEmpty) { return string.Empty; }
            return ":";
        }

#else
        /// <summary>
        /// 指定されたレイアウトを認識できるフォーマットに変換します。
        /// </summary>
        public string Convert(string layout)
        {
            var sb = new StringBuilder();
            for (int layoutIndex = 0; layoutIndex < layout.Length; layoutIndex++)
            {
                if (layout[layoutIndex] == '{')
                {
                    var isContinue = false;
                    var startIndex = layoutIndex + 1;
                    var length = layout.IndexOf('}', startIndex + 1) - startIndex;

                    if (length <= 0)
                    {
                        Debug.Print("The log layout is not in the correct format.");
                        return "";
                    }

                    var convertString = layout.Substring(startIndex, length);
                    for (int itemIndex = 0; itemIndex < Parts.Length; itemIndex++)
                    {
                        var part = Parts[itemIndex];
                        if (convertString.StartsWith(part.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (part.CanFormat)
                            {
                                var formatString = convertString.Substring(part.Name.Length);
                                var separator = GetSeparatorCharacter(formatString);
                                sb.Append($"{{{itemIndex}{separator}{formatString}}}");
                            }
                            else
                            {
                                sb.Append($"{{{itemIndex}}}");
                            }
                            layoutIndex = startIndex + length;
                            isContinue = true;
                            break;
                        }
                    }
                    if (isContinue) { continue; }
                }
                sb.Append(layout[layoutIndex]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// カスタム書式内の区切り文字を取得します。
        /// </summary>
        private string GetSeparatorCharacter(string format)
        {
            if (format.StartsWith(":")) { return string.Empty; }
            if (format.StartsWith(",")) { return string.Empty; }
            if (string.IsNullOrEmpty(format)) { return string.Empty; }
            return ":";
        }
#endif

        private LogLayoutPart[] Parts { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Layout
{
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

#if NETCOREAPP
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
                if (span[layoutIndex] == '{')
                {
                    var isContinue = false;
                    var startIndex = layoutIndex + 1;
                    var length = span[(startIndex + 1)..].IndexOf('}') + 1;

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
                                    var formatString = template[part.Name.Length..].Trim();
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

        private static bool CanReplacePart(ReadOnlySpan<char> value, LogLayoutPart part)
        {
            if (!value.StartsWith(part.Name, StringComparison.OrdinalIgnoreCase)) { return false; }

            value = value[part.Name.Length..].Trim();
            if (value.Length == 0) { return true; }

            var c = value[0];
            if (c == ':' || c == ',' || c == '|') { return true; }

            // =>
            if (value[0] == '=' && value[1] == '>') { return true; }

            return false;
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
            partPlacements.Clear();
            StringBuilder sb = new();
            for (int layoutIndex = 0; layoutIndex < layout.Length; layoutIndex++)
            {
                if (layout[layoutIndex] == '{')
                {
                    var isContinue = false;
                    var startIndex = layoutIndex + 1;
                    var length = layout.Substring(startIndex + 1).IndexOf('}') + 1;

                    if (length > 0)
                    {
                        var template = layout.Substring(startIndex, length);
                        for (int itemIndex = 0; itemIndex < parts.Length; itemIndex++)
                        {
                            var part = parts[itemIndex];
                            if (CanReplacePart(template, part))
                            {
                                if (part.CanFormat)
                                {
                                    var formatString = template.Substring(part.Name.Length).Trim();
                                    var separator = GetSeparatorCharacter(formatString);
                                    sb.Append($"{{{itemIndex}{separator}{formatString}}}");
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
                        if (isContinue) { continue; }
                    }
                    sb.Append('{');
                }
                else if (layout[layoutIndex] == '}')
                {
                    sb.Append('}');
                }

                sb.Append(layout[layoutIndex]);
            }
            return sb.ToString();
        }

        private static bool CanReplacePart(string value, LogLayoutPart part)
        {
            if (!value.StartsWith(part.Name, StringComparison.OrdinalIgnoreCase)) { return false; }
            
            value = value.Substring(part.Name.Length).Trim();
            if (value.Length == 0) { return true; }

            var c = value[0];
            if (c == ':' || c == ',' || c == '|') { return true; }

            // =>
            if (value[0] == '=' && value[1] == '>') { return true; }

            return false;
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
    }
}

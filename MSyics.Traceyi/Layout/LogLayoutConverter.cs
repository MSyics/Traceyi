/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
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

                    if (length <= 0) { throw new FormatException("入力文字列の形式が正しくありません。"); }

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

        private LogLayoutPart[] Parts { get; set; }
    }
}

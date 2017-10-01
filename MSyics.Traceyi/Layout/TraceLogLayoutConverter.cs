using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// TextLogLayout クラスで指定されたレイアウトを認識できるフォーマットに変換する機能を提供します。
    /// </summary>
    internal sealed class TraceLogLayoutConverter
    {
        /// <summary>
        /// TextLogLayoutConverter クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="items">ログの記録項目</param>
        public TraceLogLayoutConverter(params TraceLogLayoutItem[] items) => this.Items = items;

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
                    for (int itemIndex = 0; itemIndex < this.Items.Length; itemIndex++)
                    {
                        var item = this.Items[itemIndex];
                        if (convertString.StartsWith(item.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.CanFormat)
                            {
                                var formatString = convertString.Substring(item.Name.Length);
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

        private TraceLogLayoutItem[] Items { get; set; }
    }
}

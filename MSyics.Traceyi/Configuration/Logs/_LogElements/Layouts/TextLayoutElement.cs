using MSyics.Traceyi.Layout;
using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// layout 要素を表します。
    /// </summary>
    public class TextLogLayoutElement : ConfigurationElement
    {
        const string FormatPropertyName = "format";

        /// <summary>
        /// レイアウト形式を取得または設定します。
        /// </summary>
        [ConfigurationProperty(FormatPropertyName, DefaultValue = TextLogLayout.DefaultLayout)]
        public string Format
        {
            get { return (string)this[FormatPropertyName]; }
            set { this[FormatPropertyName] = value; }
        }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public TextLogLayout GetRuntimeObject()
        {
            return new TextLogLayout(this.Format);
        }
    }
}

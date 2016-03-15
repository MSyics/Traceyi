using System;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// TextWriterLog クラスから派生するクラスを設定する要素を表します。これは抽象クラスです。
    /// </summary>
    public abstract class TextWriterLogElement : LogElement
    {
        const string LayoutPropertyName = "layout";
        const string EncodingValuePropertyName = "encoding";
        const string NewLinePropertyName = "newLine";

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        [ConfigurationProperty(NewLinePropertyName, DefaultValue = "\r\n")]
        public string NewLine
        {
            get { return Regex.Unescape((string)this[NewLinePropertyName]); }
            set { this[NewLinePropertyName] = value; }
        }

        /// <summary>
        /// 文字エンコーディングの値を取得または設定します。
        /// </summary>
        [ConfigurationProperty(EncodingValuePropertyName, DefaultValue = "UTF-8")]
        public string EncodingValue
        {
            get { return (string)this[EncodingValuePropertyName]; }
            set { this[EncodingValuePropertyName] = value; }
        }

        /// <summary>
        /// layout 要素を取得または設定します。
        /// </summary>
        [ConfigurationProperty(LayoutPropertyName)]
        public TextLogLayoutElement Layout
        {
            get { return (TextLogLayoutElement)this[LayoutPropertyName]; }
            set { this[LayoutPropertyName] = value; }
        }

        /// <summary>
        /// 文字エンコーディングを取得します。
        /// </summary>
        public Encoding Encoding
        {
            get { return GetEncoding(this.EncodingValue); }
        }

        private Encoding GetEncoding(string value)
        {
            int codepage;
            if (int.TryParse(value, out codepage))
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(codepage);
                }
                catch (Exception e)
                {
                    throw new ConfigurationErrorsException("encoding", e);
                }
            }
            else
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(value);
                }
                catch (Exception e)
                {
                    throw new ConfigurationErrorsException("encoding", e);
                }
            }
        }
    }
}

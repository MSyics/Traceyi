/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Text;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// TextWriterListener クラスから派生するクラスを設定する要素を表します。これは抽象クラスです。
    /// </summary>
    public abstract class TextWriterListenerElement : ListenerElement
    {
        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; }

        /// <summary>
        /// 文字エンコーディングの値を取得または設定します。
        /// </summary>
        public string Encoding { get; set; } = System.Text.Encoding.Default.CodePage.ToString();

        /// <summary>
        /// layout 要素を取得または設定します。
        /// </summary>
        public LayoutElement Layout { get; set; } = new LayoutElement();

        /// <summary>
        /// 文字エンコーディングを取得します。
        /// </summary>
        protected Encoding GetEncoding()
        {
            if (int.TryParse(Encoding, out var codepage))
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(codepage);
                }
                catch (Exception)
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(Encoding);
                }
                catch (Exception)
                {
                    return System.Text.Encoding.Default;
                }
            }
        }
    }

}
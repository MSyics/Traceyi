/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Text;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Listener セクション要素の中で Logger 要素の基底クラスです。
    /// </summary>
    public abstract class TextLoggerElement : ListenerElement
    {
        /// <summary>
        /// グローバルロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseGlobalLock { get; set; }

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; }

        /// <summary>
        /// 文字エンコーディングの値を取得または設定します。
        /// </summary>
        public string Encoding { get; set; } = System.Text.Encoding.Default.CodePage.ToString();

        /// <summary>
        /// Layout 要素を取得または設定します。
        /// </summary>
        public LayoutElement Layout { get; set; } = new LayoutElement();

        /// <summary>
        /// 文字エンコーディングを取得します。
        /// </summary>
        protected Encoding GetEncoding()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
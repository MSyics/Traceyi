using Microsoft.Extensions.Configuration;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            int codepage;
            if (int.TryParse(Encoding, out codepage))
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
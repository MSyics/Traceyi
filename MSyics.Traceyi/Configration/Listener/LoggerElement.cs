using System;
using System.Diagnostics;
using System.Text;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Listener セクション要素の中で Logger 要素の基底クラスです。
    /// </summary>
    public abstract class LoggerElement : TraceListenerElement
    {
        /// <summary>
        /// ロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = false;

        /// <summary>
        /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得または設定します。
        /// </summary>
        public bool UseAsync { get; set; } = true;

        /// <summary>
        /// 終了を待機する時間間隔を取得または設定します。
        /// </summary>
        public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; } = Environment.NewLine;

        /// <summary>
        /// 文字エンコーディングの値を取得または設定します。
        /// </summary>
        public string Encoding { get; set; } = System.Text.Encoding.UTF8.CodePage.ToString();

        /// <summary>
        /// Layout 要素を取得または設定します。
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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(Encoding);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return System.Text.Encoding.Default;
                }
            }
        }
    }

}
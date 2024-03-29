﻿using System.Diagnostics;
using System.Text;

namespace MSyics.Traceyi.Configration;

/// <summary>
/// Listener セクション要素の中で TextLogger 要素の基底クラスです。
/// </summary>
public abstract class TextLoggerElement : TraceEventListenerElement
{
    /// <summary>
    /// 文字エンコーディングの値を取得または設定します。
    /// </summary>
    public string Encoding { get; set; } = System.Text.Encoding.UTF8.CodePage.ToString();

    /// <summary>
    /// Layout 要素を取得または設定します。
    /// </summary>
    public LogLayoutElement Layout { get; set; } = new LogLayoutElement();

    /// <summary>
    /// 改行文字を取得または設定します。
    /// </summary>
    public string NewLine { get; set; } = Environment.NewLine;

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
                Debug.WriteLine(ex);
                return System.Text.Encoding.UTF8;
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
                Debug.WriteLine(ex);
                return System.Text.Encoding.UTF8;
            }
        }
    }
}

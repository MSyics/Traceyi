/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Console 要素を表します。
    /// </summary>
    public class ConsoleElement : LoggerElement
    {
        /// <summary>
        /// エラーストリームを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseErrorStream { get; set; } = false;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override ITraceListener GetRuntimeObject() =>
             new ConsoleLogger(UseErrorStream, Layout.GetRuntimeObject())
             {
                 Name = Name,
                 NewLine = NewLine,
                 UseLock = UseLock,
                 UseAsync = UseAsync,
                 CloseTimeout = CloseTimeout, // TimeSpan.Parse(CloseTimeout),
                 Encoding = GetEncoding(),
             };
    }
}
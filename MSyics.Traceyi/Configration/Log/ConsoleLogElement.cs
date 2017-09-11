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
    /// console 要素を表します。
    /// </summary>
    public class ConsoleLogElement : TextWriterLogElement
    {
        /// <summary>
        /// エラーストリームを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseErrorStream { get; set; }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override Log GetRuntimeObject() =>
             new ConsoleLog(this.UseErrorStream, this.Layout.GetRuntimeObject())
             {
                 Name = this.Name,
                 NewLine = this.NewLine,
                 UseGlobalLock = this.UseGlobalLock,
             };
    }

}
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
    /// file 要素を表します。
    /// </summary>
    public class FileLoggingListenerElement : TextWriterListenerElement
    {
        /// <summary>
        /// パスを取得または設定します。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override LoggingListener GetRuntimeObject()
        {
            var path = string.IsNullOrWhiteSpace(this.Path) ? System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,AppDomain.CurrentDomain.FriendlyName + ".log") : this.Path;

            if (!System.IO.Path.IsPathRooted(path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            return new FileLoggingListener(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), GetEncoding(), this.Layout.GetRuntimeObject())
            {
                Name = this.Name,
                NewLine = this.NewLine,
                UseGlobalLock = this.UseGlobalLock,
            };
        }
    }

}
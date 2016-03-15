using System;
using System.Configuration;
using System.IO;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// file 要素を表します。
    /// </summary>
    public class FileLogElement : TextWriterLogElement
    {
        const string PathPropertyName = "path";

        /// <summary>
        /// パスを取得または設定します。
        /// </summary>
        [ConfigurationProperty(PathPropertyName, DefaultValue = "")]
        public string Path
        {
            get { return (string)this[PathPropertyName]; }
            set { this[PathPropertyName] = value; }
        }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public override Log GetRuntimeObject()
        {
            var path = string.IsNullOrEmpty(this.Path) ? AppDomain.CurrentDomain.FriendlyName + ".log" : this.Path;

            if (!System.IO.Path.IsPathRooted(path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            return new FileLog(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), this.Encoding, this.Layout.GetRuntimeObject())
            {
                Name = this.Name,
                NewLine = this.NewLine,
                UseGlobalLock = this.UseGlobalLock,
            };
        }
    }
}

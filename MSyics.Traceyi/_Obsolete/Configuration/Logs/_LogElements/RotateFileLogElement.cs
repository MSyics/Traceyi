using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// rotateFile 要素を表します。
    /// </summary>
    public class RotateFileLogElement : TextWriterLogElement
    {
        const string PathLayoutPropertyName = "pathLayout";
        const string MaxLengthPropertyName = "maxLength";
        const string LeaveFilesPropertyName = "leaveFiles";

        /// <summary>
        /// パスのレイアウトを取得または設定します。
        /// </summary>
        [ConfigurationProperty(PathLayoutPropertyName, DefaultValue = "")]
        public string PathLayout
        {
            get { return (string)this[PathLayoutPropertyName]; }
            set { this[PathLayoutPropertyName] = value; }
        }

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        [ConfigurationProperty(MaxLengthPropertyName, DefaultValue = "0")]
        public long MaxLength
        {
            get { return (long)this[MaxLengthPropertyName]; }
            set { this[MaxLengthPropertyName] = value; }
        }

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルを残しておくのかどうかを示す値を取得または設定します。
        /// </summary>
        [ConfigurationProperty(LeaveFilesPropertyName, DefaultValue = false)]
        public bool LeaveFiles
        {
            get { return (bool)this[LeaveFilesPropertyName]; }
            set { this[LeaveFilesPropertyName] = value; }
        }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        /// <returns></returns>
        public override Log GetRuntimeObject()
        {
            return new RotateFileLog(this.PathLayout)
            {
                Encoding = this.Encoding,
                Layout = this.Layout.GetRuntimeObject(),
                Name = this.Name,
                NewLine = this.NewLine,
                UseGlobalLock = this.UseGlobalLock,
                MaxLength = this.MaxLength,
                LeaveFiles = this.LeaveFiles,
            };
        }
    }
}

using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// console 要素を表します。
    /// </summary>
    public class ConsoleLogElement : TextWriterLogElement
    {
        const string UseErrorStreamPropertyName = "useErrorStream";

        /// <summary>
        /// エラーストリームを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        [ConfigurationProperty(UseErrorStreamPropertyName, DefaultValue = false)]
        public bool UseErrorStream
        {
            get { return (bool)this[UseErrorStreamPropertyName]; }
            set { this[UseErrorStreamPropertyName] = value; }
        }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        /// <returns></returns>
        public override Log GetRuntimeObject()
        {
            return new ConsoleLog(this.UseErrorStream, this.Layout.GetRuntimeObject())
            {
                Name = this.Name,
                NewLine = this.NewLine,
                UseGlobalLock = this.UseGlobalLock,
            };
        }
    }
}

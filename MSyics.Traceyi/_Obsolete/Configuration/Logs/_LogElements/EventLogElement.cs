using System.Configuration;
using System.Text.RegularExpressions;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// TextWriterLog クラスから派生するクラスを設定する要素を表します。これは抽象クラスです。
    /// </summary>
    public sealed class EventLogElement : LogElement
    {
        const string SourceNamePropertyName = "sourceName";
        const string LogNamePropertyName = "logName";
        const string MachineNamePropertyName = "machineName";
        const string LayoutPropertyName = "layout";

        /// <summary>
        /// ソース名を取得または設定します。
        /// </summary>
        [ConfigurationProperty(SourceNamePropertyName, IsRequired=true)]
        public string SourceName
        {
            get { return Regex.Unescape((string)this[SourceNamePropertyName]); }
            set { this[SourceNamePropertyName] = value; }
        }

        /// <summary>
        /// ログ名を取得または設定します。
        /// </summary>
        [ConfigurationProperty(LogNamePropertyName, IsRequired=true)]
        public string LogName
        {
            get { return (string)this[LogNamePropertyName]; }
            set { this[LogNamePropertyName] = value; }
        }

        /// <summary>
        /// マシン名を取得または設定します。
        /// </summary>
        [ConfigurationProperty(MachineNamePropertyName, DefaultValue = EventLog.LocalMachineName)]
        public string MachineName
        {
            get { return (string)this[MachineNamePropertyName]; }
            set { this[MachineNamePropertyName] = value; }
        }

        /// <summary>
        /// layout 要素を取得または設定します。
        /// </summary>
        [ConfigurationProperty(LayoutPropertyName)]
        public TextLogLayoutElement Layout
        {
            get { return (TextLogLayoutElement)this[LayoutPropertyName]; }
            set { this[LayoutPropertyName] = value; }
        }

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        /// <returns></returns>
        public override Log GetRuntimeObject()
        {
            return new EventLog(this.SourceName, this.LogName, this.MachineName);
        }
    }
}

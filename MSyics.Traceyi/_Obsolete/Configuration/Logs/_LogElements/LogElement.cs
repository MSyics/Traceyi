using System.Configuration;
using System.Xml;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// Log クラスから派生するクラスを設定する要素を表します。これは抽象クラスです。
    /// </summary>
    public abstract class LogElement : NamedElement, IConfigurationMixable
    {
        const string UseGlobalLockPropertyName = "useGlobalLock";

        /// <summary>
        /// グローバルロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        [ConfigurationProperty(UseGlobalLockPropertyName, DefaultValue = false)]
        public bool UseGlobalLock
        {
            get { return (bool)this[UseGlobalLockPropertyName]; }
            set { this[UseGlobalLockPropertyName] = value; }
        }

        string IConfigurationMixable.Key
        {
            get { return this.Name; }
        }

        void IConfigurationMixable.Deserialize(XmlReader reader)
        {
            DeserializeElement(reader, false);
        }

        /// <summary>
        ///　派生クラスでオーバーライドされると実行オブジェクトを取得します。
        /// </summary>
        public abstract Log GetRuntimeObject();
    }
}

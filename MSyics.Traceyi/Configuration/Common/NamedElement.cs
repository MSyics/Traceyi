using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    /// <summary>
    /// name 属性を持つ要素を表します。
    /// </summary>
    public abstract class NamedElement : ConfigurationElement
    {
        const string NamePropertyName = "name";

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this[NamePropertyName]; }
            set { this[NamePropertyName] = value; }
        }
    }
}

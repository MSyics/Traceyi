using System.Configuration;
using System.Xml;

namespace MSyics.Traceyi.Configuration
{
    [ConfigurationCollection(typeof(AttachedLogElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    internal abstract class TracerElementBase : ElementCollection<AttachedLogElement>, IConfigurationMixable
    {
        private const string FilterNamePropertyName = "filter";

        protected override string ElementName
        {
            get { return "attach"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AttachedLogElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AttachedLogElement)element).LogName;
        }

        [ConfigurationProperty(FilterNamePropertyName)]
        public string FilterName
        {
            get { return (string)this[FilterNamePropertyName]; }
            set { this[FilterNamePropertyName] = value; }
        }

        public abstract string Name { get; protected set; }

        string IConfigurationMixable.Key
        {
            get { return this.Name; }
        }

        void IConfigurationMixable.Deserialize(XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }

        public bool HasFilterName
        {
            get { return !string.IsNullOrEmpty(this.FilterName); }
        }
    }
}

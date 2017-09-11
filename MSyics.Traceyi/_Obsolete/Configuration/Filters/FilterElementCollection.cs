using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    [ConfigurationCollection(typeof(FilterElement), AddItemName = "add")]
    internal sealed class FilterElementCollection : NamedElementCollection<FilterElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FilterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FilterElement)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
    }
}

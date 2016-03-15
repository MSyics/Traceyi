using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    [ConfigurationCollection(typeof(TracerElementBase), AddItemName = "attach")]
    internal sealed class TracerElementCollection : MixableElementCollection<TracerElementBase>
    {
        protected override TracerElementBase CreateSelectNewElement(string elementName)
        {
            switch (elementName)
            {
                case "default":
                    return new DefaultTracerElement();
                case "tracer":
                    return new TracerElement();
                default:
                    throw new ConfigurationErrorsException("element name");
            }
        }

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;
    }
}

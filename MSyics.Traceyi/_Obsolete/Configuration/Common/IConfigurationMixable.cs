using System.Xml;

namespace MSyics.Traceyi.Configuration
{
    internal interface IConfigurationMixable
    {
        string Key { get; }
        void Deserialize(XmlReader reader);
    }
}

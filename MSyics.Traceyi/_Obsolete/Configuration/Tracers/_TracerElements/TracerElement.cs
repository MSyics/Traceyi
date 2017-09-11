using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    internal sealed class TracerElement : TracerElementBase
    {
        const string NamePropertyName = "name";

        [ConfigurationProperty(NamePropertyName, IsKey = true, IsRequired = true)]
        public override string Name
        {
            get { return (string)this[NamePropertyName]; }
            protected set { this[NamePropertyName] = value; }
        }
    }
}

using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    internal sealed class FilterElement : NamedElement
    {
        const string ValuePropertyName = "value";

        [ConfigurationProperty(ValuePropertyName, DefaultValue = TraceFilters.All)]
        public TraceFilters Value
        {
            get { return (TraceFilters)this[ValuePropertyName]; }
            set
            {
                this[ValuePropertyName] = value;
            }
        }
    }
}

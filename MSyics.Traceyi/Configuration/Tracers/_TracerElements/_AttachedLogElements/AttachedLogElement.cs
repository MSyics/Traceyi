using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    internal sealed class AttachedLogElement : ConfigurationElement
    {
        const string LogNamePropertyName = "log";
        const string FilterNamePropertyName = "filter";

        [ConfigurationProperty(LogNamePropertyName, IsKey = true, IsRequired = true)]
        public string LogName
        {
            get { return (string)this[LogNamePropertyName]; }
            set { this[LogNamePropertyName] = value; }
        }

        [ConfigurationProperty(FilterNamePropertyName)]
        public string FilterName
        {
            get { return (string)this[FilterNamePropertyName]; }
            set { this[FilterNamePropertyName] = value; }
        }
    }
}

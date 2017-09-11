using MSyics.Traceyi.Configuration;
using System.Configuration;

namespace MSyics.Traceyi
{
    internal sealed class TraceyiConfigurationSection : ConfigurationSection
    {
        private const string TracersPropertyName = "tracers";
        private const string FiltersPropertyName = "filters";
        private const string LogsPropertyName = "logs";

        [ConfigurationProperty(TracersPropertyName)]
        public TracerElementCollection Tracers => (TracerElementCollection)base[TracersPropertyName];

        [ConfigurationProperty(FiltersPropertyName)]
        public FilterElementCollection Filters => (FilterElementCollection)base[FiltersPropertyName];

        [ConfigurationProperty(LogsPropertyName)]
        public LogElementCollection Logs => (LogElementCollection)base[LogsPropertyName];

        public bool HasTracers => this.Tracers != null && this.Tracers.Count > 0;
        public bool HasFilters => this.Filters != null && this.Filters.Count > 0;
        public bool HasLogs => this.Logs != null && this.Logs.Count > 0;
    }
}

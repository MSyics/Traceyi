using System.Configuration;

namespace MSyics.Traceyi
{
    internal sealed class TraceyiConfiguration
    {
        private const string TraceyiSectionName = "msyics/traceyi";

        static TraceyiConfiguration()
        {
            Root = ConfigurationManager.GetSection(TraceyiSectionName) as TraceyiConfigurationSection;
        }

        internal static TraceyiConfigurationSection Root { get; private set; }
        internal static bool HasRoot { get { return Root != null; } }
    }
}

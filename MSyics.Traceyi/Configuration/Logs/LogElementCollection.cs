using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    [ConfigurationCollection(typeof(LogElement), AddItemName = "")]
    internal sealed class LogElementCollection : MixableElementCollection<LogElement>
    {
        const string ResolversPropertyName = "resolvers";

        [ConfigurationProperty(ResolversPropertyName)]
        public LogResolverElementCollection Resolvers
        {
            get { return (LogResolverElementCollection)base[ResolversPropertyName]; }
        }

        protected override LogElement CreateSelectNewElement(string elementName)
        {
            var resolver = this.Resolvers.Find(elementName);
            return resolver.GetRuntimeObject<LogElement>();
        }

        internal bool HasResolvers
        {
            get { return this.Resolvers != null && this.Resolvers.Count > 0; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
    }
}

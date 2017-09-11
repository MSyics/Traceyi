using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    [ConfigurationCollection(typeof(LogResolverElement), AddItemName = "add")]
    internal sealed class LogResolverElementCollection : NamedElementCollection<LogResolverElement>
    {
        public LogResolverElementCollection()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BaseAdd(new LogResolverElement() { Name = "console", Type = typeof(ConsoleLogElement).FullName });
            this.BaseAdd(new LogResolverElement() { Name = "file", Type = typeof(FileLogElement).FullName });
            this.BaseAdd(new LogResolverElement() { Name = "rotateFile", Type = typeof(RotateFileLogElement).FullName });
            this.BaseAdd(new LogResolverElement() { Name = "event", Type = typeof(EventLogElement).FullName });
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LogResolverElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogResolverElement)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
    }
}

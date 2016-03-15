using System;
using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    internal class TypedElement : NamedElement
    {
        private const string TypePropertyName = "type";

        [ConfigurationProperty(TypePropertyName, IsRequired = true)]
        public string Type
        {
            get { return (string)this[TypePropertyName]; }
            set { this[TypePropertyName] = value; }
        }

        public object GetRuntimeObject()
        {
            try
            {
                return Activator.CreateInstance(System.Type.GetType(this.Type));
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException("type", e);
            }
        }

        public T GetRuntimeObject<T>()
            where T : ConfigurationElement
        {
            return (T)GetRuntimeObject();
        }
    }
}

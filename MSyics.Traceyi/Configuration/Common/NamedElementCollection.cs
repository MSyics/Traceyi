using System;
using System.Linq;
using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    internal abstract class NamedElementCollection<TElement> : ElementCollection<TElement>
        where TElement : NamedElement
    {
        public bool Exists(string name)
        {
            return this.Any(x => x.Name == name);
        }

        public TElement Find(string name)
        {
            try
            {
                return this.First(x => x.Name == name);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException("name", e);
            }
        }
    }
}

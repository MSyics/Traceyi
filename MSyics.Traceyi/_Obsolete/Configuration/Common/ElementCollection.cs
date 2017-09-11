using System.Collections.Generic;
using System.Configuration;

namespace MSyics.Traceyi.Configuration
{
    internal abstract class ElementCollection<TElement> : ConfigurationElementCollection, IEnumerable<TElement>
        where TElement : ConfigurationElement
    {
        public new IEnumerator<TElement> GetEnumerator()
        {
            foreach (var item in this.BaseGetAllKeys())
            {
                yield return (TElement)this.BaseGet(item);
            }
        }
    }
}

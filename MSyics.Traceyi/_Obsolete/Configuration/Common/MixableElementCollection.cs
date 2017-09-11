using System;
using System.Linq;
using System.Configuration;
using System.Xml;

namespace MSyics.Traceyi.Configuration
{
    internal abstract class MixableElementCollection<TElement> : ElementCollection<TElement>
        where TElement : ConfigurationElement, IConfigurationMixable
    {
        protected abstract TElement CreateSelectNewElement(string elementName);

        protected override ConfigurationElement CreateNewElement()
        {
            throw new ConfigurationErrorsException();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            #region Dore
            if (string.IsNullOrEmpty(elementName)) { throw new ConfigurationErrorsException("elementName"); }
            #endregion

            var element = CreateSelectNewElement(elementName);
            if (element == null)
            {
                throw new ConfigurationErrorsException("elememntName");
            }
            else
            {
                return element;
            }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            #region Dore
            if (!(element is TElement)) { throw new ConfigurationErrorsException("element"); }
            #endregion

            return ((TElement)element).Key;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            #region Dore
            if (string.IsNullOrEmpty(elementName)) { throw new ConfigurationErrorsException("elementName"); }
            if (reader == null) { throw new ConfigurationErrorsException("reader"); }
            #endregion

            var element = CreateNewElement(elementName) as TElement;
            if (element == null)
            {
                return false;
            }
            else
            {
                element.Deserialize(reader);
                BaseAdd(element);
                return true;
            }
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            #region Dore
            if (!(element is IConfigurationMixable)) { throw new ConfigurationErrorsException("element"); }
            #endregion

            if (this.BaseGet(((TElement)element).Key) != null)
            {
                throw new ConfigurationErrorsException(((TElement)element).Key + " element exist");
            }

            base.BaseAdd(element);
        }

        public bool Exists(string key)
        {
            return this.Any(x => x.Key == key);
        }

        public TElement Find(string key)
        {
            try
            {
                return this.First(x => x.Key == key);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException("key", e);
            }
        }
    }
}

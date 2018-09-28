using System;
using System.Configuration;
using System.Reflection;

namespace LineCheckerSrv.LoginCheckersConfig
{
    /// <summary>
    /// Represents a configuration element containing a collection of child elements.
    /// </summary>
    /// <typeparam name="T">ConfigurationElement type</typeparam>
    public class ConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return typeof(T).GetConstructor(new Type[] { }).
                Invoke(new object[] { }) as ConfigurationElement;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(ConfigurationPropertyAttribute), true))
                {
                    ConfigurationPropertyAttribute attribute = 
                        property.GetCustomAttributes(typeof(ConfigurationPropertyAttribute),
                                                    true)[0] as ConfigurationPropertyAttribute;
                    if (attribute != null && attribute.IsKey)
                    {
                        return property.GetValue(element, null);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public new int Count
        {
            get { return base.Count; }
        }

        /// <summary>
        /// Gets the ConfigurationElement at the specified index location.
        /// </summary>
        /// <param name="index">position</param>
        /// <returns>ConfigurationElement</returns>
        public T this[int index]
        {
            get { return (T)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Returns an instance for a specified section name
        /// </summary>
        /// <param name="name">section name</param>
        /// <returns>instance of a specified section</returns>
        new public T this[string name]
        {
            get { return (T)BaseGet(name); }
        }
    }
}

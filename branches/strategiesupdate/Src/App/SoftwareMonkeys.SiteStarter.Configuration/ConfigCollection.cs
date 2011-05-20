using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
    public class ConfigCollection : List<IConfig>
    {
        /// <summary>
        /// Gets/sets the configuration object in the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the configuration object.</param>
        /// <returns>The configuration object with the specified name.</returns>
        public IConfig this[string name]
        {
            get
            {
                foreach (IConfig config in this)
                    if (config.Name == name)
                        return config;
                return null;
            }
            set
            {
                if (value == null)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (this[i].Name == name)
                            RemoveAt(i);
                    }
                }
                else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (this[i].Name == name)
                        {
                            this[i] = value;
                        }
                    }
                }
            }
        }

        private IAppConfig application;
        /// <summary>
        /// Gets/sets the current application level configuration object.
        /// </summary>
        public IAppConfig Application
        {
            get { return application; }
            set { application = value; }
        }
    }
}

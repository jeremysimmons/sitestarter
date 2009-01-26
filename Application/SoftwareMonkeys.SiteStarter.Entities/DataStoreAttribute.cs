using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    public class DataStoreAttribute : Attribute
    {
        private string dataStoreName;
        /// <summary>
        /// Gets/sets the name of the data store that the entity with this attribute is stored in.
        /// </summary>
        public string DataStoreName
        {
            get { return dataStoreName; }
            set { dataStoreName = value; }
        }

        /// <summary>
        /// Sets the name of the data store that the entity with this attribute is stored in.
        /// </summary>
        /// <param name="dataStoreName"></param>
        public DataStoreAttribute(string dataStoreName)
        {
            this.dataStoreName = dataStoreName;
        }
    }
}

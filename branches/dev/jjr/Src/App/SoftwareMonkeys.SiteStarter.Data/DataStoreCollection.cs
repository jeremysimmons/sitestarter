using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Defines the interface required for all data store collections.
    /// </summary>
    public class DataStoreCollection : List<IDataStore>
    {
        // TODO: Remove if not needed
     /*   private DataProvider provider;
        /// <summary>
        /// Gets/sets the data provider.
        /// </summary>
        public DataProvider Provider
        {
            get { return provider; }
            set { provider = value; }
        }*/

      //  public void IDataStore[] this[int index] { get { return this[; }
      //  IDataStore[] this[string dataStoreName] { get; }

        public IDataStore this[string dataStoreName]
        {
            get
            {
                IDataStore store = GetByName(dataStoreName);
                if (store == null)
                {
                    store = DataAccess.Data.InitializeDataStore(dataStoreName);
                    Add(store);
                }
                return store;
            }
        }

        public IDataStore this[Type entityType]
        {
            get
            {
                string dataStoreName = DataUtilities.GetDataStoreName(entityType);
                IDataStore store = GetByName(dataStoreName);
                if (store == null)
                {
                    store = DataAccess.Data.InitializeDataStore(dataStoreName);
                    Add(store);
                }
                return store;
            }
        }

        public DataStoreCollection()
        {
        }

        // TODO: Remove if not needed
     /*   public DataStoreCollection(DataProvider provider)
        {
            this.provider = provider;
        }*/

        public IDataStore GetByName(string dataStoreName)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Name == dataStoreName)
                    return this[i];
            return null;
        }
    }
}

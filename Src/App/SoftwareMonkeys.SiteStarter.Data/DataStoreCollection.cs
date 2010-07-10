using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

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

        public IDataStore this[IEntity entity]
        {
            get
            {
                string dataStoreName = DataUtilities.GetDataStoreName(entity);
                IDataStore store = GetByName(dataStoreName);
                if (store == null)
                {
                    store = DataAccess.Data.InitializeDataStore(dataStoreName);
                    Add(store);
                }
                return store;
            }
        }
        
        public IDataStore this[Type type]
        {
            get
            {
                string dataStoreName = DataUtilities.GetDataStoreName(type);
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
        
        public new void Remove(IDataStore store)
        {
        	store.Dispose();
        	base.Remove(store);
        }
    }
}

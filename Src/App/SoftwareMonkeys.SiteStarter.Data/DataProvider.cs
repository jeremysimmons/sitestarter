using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Defines the interface required for all data providers.
    /// </summary>
    public abstract class DataProvider : ProviderBase
    {
        public abstract DataStoreCollection Stores
        { get; }

       // public abstract void Initialize(string name, NameValueCollection settings);

       // void Dispose();

        public abstract IDataStore InitializeDataStore(string dataStoreName);
        public abstract string GetDataStoreName(Type objectType);
        public abstract string[] GetDataStoreNames();

	public abstract BaseFilter CreateFilter(Type baseType);

	#region Data access functions
        /// <summary>
        /// Retrieves all the entities of the specified type from the data store.
        /// </summary>
	/// <param name="filter">The filter to apply to the query.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public abstract BaseEntity[] GetEntities(BaseFilter filter);

        /// <summary>
        /// Retrieves all the entities of the specified type from the data store.
        /// </summary>
	/// <param name="group">The group of filters to apply to the query.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public abstract BaseEntity[] GetEntities(FilterGroup group);
	#endregion
    }
}

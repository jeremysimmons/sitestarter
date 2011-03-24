using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface required for all data store collections.
	/// </summary>
	public class DataStoreCollection : List<IDataStore>
	{
		
		private bool enableVirtualServers = true;
		/// <summary>
		/// Gets/sets a boolean value indicating whether virtual servers are enabled.
		/// </summary>
		public bool EnableVirtualServers
		{
			get { return enableVirtualServers; }
			set { enableVirtualServers = value; }
		}
		
		/// <summary>
		/// Gets the data store with the specified name and, if virtual servers are enabled, within the current virtual server.
		/// </summary>
		public IDataStore this[string dataStoreName]
		{
			get
			{
				IDataStore store = GetByName(dataStoreName);
				if (store == null || store.IsClosed)
				{
					store = DataAccess.Data.InitializeDataStore(dataStoreName);
					Add(store);
				}
				return store;
			}
		}

		/// <summary>
		/// Gets the data store corresponding with the provided entity.
		/// </summary>
		public IDataStore this[IEntity entity]
		{
			get
			{
				string dataStoreName = DataUtilities.GetDataStoreName(entity);
				return this[dataStoreName];
			}
		}
		
		/// <summary>
		/// Gets the data store corresponding with the provided type.
		/// </summary>
		public IDataStore this[Type type]
		{
			get
			{
				string dataStoreName = DataUtilities.GetDataStoreName(type);
				return this[dataStoreName];
			}
		}


		public DataStoreCollection()
		{
		}
		
		/// <summary>
		/// Retrieves the data store with the specified name and, if virtual servers are enabled, from within the current virtual server.
		/// </summary>
		/// <param name="dataStoreName">The name of the data store to retrieve.</param>
		/// <returns>The data store with the specified name.</returns>
		public IDataStore GetByName(string dataStoreName)
		{
			IDataStore store = null;
			
			for (int i = 0; i < Count; i++)
			{				
				bool nameMatches = dataStoreName.Equals(this[i].Name);
				
				if (nameMatches)
				{
					store = this[i];
				}
			}
			
			return store;
		}
		
		/// <summary>
		/// Removes the provided data store from the collection.
		/// </summary>
		/// <param name="store">The data store to remove from the collection.</param>
		public new void Remove(IDataStore store)
		{
			store.Dispose();
			base.Remove(store);
		}
	}
}

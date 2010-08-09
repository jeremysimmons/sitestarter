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
				return this[VirtualServerState.VirtualServerID, dataStoreName];
			}
		}
		
		/// <summary>
		/// Gets the data store with the specified name and virtual server.
		/// </summary>
		public IDataStore this[string virtualServerID, string dataStoreName]
		{
			get
			{
				IDataStore store = GetByName(virtualServerID, dataStoreName);
				if (store == null || store.IsClosed)
				{
					store = DataAccess.Data.InitializeDataStore(virtualServerID, dataStoreName);
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
		
		/// <summary>
		/// Gets the data store corresponding with the provided type and in the specified virtual server.
		/// </summary>
		public IDataStore this[string virtualServerID, Type type]
		{
			get
			{
				string dataStoreName = DataUtilities.GetDataStoreName(type);
				return this[virtualServerID, dataStoreName];
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
			return GetByName(VirtualServerState.VirtualServerID, dataStoreName);
		}

		/// <summary>
		/// Retrieves the data store with the specified name and, if virtual servers are enabled, from within the current virtual server.
		/// </summary>
		/// <param name="virtualServerID">The ID of the virtual server to get the data store for.</param>
		/// <param name="dataStoreName">The name of the data store to retrieve.</param>
		/// <returns>The data store with the specified name.</returns>
		public IDataStore GetByName(string virtualServerID, string dataStoreName)
		{
			IDataStore store = null;
			
			for (int i = 0; i < Count; i++)
			{
				string fullName = virtualServerID + "--" + dataStoreName;
				
				bool nameMatches = dataStoreName.Equals(this[i].Name);
				bool fullNameMatches = fullName.Equals(this[i].Name);
				
				// If either version of the
				if (nameMatches ||
				    (fullNameMatches && EnableVirtualServers))
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

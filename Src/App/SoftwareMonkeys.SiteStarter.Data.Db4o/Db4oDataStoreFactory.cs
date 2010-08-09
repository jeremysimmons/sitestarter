using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Ext;
using System.Reflection;
using SoftwareMonkeys.SiteStarter;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using Db4objects.Db4o.Config;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Assists in interaction with the db4o database.
	/// </summary>
	public static class Db4oDataStoreFactory
	{
		static public Db4oDataStore InitializeDataStore(string dataStoreName)
		{
			return InitializeDataStore(dataStoreName, Db4oFactory.CloneConfiguration());
		}
		
		static public Db4oDataStore InitializeDataStore(string virtualServerID, string dataStoreName)
		{
			return InitializeDataStore(virtualServerID, dataStoreName, Db4oFactory.CloneConfiguration());
		}
		
		/// <summary>
		/// Loads the data from the .yap file.
		/// </summary>
		static public Db4oDataStore InitializeDataStore(string virtualServerID, string dataStoreName, IConfiguration db4oConfiguration)
		{
			Db4oDataStore store = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing data store.", NLog.LogLevel.Debug))
			{
				if (!Config.IsInitialized)
					throw new InvalidOperationException("The application config file is not present. Run the setup process and try again.");
				
				AppLogger.Debug("Data store name: " + dataStoreName);
				AppLogger.Debug("Virtual server ID: " + virtualServerID);
				
				string fullName = String.Empty;
				
				if (virtualServerID != String.Empty)
				{
					fullName = virtualServerID + "--" + dataStoreName;
				}
				else
					fullName = dataStoreName;
				
				AppLogger.Debug("Full name: " + fullName);
				
				// Create a new data store
				store = new Db4oDataStore(Db4oFactory.CloneConfiguration());
				store.Name = fullName;
			}
			return store;
			
		}
		
		/// <summary>
		/// Loads the data from the .yap file.
		/// </summary>
		static public Db4oDataStore InitializeDataStore(string dataStoreName, IConfiguration db4oConfiguration)
		{
			Db4oDataStore store = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing data store.", NLog.LogLevel.Debug))
			{
				if (!Config.IsInitialized)
					throw new InvalidOperationException("The application config file is not present. Run the setup process and try again.");
				
				AppLogger.Debug("Data store name: " + dataStoreName);
				
				// Create a new data store
				store = new Db4oDataStore(Db4oFactory.CloneConfiguration());
				store.Name = dataStoreName;
			}
			return store;
			
		}
	}
}
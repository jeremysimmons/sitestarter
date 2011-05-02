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
	/// Used to create new data stores.
	/// </summary>
	public static class Db4oDataStoreFactory
	{
		static public Db4oDataStore InitializeDataStore(string dataStoreName)
		{
			return InitializeDataStore(dataStoreName, Db4oFactory.NewConfiguration());
		}
		
		/// <summary>
		/// Initializes the data store.
		/// </summary>
		static public Db4oDataStore InitializeDataStore(string dataStoreName, IConfiguration db4oConfiguration)
		{
			Db4oDataStore store = null;
			
			using (LogGroup logGroup = LogGroup.Start("Initializing data store: " + dataStoreName, NLog.LogLevel.Debug))
			{
				if (!Config.IsInitialized)
					throw new InvalidOperationException("The application config file is not present. Run the setup process and try again.");
				
				LogWriter.Info("Creating new data store: " + dataStoreName);
				
				// Create a new data store
				store = new Db4oDataStore(db4oConfiguration);
				store.Name = dataStoreName;
			}
			return store;
			
		}
	}
}
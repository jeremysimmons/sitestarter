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

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Assists in interaction with the db4o database.
	/// </summary>
	public static class Db4oDataStoreFactory
	{
		/// <summary>
		/// Loads the data from the .yap file.
		/// </summary>
		static public Db4oDataStore InitializeDataStore(string dataStoreName)
		{
			Db4oDataStore store = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing data store.", NLog.LogLevel.Debug))
			{
				if (!Config.IsInitialized)
					throw new InvalidOperationException("The application config file is not present. Run the setup process and try again.");
				
				// Create a new data store
				store = new Db4oDataStore();
				store.Name = dataStoreName;
			}
			return store;
			
		}
	}
}
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

namespace SoftwareMonkeys.SiteStarter.Data
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
            if (!Config.IsInitialized)
                throw new InvalidOperationException("The application config file is not present. Run the setup process and try again.");
           
            // Add messages to the trace
            System.Diagnostics.Trace.WriteLine(Config.Application.PhysicalPath + @"\App_Data\" + dataStoreName + ".yap", "Loading data store: " + dataStoreName);

            if (!Directory.Exists(Path.GetDirectoryName(Config.Application.PhysicalPath + @"\App_Data\")))
                Directory.CreateDirectory(Path.GetDirectoryName(Config.Application.PhysicalPath + @"\App_Data\"));

		// Create a new data store and load the yap file
		Db4oDataStore store = new Db4oDataStore();
		store.Name = dataStoreName;

            return store;
           
        }
    }
}
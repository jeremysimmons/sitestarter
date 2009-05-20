using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Xml.Serialization;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides an interface for interacting with servers.
	/// </summary>
    [DataObject(true)]
	public class VirtualServerFactory
    {
        /// <summary>
        /// Gets the data store containing the objects that this factory interact with.
        /// </summary>
        static public IDataStore DataStore
        {
            get { return DataAccess.Data.Stores[typeof(Entities.VirtualServer)]; }
        }

		#region Retrieve functions
	    /// <summary>
		/// Retrieves all the servers from the DB.
		/// </summary>
		/// <returns>A VirtualServerSet containing the retrieved servers.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
		static public Entities.VirtualServer[] GetVirtualServers()
		{
	        State.VirtualServerState.SuspendVirtualServerState();
	        
			VirtualServer[] servers = (VirtualServer[])Collection<Entities.VirtualServer>.ConvertAll(DataStore.GetEntities(typeof(Entities.VirtualServer)));
			
	        State.VirtualServerState.RestoreVirtualServerState();
	        
	        return servers;
		}

		/// <summary>
		/// Retrieves all the specified servers from the DB.
		/// </summary>
		/// <param name="serverIDs">An array of IDs of servers to retrieve.</param>
		/// <returns>A VirtualServerSet containing the retrieved servers.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public Entities.VirtualServer[] GetVirtualServers(Guid[] serverIDs)
		{
	        State.VirtualServerState.SuspendVirtualServerState();
           
			// Create a new server collection
            Collection<Entities.VirtualServer> servers = new Collection<Entities.VirtualServer>();

			// Loop through the IDs and add each server to the collection
			foreach (Guid serverID in serverIDs)
			{
				if (serverID != Guid.Empty)
					servers.Add(GetVirtualServer(serverID));
			}
			
			State.VirtualServerState.RestoreVirtualServerState();

			// Return the collection
			return servers.ToArray();
		}

		/// <summary>
		/// Retrieves the specified server from the DB.
		/// </summary>
		/// <param name="serverID">The ID of the server to retrieve.</param>
		/// <returns>A VirtualServer object containing the requested info.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public Entities.VirtualServer GetVirtualServer(Guid serverID)
		{
            // If the ID is empty return null
            if (serverID == Guid.Empty)
                return null;
                
	        State.VirtualServerState.SuspendVirtualServerState();

            VirtualServer server = (Entities.VirtualServer)DataStore.GetEntity(typeof(Entities.VirtualServer), "id", serverID);
            
            
	        State.VirtualServerState.RestoreVirtualServerState();
	        
	        return server;
		}

        /// <summary>
        /// Retrieves the servers with the provided name.
        /// </summary>
        static public Entities.VirtualServer[] GetVirtualServersByName(string name)
        {
	        State.VirtualServerState.SuspendVirtualServerState();
	        
            VirtualServer[] servers = (VirtualServer[])Collection<Entities.VirtualServer>.ConvertAll(DataAccess.Data.GetEntities(typeof(Entities.VirtualServer), "Name", name));
            
	        State.VirtualServerState.RestoreVirtualServerState();
	        
	        return servers;
        }


        /// <summary>
        /// Retrieves the server with the provided name.
        /// </summary>
        static public Entities.VirtualServer GetVirtualServerByName(string name)
        {
            State.VirtualServerState.SuspendVirtualServerState();
            
            VirtualServer server = (Entities.VirtualServer)DataStore.GetEntity(typeof(Entities.VirtualServer), "name", name);
            
	        State.VirtualServerState.RestoreVirtualServerState();
	        
	        return server;
        }
		#endregion

		
		#region Save functions
		/// <summary>
		/// Saves the provided server to the DB.
		/// </summary>
		/// <param name="server">The server to save.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        static public bool SaveVirtualServer(Entities.VirtualServer server)
		{
			bool success = false;
	        State.VirtualServerState.SuspendVirtualServerState();
	        
			// Check if the servername is already taken.
			if (VirtualServerNameTaken(server))
			{
				// Save unsuccessful.
				success = false;
			}
			// ... if the servername is NOT taken.
			else
			{
				// Save the object.
				DataStore.Save(server);

				// Save successful.
				success = true;
			}
			
			State.VirtualServerState.RestoreVirtualServerState();
			
			return success;
		}
		#endregion
		
		       /// <summary>
        /// Saves the provided configuration object to file.
        /// </summary>
        /// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
        /// <param name="config">The configuration object to save.</param>
        /// <param name="variation">The variation to be applied to the configuration file (ie. local, staging, etc.).</param>
        static public void SaveConfig(string physicalDataDirectoryPath, IVirtualServerConfig config)
        {
            ConfigFactory.SaveConfig(physicalDataDirectoryPath, config, String.Empty);
        }
        
               /// <summary>
        /// Loads the config file at the specified path.
        /// </summary>
        /// <param name="configPath">The physical path to the config file.</param>
        /// <param name="type">The type of configuration object to load.</param>
        /// <returns>The config from the specified path.</returns>
        static public IVirtualServerConfig LoadConfig(string physicalDataDirectoryPath)
        {
        	return (IVirtualServerConfig)ConfigFactory.LoadConfig(physicalDataDirectoryPath, typeof(IVirtualServerConfig));
        }

		#region Update functions
		/// <summary>
		/// Updates the provided server to the DB.
		/// </summary>
		/// <param name="server">The server to update.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        static public bool UpdateVirtualServer(Entities.VirtualServer server)
		{
		
			State.VirtualServerState.SuspendVirtualServerState();
			
			bool success = false;
			// Check if the servername is already taken.
			if (VirtualServerNameTaken(server))
			{
				// Update unsuccessful.
				success = false;
			}
			// ... if the servername is NOT taken.
			else
			{
				// Update the object.
                		DataStore.Update(server);

				// Update successful.
				success = true;
			}
			
			State.VirtualServerState.RestoreVirtualServerState();
			
			return success;
		}
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided server.
		/// </summary>
		/// <param name="server">The server to delete.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        static public void DeleteVirtualServer(Entities.VirtualServer server)
		{
			State.VirtualServerState.SuspendVirtualServerState();
			
            if (server != null)
            {
                // Check that the server is bound to the DB
                if (!DataStore.IsStored(server))
                    server = GetVirtualServer(server.ID);

                DataStore.Delete(server);
            }
            
			State.VirtualServerState.RestoreVirtualServerState();
		}
		#endregion

		#region Validation functions
		/// <summary>
		/// Checks whether the servername of the provided server is already taken.
		/// </summary>
		/// <param name="server">The server to check the servername of.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
        static public bool VirtualServerNameTaken(Entities.VirtualServer server)
		{
			bool taken = false;
			
			State.VirtualServerState.SuspendVirtualServerState();
		
            using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the servername is unique.", NLog.LogLevel.Info))
            {
                AppLogger.Info("VirtualServer ID: " + server.ID.ToString());
                AppLogger.Info("VirtualServername: " + server.Name);

                // If no servername was specified just skip this function
                if (server.Name == String.Empty)
                    taken = false;

                // Retrieve any existing server with the servername.
                Entities.VirtualServer existing = GetVirtualServerByName(server.Name);

                AppLogger.Info("Found match - VirtualServer ID: " + server.ID.ToString());
                AppLogger.Info("Found match - VirtualServername: " + server.Name);

                taken = (existing != null && existing.ID != server.ID);

                if (taken)
                    AppLogger.Info("VirtualServername has already been taken.");
                else
                    AppLogger.Info("VirtualServername can be used.");
                    
				State.VirtualServerState.RestoreVirtualServerState();

                // If a server was found and the IDs are not the same then it's already taken.
                return taken;
            }
		}
		#endregion
	}
}

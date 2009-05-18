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
            return (VirtualServer[])Collection<Entities.VirtualServer>.ConvertAll(DataStore.GetEntities(typeof(Entities.VirtualServer)));
		}

		/// <summary>
		/// Retrieves all the specified servers from the DB.
		/// </summary>
		/// <param name="serverIDs">An array of IDs of servers to retrieve.</param>
		/// <returns>A VirtualServerSet containing the retrieved servers.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public Entities.VirtualServer[] GetVirtualServers(Guid[] serverIDs)
		{
			// Create a new server collection
            Collection<Entities.VirtualServer> servers = new Collection<Entities.VirtualServer>();

			// Loop through the IDs and add each server to the collection
			foreach (Guid serverID in serverIDs)
			{
				if (serverID != Guid.Empty)
					servers.Add(GetVirtualServer(serverID));
			}

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

            return (Entities.VirtualServer)DataStore.GetEntity(typeof(Entities.VirtualServer), "id", serverID);
		}

        /// <summary>
        /// Retrieves the server with the provided email.
        /// </summary>
        static public Entities.VirtualServer[] GetVirtualServersByEmail(string email)
        {
            return (VirtualServer[])Collection<Entities.VirtualServer>.ConvertAll(DataStore.GetEntities(typeof(Entities.VirtualServer), "email", email));
        }

        /// <summary>
        /// Retrieves the servers with the provided name.
        /// </summary>
        static public Entities.VirtualServer[] GetVirtualServersByName(string name)
        {
            return (VirtualServer[])Collection<Entities.VirtualServer>.ConvertAll(DataStore.GetEntities(typeof(Entities.VirtualServer), "name", name));
        }


        /// <summary>
        /// Retrieves the server with the provided name.
        /// </summary>
        static public Entities.VirtualServer GetVirtualServerByName(string name)
        {
            return (Entities.VirtualServer)DataStore.GetEntity(typeof(Entities.VirtualServer), "name", name);
        }

        /// <summary>
        /// Retrieves the server with the provided email.
        /// </summary>
        static public Entities.VirtualServer GetVirtualServerByEmail(string email)
        {
            return (Entities.VirtualServer)DataStore.GetEntity(typeof(Entities.VirtualServer), "email", email);
        }
		#endregion

		#region Security functions
		/*/// <summary>
		/// Retrieves the server with the specified servername and password.
		/// </summary>
		/// <param name="servername">The servername of the server to retrieve.</param>
		/// <param name="password">The password of the server to retrieve.</param>
		/// <returns>The server with the provided credentials.</returns>
        static public Entities.VirtualServer AuthenticateVirtualServer(string servername, string password)
		{
			Entities.VirtualServer server = null;

			using(LogGroup logGroup = AppLogger.StartGroup("Retrieves the server with the specified servername and password.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("VirtualServername: " + servername);
                  // TODO: Check encrypt password code
			// Encrypt the password if necessary.
			//if (encryptPassword)
				password = Crypter.EncryptPassword(password);
			
			// Create the query
		            Dictionary<string, object> parameters = new Dictionary<string, object>();
		            parameters.Add("servername", servername);
		            parameters.Add("password", password);

			// Retrieve and return the server with the servername and password.
            			server = (Entities.VirtualServer)DataStore.GetEntity(typeof(Entities.VirtualServer), parameters);

				if (server != null)
				{
					AppLogger.Debug("VirtualServer ID: " + server.ID);
					AppLogger.Debug("Name: " + server.Name);
					AppLogger.Debug("Email: " + server.Email);
				}
				else
					AppLogger.Debug("VirtualServer not found...credentials are invalid.");
			}

			return server;
		}*/
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
			// Check if the servername is already taken.
			if (VirtualServerNameTaken(server))
			{
				// Save unsuccessful.
				return false;
			}
			// ... if the servername is NOT taken.
			else
			{
				// Save the object.
				DataStore.Save(server);

				// Save successful.
				return true;
			}
		}
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided server to the DB.
		/// </summary>
		/// <param name="server">The server to update.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        static public bool UpdateVirtualServer(Entities.VirtualServer server)
		{
			// Check if the servername is already taken.
			if (VirtualServerNameTaken(server))
			{
				// Update unsuccessful.
				return false;
			}
			// ... if the servername is NOT taken.
			else
			{
				// Update the object.
                		DataStore.Update(server);

				// Update successful.
				return true;
			}
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
            if (server != null)
            {
                // Check that the server is bound to the DB
                if (!DataStore.IsStored(server))
                    server = GetVirtualServer(server.ID);

                DataStore.Delete(server);
            }
		}
		#endregion

		#region Search functions
		/*/// <summary>
		/// Searches the servers with the provided query.
		/// </summary>
		/// <param name="query">The query to search servers with.</param>
		/// <returns>A VirtualServerSet containing the matching servers.</returns>
        static public Collection<Entities.VirtualServer> SearchVirtualServers(string query)
		{
			// Create a list of searchable properties
			string[] properties = new string[] {"firstName",
												   "lastName",
												   "servername"};

			// Search the servers
            Collection<Entities.VirtualServer> servers = new Collection<Entities.VirtualServer>(Db4oHelper.SearchObjects(typeof(Entities.VirtualServer), properties, query));

			// Return all matching servers
			return servers;
		}*/
		#endregion

		#region Validation functions
		/// <summary>
		/// Checks whether the servername of the provided server is already taken.
		/// </summary>
		/// <param name="server">The server to check the servername of.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
        static public bool VirtualServerNameTaken(Entities.VirtualServer server)
		{
            using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the servername is unique.", NLog.LogLevel.Info))
            {
                AppLogger.Info("VirtualServer ID: " + server.ID.ToString());
                AppLogger.Info("VirtualServername: " + server.Name);

                // If no servername was specified just skip this function
                if (server.Name == String.Empty)
                    return false;

                // Retrieve any existing server with the servername.
                Entities.VirtualServer existing = GetVirtualServerByName(server.Name);

                AppLogger.Info("Found match - VirtualServer ID: " + server.ID.ToString());
                AppLogger.Info("Found match - VirtualServername: " + server.Name);

                bool isTaken = (existing != null && existing.ID != server.ID);

                if (isTaken)
                    AppLogger.Info("VirtualServername has already been taken.");
                else
                    AppLogger.Info("VirtualServername can be used.");

                // If a server was found and the IDs are not the same then it's already taken.
                return isTaken;
            }
		}
		#endregion
	}
}

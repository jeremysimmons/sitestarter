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
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Net.Mail;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides an interface for interacting with servers.
	/// </summary>
	[DataObject(true)]
	public class VirtualServerFactory : VirtualServerFactory<VirtualServer>
	{
		
	}
	
	/// <summary>
	/// Provides an interface for interacting with servers.
	/// </summary>
	[DataObject(true)]
	public class VirtualServerFactory<V> : BaseFactory
		where V : IVirtualServer
	{
		static private VirtualServerFactory<V> current;
		static public VirtualServerFactory<V> Current
		{
			get
			{
				if (current == null)
					current = new VirtualServerFactory<V>();
				return current;
			}
		}

		#region Retrieve functions
		/// <summary>
		/// Retrieves all the servers from the DB.
		/// </summary>
		/// <returns>A VirtualServerSet containing the retrieved servers.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public V[] GetVirtualServers()
		{
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
			V[] servers = (V[])Collection<V>.ConvertAll(DataAccess.Data.Indexer.GetEntities<V>());
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
			
			return servers;
		}

		/// <summary>
		/// Retrieves all the specified servers from the DB.
		/// </summary>
		/// <param name="serverIDs">An array of IDs of servers to retrieve.</param>
		/// <returns>A VirtualServerSet containing the retrieved servers.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public V[] GetVirtualServers(Guid[] serverIDs)
		{
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
			// Create a new server collection
			Collection<V> servers = new Collection<V>();

			// Loop through the IDs and add each server to the collection
			foreach (Guid serverID in serverIDs)
			{
				if (serverID != Guid.Empty)
					servers.Add(GetVirtualServer(serverID));
			}
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();

			// Return the collection
			return servers.ToArray();
		}

		/// <summary>
		/// Retrieves the specified server from the DB.
		/// </summary>
		/// <param name="serverID">The ID of the server to retrieve.</param>
		/// <returns>A VirtualServer object containing the requested info.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public V GetVirtualServer(Guid serverID)
		{
			// If the ID is empty return null
			if (serverID == Guid.Empty)
				return default(V);
			
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();

			V server = (V)DataAccess.Data.Reader.GetEntity<V>("ID", serverID);
			
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
			
			return server;
		}

		/// <summary>
		/// Retrieves the servers with the provided name.
		/// </summary>
		public V[] GetVirtualServersByName(string name)
		{
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
			V[] servers = DataAccess.Data.Indexer.GetEntities<V>("Name", name);
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
			
			return servers;
		}


		/// <summary>
		/// Retrieves the server with the provided name.
		/// </summary>
		public V GetVirtualServerByName(string name)
		{
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
			V server = (V)DataAccess.Data.Reader.GetEntity<V>("Name", name);
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
			
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
		public bool SaveVirtualServer(V server)
		{
			bool success = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided virtual server.", NLog.LogLevel.Debug))
			{
				SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
				
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
					DataAccess.Data.Saver.Save(server);
					
					SaveConfig(Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\VS\" + server.ID.ToString(), server);
					
					// Copy the default sitemap to virtual server
					File.Copy(Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\Menu.default.sitemap", Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\VS\" + server.ID.ToString() + @"\Menu.sitemap", true);

					// Save successful.
					success = true;
				}
				
				SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
			}
			return success;
			
		}
		#endregion
		
		/// <summary>
		/// Saves the provided configuration object to file.
		/// </summary>
		/// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
		/// <param name="config">The configuration object to save.</param>
		/// <param name="variation">The variation to be applied to the configuration file (ie. local, staging, etc.).</param>
		public void SaveConfig(string physicalDataDirectoryPath, V config)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided virtual server configuration file.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Physical data directory path: " + physicalDataDirectoryPath);
				AppLogger.Debug("Configuration name: " + config.Name);
				
				ConfigFactory<V>.SaveConfig(physicalDataDirectoryPath, config, String.Empty);
			}
		}
		
		/// <summary>
		/// Loads the config file at the specified path.
		/// </summary>
		/// <param name="configPath">The physical path to the config file.</param>
		/// <param name="type">The type of configuration object to load.</param>
		/// <returns>The config from the specified path.</returns>
		public V LoadConfig(string physicalDataDirectoryPath)
		{
			return (V)ConfigFactory<V>.LoadConfig(physicalDataDirectoryPath, "VirtualServer", "");
		}

		#region Update functions
		/// <summary>
		/// Updates the provided server to the DB.
		/// </summary>
		/// <param name="server">The server to update.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
		[DataObjectMethod(DataObjectMethodType.Update, true)]
		public bool UpdateVirtualServer(V server)
		{
			
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
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
				DataAccess.Data.Updater.Update(server);
				
				SaveConfig(Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\VS\" + server.ID.ToString(), server);

				// Update successful.
				success = true;
			}
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
			
			return success;
		}
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided server.
		/// </summary>
		/// <param name="server">The server to delete.</param>
		[DataObjectMethod(DataObjectMethodType.Delete, true)]
		public void DeleteVirtualServer(V server)
		{
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
			if (server != null)
			{
				// Check that the server is bound to the DB
				if (!DataAccess.Data.IsStored(server))
					server = GetVirtualServer(server.ID);

				DataAccess.Data.Deleter.Delete(server);
			}
			
			SiteStarter.State.VirtualServerState.RestoreVirtualServerState();
		}
		#endregion

		#region Validation functions
		/// <summary>
		/// Checks whether the servername of the provided server is already taken.
		/// </summary>
		/// <param name="server">The server to check the servername of.</param>
		/// <returns>A boolean value indicating whether the servername is taken.</returns>
		public bool VirtualServerNameTaken(V server)
		{
			bool taken = false;
			
			SiteStarter.State.VirtualServerState.SuspendVirtualServerState();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the servername is unique.", NLog.LogLevel.Info))
			{
				AppLogger.Info("VirtualServer ID: " + server.ID.ToString());
				AppLogger.Info("VirtualServername: " + server.Name);

				// If no servername was specified just skip this function
				if (server.Name == String.Empty)
					taken = false;

				// Retrieve any existing server with the servername.
				V existing = GetVirtualServerByName(server.Name);

				AppLogger.Info("Found match - VirtualServer ID: " + server.ID.ToString());
				AppLogger.Info("Found match - VirtualServername: " + server.Name);

				taken = (existing != null && existing.ID != server.ID);

				if (taken)
					AppLogger.Info("VirtualServername has already been taken.");
				else
					AppLogger.Info("VirtualServername can be used.");
				
				SiteStarter.State.VirtualServerState.RestoreVirtualServerState();

				// If a server was found and the IDs are not the same then it's already taken.
				return taken;
			}
		}
		#endregion

		/// <summary>
		/// Sends the welcome email.
		/// </summary>
		/// <param name="server">The virtual server to send the email for.</param>
		/// <param name="subject">The subject of the welcome email.</param>
		/// <param name="body">The body of the welcome email.</param>
		/// <param name="systemAdministrator">The system administrator.</param>
		public void SendWelcomeEmail(V server, string subject, string body, Entities.IUser systemAdministrator)
		{
			if (systemAdministrator == null)
				throw new InvalidOperationException("The system administrator could not be found with ID " + Config.Application.PrimaryAdministratorID);

			Activate(server, "PrimaryAdministrator");

			if (server.PrimaryAdministrator == null)
				throw new InvalidOperationException("The primary administrator ID isn't specified for the virtual server.");
			
			//if (server.PrimaryAdministrator == null)
			//	server.PrimaryAdministrator = UserFactory.GetUser(server.PrimaryAdministratorID);
			
			if (server.PrimaryAdministrator == null)
				throw new InvalidOperationException("The administrator of the virtual server hasn't been set.");

			string from = systemAdministrator.Email;
			string to = server.PrimaryAdministrator.Email;

			MailMessage message = new MailMessage(from,
			                                      to,
			                                      subject,
			                                      body);

			message.IsBodyHtml = true;

			new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"]).Send(message);
		}
		/// <summary>
		/// Sends the registration alert.
		/// </summary>
		/// <param name="server">The virtual server to send the email for.</param>'
		/// <param name="subject">The subject of the registration alert email.</param>
		/// <param name="body">The body of the registration alert email.</param>
		/// <param name="systemAdministrator">The system administrator.</param>
		public void SendRegistrationAlert(V server, string subject, string body, Entities.IUser systemAdministrator)
		{


			if (server.PrimaryAdministrator == null)
				VirtualServerFactory.Current.Activate(server, "PrimaryAdministrator");

			if (server.PrimaryAdministrator == null)
				throw new InvalidOperationException("The administrator of the virtual server could not be found.");
			
			string from = systemAdministrator.Email;
			string to = server.PrimaryAdministrator.Email;
			

			MailMessage message = new MailMessage(from,
			                                      to,
			                                      subject,
			                                      body);

			message.IsBodyHtml = true;

			new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"]).Send(message);

		}
	}
}

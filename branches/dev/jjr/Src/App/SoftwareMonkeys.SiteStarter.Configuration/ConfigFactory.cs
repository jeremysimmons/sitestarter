using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Contains functions for managing application and module configuration objects/files.
	/// </summary>
	public class ConfigFactory<T>
		where T : IConfig
	{
		#region Load functions

		/// <summary>
		/// Loads the config file at the specified path.
		/// </summary>
		/// <param name="configPath">The physical path to the config file.</param>
		/// <param name="type">The type of configuration object to load.</param>
		/// <returns>The config from the specified path.</returns>
		static public T LoadConfig(string configPath, string name, string variation)
		{
			if (configPath.ToLower().IndexOf(".config") == -1)
				configPath = CreateConfigPath(configPath, name, variation);
			
			T config = default(T);
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading configuration file: " + configPath))
			{

				if (!File.Exists(configPath))
				{
					AppLogger.Debug("Configuration file not found.");
					return default(T);
				}
				
				try
				{
					using (FileStream stream = File.Open(configPath, FileMode.Open))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(T));
						
						config = (T)serializer.Deserialize(stream);
						
						stream.Close();
					}
				}
				catch (Exception ex)
				{
					//DataLogger.Error("Loading configuration file: " + configPath);
					
					throw ex;
					// TODO: Add error handling
				}
			}


			return config;
		}
		#endregion

		#region Create functions
		/// <summary>
		/// Creates a new instance of the specified configuration object.
		/// </summary>
		/// <param name="name">The name of the configuration file or module (excluding the file extension).</param>
		/// <param name="configType">The type of configuration object to instantiate.</param>
		/// <returns>An instance of the specified configuration type.</returns>
		static public T NewConfig(string name)
		{
			T config = (T)Activator.CreateInstance(typeof(T));

			config.Name = name;

			return config;
		}
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided configuration object to file.
		/// </summary>
		/// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
		/// <param name="config">The configuration object to save.</param>
		static public void SaveConfig(string physicalDataDirectoryPath, T config)
		{
			SaveConfig(physicalDataDirectoryPath, config, String.Empty);
		}
		
		/*/// <summary>
		/// Saves the provided configuration object to file.
		/// </summary>
		/// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
		/// <param name="config">The configuration object to save.</param>
		static public void SaveConfig<T>(string physicalDataDirectoryPath, IConfig config)
			where T : IConfig
		{
			SaveConfig<T>(physicalDataDirectoryPath, config, String.Empty);
		}
		
		/// <summary>
		/// Saves the provided configuration object to file.
		/// </summary>
		/// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
		/// <param name="config">The configuration object to save.</param>
		/// <param name="variation">The variation to be applied to the configuration file (ie. local, staging, etc.).</param>
		static public void SaveConfig<T>(string physicalDataDirectoryPath, IConfig config, string variation)
			where T : IConfig
		{
			SaveConfig(physicalDataDirectoryPath, typeof(T), config, variation);
		}*/
			
			/// <summary>
			/// Saves the provided configuration object to file.
			/// </summary>
			/// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
			/// <param name="config">The configuration object to save.</param>
			/// <param name="variation">The variation to be applied to the configuration file (ie. local, staging, etc.).</param>
			static public void SaveConfig(string physicalDataDirectoryPath, T config, string variation)
		{			
			using (LogGroup logGroup = AppLogger.StartGroup("Saving configuration file.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Physical data directory path: " + physicalDataDirectoryPath);
				AppLogger.Debug("Config type: " + config == null ? "[null]" : config.GetType().ToString());
				AppLogger.Debug("Variation: " + variation);
				AppLogger.Debug("Config name: " + config.Name);
				
				// Check if it's the application level configuration object
				if (config is IAppConfig)
				{
					AppLogger.Debug("The configuration object is derived from IAppConfig. Setting it to Config.Application.");
					Config.Application = (IAppConfig)config;
				}

				string name = config.Name;
				config.PathVariation = variation;
				
				// All virtual server config files should be named VirtualServer.config because they have separate directories
				if (config is IVirtualServerConfig)
				{
					AppLogger.Debug("The configuration object is derived from IVirtualServerConfig. Setting the name of it to 'VirtualServer'.");
					
					name = "VirtualServer";
				}
				
				string configPath = CreateConfigPath(physicalDataDirectoryPath, name, variation);
				
				AppLogger.Debug("Configuration path: " + configPath);

				if (!Directory.Exists(physicalDataDirectoryPath))
					Directory.CreateDirectory(physicalDataDirectoryPath);

				using (FileStream stream = File.Create(configPath))
				{
					AppLogger.Debug("Created configuration file stream");
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					serializer.Serialize(stream, config);
					AppLogger.Debug("Serialized config to file");
					stream.Close();
				}
				
				Config.All.Add(config);
				
				AppLogger.Debug("Configuration file added to Config.All.");
			}
		}
		#endregion

		#region Utility functions
		/// <summary>
		/// Creates the full physical path to the specified configuration file.
		/// </summary>
		/// <param name="physicalDataDirectoryPath">The physical path to the data directory.</param>
		/// <param name="configName">The name of the configuration file (excluding the extension).</param>
		/// <param name="variation">The file name variation (ie. local, staging, etc.). Note: Live configuration files should have a String.Empty variation.</param>
		/// <returns>The full physical path to the specified configuration file.</returns>
		static public string CreateConfigPath(string physicalDataDirectoryPath, string configName, string variation)
		{
			// TODO: Remove commented code
			/*string fileName;
            if (HttpContext.Current.Request.Url.Host == "localhost" || HttpContext.Current.Request.Url.Host == "127.0.0.1")
                fileName = "WorkHub.local.config";
            else if (HttpContext.Current.Request.Url.Host.ToLower().IndexOf("staging") > -1)
                fileName = "WorkHub.staging.config";
            else
                fileName = "WorkHub.config";
            physicalConfigPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/Data/" + fileName);*/

			// Start with the physical data directory path (and trim the slash off the end)
			string path = physicalDataDirectoryPath.TrimEnd('\\');

			//string virtualServerName = (string)State.StateAccess.State.GetSession("VirtualServerID");
			//if (virtualServerName != null && virtualServerName != String.Empty)
			//    path += @"\" + virtualServerName;

			// Add the config name
			path += @"\" + configName;

			// If a variation was specified then add it
			if (variation != null && variation != String.Empty)
				path += "." + variation;

			// Add the file extension to the end
			path += ".config";

			// Return the file path
			return path;
		}

		#endregion
	}
}
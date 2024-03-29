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
		/// <param name="dataDirectoryPath">The physical path to the data directory.</param>
		/// <param name="name">The short name of the configuration file (excluding variation and extension).</param>
		/// <param name="variation">The variation applied to the configuration file name (eg. local or staging).</param>
		/// <returns>The config from the specified path.</returns>
		static public T LoadConfig(string dataDirectoryPath, string name, string variation)
		{
			if (dataDirectoryPath.ToLower().IndexOf(".config") > -1)
				throw new ArgumentException("The specified directory should not include the file name.");
			
			string configPath = CreateConfigPath(dataDirectoryPath, name, variation);
			
			T config = default(T);
			
			using (LogGroup logGroup = LogGroup.StartDebug("Loading configuration file: " + configPath))
			{

				if (!File.Exists(configPath))
				{
					// Exception not needed. Just return a default config to allow the setup page to run
					//throw new Exception("Configuration file not found: " + configPath);
					LogWriter.Debug("Configuration file not found.");
					config = default(T);
				}
				else
				{
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
				
				if (config != null)
				{
					config.FilePath = configPath;
					config.Saver = new ConfigSaver();
				}
			}


			return config;
		}
		#endregion

		#region Create functions
		/// <summary>
		/// Creates a new instance of the specified configuration object.
		/// </summary>
		/// <param name="physicalDataDirectoryPath">The physical path to the directory containing application data.</param>
		/// <param name="name">The name of the configuration file or module (excluding the file extension).</param>
		/// <param name="variation">The variation in the configuration file name.</param>
		/// <returns>An instance of the specified configuration type.</returns>
		static public T NewConfig(string physicalDataDirectoryPath, string name, string variation)
		{
			T config = (T)Activator.CreateInstance(typeof(T));

			config.Name = name;
			
			config.Saver = new ConfigSaver();
			config.FilePath = CreateConfigPath(physicalDataDirectoryPath, name, variation);
			config.PathVariation = variation;

			return config;
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
			// Start with the physical data directory path (and trim the slash off the end)
			string path = physicalDataDirectoryPath.TrimEnd('\\');

			//string virtualServerName = (string)State.StateAccess.State.GetSession("VirtualServerID");
			//if (virtualServerName != null && virtualServerName != String.Empty)
			//    path += @"\" + virtualServerName;

			// Add the config name
			path = path + Path.DirectorySeparatorChar + configName;

			// If a variation was specified then add it
			if (variation != null && variation != String.Empty)
				path = path + "." + variation;

			// Add the file extension to the end
			path = path + ".config";
			
			// Return the file path
			return path;
		}

		#endregion
	}
}

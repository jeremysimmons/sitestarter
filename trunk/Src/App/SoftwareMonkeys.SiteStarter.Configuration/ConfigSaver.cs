using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Saves configuration components to file.
	/// </summary>
	public class ConfigSaver : IConfigSaver
	{
		/// <summary>
		/// Saves the provided configuration component to file.
		/// </summary>
		/// <param name="config">The configuration component to save to file.</param>
		public virtual void Save(IConfig config)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving configuration file.", NLog.LogLevel.Debug))
			{
				if (config == null)
					throw new ArgumentNullException("config");
				
				if (config.FilePath == String.Empty)
					throw new ArgumentException("The provided configuration file path must be specified.", "config.FilePath");
				
				AppLogger.Debug("Config file path: " + config.FilePath);
				AppLogger.Debug("Config type: " + config == null ? "[null]" : config.GetType().ToString());
				AppLogger.Debug("Config name: " + config.Name);
				
				// Check if it's the application level configuration object
				if (config is IAppConfig)
				{
					AppLogger.Debug("The configuration object is derived from IAppConfig. Setting it to Config.Application.");
					Config.Application = (IAppConfig)config;
				}

				if (config.Name == String.Empty)
					throw new ArgumentException("The config.Name property must be set.");
				
				string name = config.Name;
				
				// All virtual server config files should be named VirtualServer.config because they have separate directories
				if (config is IVirtualServerConfig)
				{
					AppLogger.Debug("The configuration object is derived from IVirtualServerConfig. Setting the name of it to 'VirtualServer'.");
					
					name = "VirtualServer";
				}
				
				string configPath = config.FilePath;
				
				AppLogger.Debug("Configuration path: " + configPath);

				if (!Directory.Exists(Path.GetDirectoryName(config.FilePath)))
					Directory.CreateDirectory(Path.GetDirectoryName(config.FilePath));

				using (FileStream stream = File.Create(configPath))
				{
					AppLogger.Debug("Created configuration file stream");
					XmlSerializer serializer = new XmlSerializer(config.GetType());
					serializer.Serialize(stream, config);
					AppLogger.Debug("Serialized config to file");
					stream.Close();
				}
				
				Config.All.Add(config);
				
				AppLogger.Debug("Configuration file added to Config.All.");
			}
		}
	}
}

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
			using (LogGroup logGroup = LogGroup.Start("Saving configuration file.", NLog.LogLevel.Debug))
			{
				if (config == null)
					throw new ArgumentNullException("config");
				
				if (config.FilePath == String.Empty)
					throw new ArgumentException("The provided configuration file path must be specified.", "config.FilePath");
				
				LogWriter.Debug("Config file path: " + config.FilePath);
				LogWriter.Debug("Config type: " + config == null ? "[null]" : config.GetType().ToString());
				LogWriter.Debug("Config name: " + config.Name);
				
				// Check if it's the application level configuration object
				if (config is IAppConfig)
				{
					LogWriter.Debug("The configuration object is derived from IAppConfig. Setting it to Config.Application.");
					Config.Application = (IAppConfig)config;
				}

				if (config.Name == String.Empty)
					throw new ArgumentException("The config.Name property must be set.");
				
				string name = config.Name;
				
				string configPath = config.FilePath;
				
				LogWriter.Debug("Configuration path: " + configPath);

				if (!Directory.Exists(Path.GetDirectoryName(config.FilePath)))
					Directory.CreateDirectory(Path.GetDirectoryName(config.FilePath));

				using (FileStream stream = File.Create(configPath))
				{
					LogWriter.Debug("Created configuration file stream");
					XmlSerializer serializer = new XmlSerializer(config.GetType());
					serializer.Serialize(stream, config);
					LogWriter.Debug("Serialized config to file");
					stream.Close();
				}
				
				Config.All.Add(config);
				
				LogWriter.Debug("Configuration file added to Config.All.");
			}
		}
	}
}

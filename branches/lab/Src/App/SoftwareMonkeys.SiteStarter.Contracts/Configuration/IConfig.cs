using System;
using System.Resources;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Defines the configuration settings required for backend operations.
	/// </summary>
	public interface IConfig
	{		
		/// <summary>
		/// The name of the config/module file (excluding the extension).
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// The variation applied to the config file path (eg. staging, local, etc.).
		/// </summar>
		string PathVariation { get;set;}
		
		/// <summary>
		/// Retrieves the file path corresponding with the configuration component.
		/// </summary>
		string FilePath {get;set;}

		/// <summary>
		/// Gets/sets the saver component used to save config settings to file.
		/// </summary>
		IConfigSaver Saver {get;set;}
		
		/// <summary>
		/// Saves the configuration component to file.
		/// </summary>
		void Save();
	}
}

using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Defines the interface of all config saver components.
	/// </summary>
	public interface IConfigSaver
	{
		/// <summary>
		/// Saves the configuration settings to the corresponding file.
		/// </summary>
		/// <param name="config">The configuration component to save.</param>
		void Save(IConfig config);
	}
}

using System;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Provides utilities for dealing with configuration settings (particularly Web.config).
	/// </summary>
	public static class ConfigurationUtilities
	{
		
		/// <summary>
		/// Retrieves the setting with the specified key or, if not found, the default value.
		/// </summary>
		/// <param name="key">The configuration settings key.</param>
		/// <param name="defaultValue">The default value to use if the setting is not found.</param>
		/// <returns>The setting with the specified key, or if it's not found then the default value.</returns>
		static public string GetSetting(string key, string defaultValue)
		{
			string value = String.Empty;
			if (ConfigurationSettings.AppSettings[key] != null)
				value = ConfigurationSettings.AppSettings[key];
			else
				value = defaultValue;
			
			return value;
		}
	}
}

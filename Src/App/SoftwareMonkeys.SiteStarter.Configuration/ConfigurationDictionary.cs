using System;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of ConfigurationDictionary.
	/// </summary>
	public class ConfigurationDictionary : SerializableDictionary<string, object>, IConfigurationDictionary
	{
		public ConfigurationDictionary()
		{
		}
		
		public ConfigurationDictionary(IConfigurationDictionary dict)
		{
			foreach (string key in dict.GetKeys())
			{
				this.Add(key, dict[key]);
			}
		}
		
		public string[] GetKeys()
		{
			List<string> keys = new List<string>();
			foreach (string key in this.Keys)
			{
				keys.Add(key);
			}
			return keys.ToArray();
		}
	}
}

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
		
		/// <summary>
		/// Retrieves a string value from the dictionary.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetString(string key)
		{
			if (ContainsKey(key))
				return (string)this[key];
			else
				return String.Empty;
		}
		
		/// <summary>
		/// Retrieves a bool value from the dictionary.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool GetBool(string key)
		{
			if (ContainsKey(key))
				return (bool)this[key];
			else
				return false;
		}

		/// <summary>
		/// Retrieves an integer value from the dictionary.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int GetInt(string key)
		{
			if (ContainsKey(key))
				return (int)this[key];
			else
				return 0;
		}
	}
}

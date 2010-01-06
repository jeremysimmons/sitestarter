using System;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of IConfigurationDictionary.
	/// </summary>
	public interface IConfigurationDictionary
	{	
		int Count { get; }
		
		//string[] Keys {get;}
		//object[] Values{get; }
		
		object this[string key] {get;set;}
		
		void Add(string key, object value);
		
		bool ContainsKey(string key);
		string[] GetKeys();
	}
}

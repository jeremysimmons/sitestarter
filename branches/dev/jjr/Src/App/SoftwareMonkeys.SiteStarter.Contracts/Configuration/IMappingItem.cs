using System;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of IMappingItem.
	/// </summary>
	public interface IMappingItem
	{
		string TypeName { get;set; }
		IConfigurationDictionary Settings { get;set;}
	}
}

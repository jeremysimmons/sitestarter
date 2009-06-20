/*
 * Created by SharpDevelop.
 * User: John
 * Date: 11/06/2009
 * Time: 12:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of IMappingItem.
	/// </summary>
	[Serializable]
	public interface IMappingItem
	{
		string TypeName { get;set; }
		SerializableDictionary Settings { get;set;}
		
	}
}

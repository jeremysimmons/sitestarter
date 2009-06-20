/*
 * Created by SharpDevelop.
 * User: John
 * Date: 11/06/2009
 * Time: 12:20 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of MappingItem.
	/// </summary>
	public class MappingItem
	{
		private string typeName;
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private ConfigurationDictionary settings = new ConfigurationDictionary();
		/// <summary>
		/// The mapping settings applied to this item.
		/// </summary>
		public ConfigurationDictionary Settings
		{
			get { return settings; }
			set { settings = value; }
		}
		
		public MappingItem()
		{
		}
		
		public MappingItem(Type entityType)
		{
			typeName = entityType.ToString();
			
		}
	}
}

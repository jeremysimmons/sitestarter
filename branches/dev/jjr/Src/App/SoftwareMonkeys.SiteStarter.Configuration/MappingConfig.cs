/*
 * Created by SharpDevelop.
 * User: John
 * Date: 11/06/2009
 * Time: 11:34 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of MappingConfig.
	/// </summary>
   // [XmlType(Namespace = "urn:SoftwareMonkeys.SiteStarter.Configuration.MappingConfig")]
   //[XmlRoot(Namespace = "urn:SoftwareMonkeys.SiteStarter.Configuration")]
   [Serializable]
	public class MappingConfig : IConfig
	{		
		private string name = "Mappings";
		/// <summary>
		/// Gets/sets the name of the configuration file.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		private string pathVariation;
		/// <summary>
		/// Gets/sets the variation applied to the config file path (eg. staging, local, etc.).
		/// </summary>
		public string PathVariation
		{
			get { return pathVariation; }
			set { pathVariation = value; }
		}
		
		private MappingItem[] items = new MappingItem[]{};
		/// <summary>
		/// Gets/sets the mapping settings.
		/// </summary>
		public MappingItem[] Items
		{
			get { return items; }
			set { items = value; }
		}
		
		public MappingConfig()
		{}
		
		public void AddItem(MappingItem item)
		{
			List<MappingItem> list = (items == null ? new List<MappingItem>() : new List<MappingItem>(items));
			bool exists = false;
			foreach (MappingItem t in Items)
			{
				if (t.TypeName == item.TypeName)
					exists = true;
			}
			if (!exists)
				list.Add(item);
			
			items = (MappingItem[])list.ToArray();
		}
		
		public MappingItem GetItem(Type type)
		{
			foreach (MappingItem item in Items)
			{
				// Match either type name or full name
				if (MatchesType(item, type))
				{
					return item;
				}
			}
			return null;
		}
		
		private bool MatchesType(MappingItem item, Type type)
		{
			if (item.TypeName == type.ToString()
			    || item.TypeName == type.Name)
				return true;
			
			Type[] interfaces = type.GetInterfaces();
			foreach (Type i in interfaces)
			{
				if (i.Name == item.TypeName
				    || i.ToString() == item.TypeName)
				return true;
			}
			
			return false;
		}
	}
}

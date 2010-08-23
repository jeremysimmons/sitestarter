using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Xml;
using System.Xml.Schema;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of MappingConfig.
	/// </summary>
	// [XmlType(Namespace = "urn:SoftwareMonkeys.SiteStarter.Configuration.MappingConfig")]
	//[XmlRoot(Namespace = "urn:SoftwareMonkeys.SiteStarter.Configuration")]
	[Serializable]
	//[XmlType("SiteStarter.MappingConfig")]
	public class MappingConfig : BaseConfig, IConfig
	{
		public MappingItem this[string name]
		{
			get
			{
				return GetItem(name, true);
			}
		}
		
		private string name = "Mappings";
		/// <summary>
		/// Gets/sets the name of the configuration file.
		/// </summary>
		public new string Name
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
		
		
		private MappingItem[] items;
		/// <summary>
		/// Gets/sets the mapping settings.
		/// </summary>
		public virtual MappingItem[] Items
		{
			get { return items; }
			set { items = value; }
		}
		
		/*/// <summary>
		/// Gets/sets the mapping settings.
		/// </summary>
		IMappingItem[] IMappingConfig.Items
		{
			get { return new List<IMappingItem>(items).ToArray(); }
			//set { items = new List(value).ToArray(typeof(MappingItem)); }
		}*/
			
			public MappingConfig()
		{}
		
		public void AddItem(MappingItem item)
		{
			if (item == null)
				return;
			
			// NOTE: Logging is commented out simply to reduce size of log.
			// Should be kept in case it's needed for debugging at some point.
			//using (LogGroup logGroup = AppLogger.StartGroup("Adding mapping item to mapping config.", NLog.LogLevel.Debug))
			//{
			//	if (Items != null)
			//		AppLogger.Debug("Existing items: " + Items.Length);
			//	else
			//		AppLogger.Debug("Items collection is null. This is first item.");
			
			bool exists = false;
			
			List<MappingItem> list = new List<MappingItem>();
			
			if (Items != null && Items.Length > 0)
			{
				foreach (MappingItem i in Items)
					list.Add(i);

				foreach (MappingItem i in list)
				{
					if (i.TypeName == item.TypeName)
					{
						exists = true;
						//					AppLogger.Debug("Item already exists.");
					}
					else
					{
						exists = false;
					}
				}
			}
			if (!exists)
			{
				//		AppLogger.Debug("Adding item to the list.");
				list.Add(item);
			}
			
			Items = list.ToArray();
			//}
		}
		
		public MappingItem GetItem(Type type, bool includeInterfaces)
		{
			MappingItem returnValue = null;//default(I);
			// NOTE: Logging is commented out simply to reduce size of log.
			// Should be kept in case it's needed for debugging at some point.
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving item for the type: " + type.ToString(), NLog.LogLevel.Debug))
			//{
			if (type == null)
				throw new ArgumentNullException("type");
			
			if (Items == null || Items.Length == 0)
				returnValue = null;//default(I);
			else
			{
				foreach (MappingItem item in Items)
				{
					//if (item is I)
					//{
					// Match either type name or full name
					if (MatchesType(item, type, includeInterfaces))
					{
						returnValue = item;
					}
					//}
				}
			}
			//}
			return returnValue;
		}
		
		public MappingItem GetItem(string name, bool includeInterfaces)
		{
			MappingItem returnValue = null;//default(I);
			
			// NOTE: Logging is commented out simply to reduce size of log.
			// Should be kept in case it's needed for debugging at some point.
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving item for the name: " + name, NLog.LogLevel.Debug))
			//{
			if (name == null || name == String.Empty || items == null)
				returnValue = null;//default(I);
			else
			{
				foreach (MappingItem item in Items)
				{
					//if (item is I)
					//{
					// Match either type name or full name
					if (MatchesType(item, name, includeInterfaces))
					{
						//				AppLogger.Debug("Found item with type name: " + ((I)item).TypeName);
						returnValue = item;
					}
					//}
				}
			}
			return returnValue;
		}
		
		private bool MatchesType(MappingItem item, Type type, bool includeInterfaces)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			if (item.TypeName == type.ToString()
			    || item.TypeName == type.Name)
				return true;
			
			if (includeInterfaces)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type i in interfaces)
				{
					if (i.Name == item.TypeName
					    || i.ToString() == item.TypeName)
						return true;
				}
			}
			
			return false;
		}
		
		
		private bool MatchesType(MappingItem item, string name, bool includeInterfaces)
		{
			bool flag = false;
			
			// Logging disabled to reduce size of log.
			//using (LogGroup logGroup = AppLogger.StartGroup("Matching mapping item with name: " + name, NLog.LogLevel.Debug))
			//{
			//	AppLogger.Debug("Item type name: " + item.TypeName);
			
			if (item == null)
				throw new ArgumentNullException("item");
			
			if (item.TypeName == name)
			{
				//		AppLogger.Debug("Matches");
				flag = true;
			}
			else
			{
				//		AppLogger.Debug("No match");
				flag = false;
			}
			//}
			
			return flag;
		}
		
		public void RemoveItem(MappingItem item)
		{
			if (item == null)
				return;
			
			// NOTE: Logging is commented out simply to reduce size of log.
			// This commented code should be kept in case it's needed for debugging at some point.
			//using (LogGroup logGroup = AppLogger.StartGroup("Adding mapping item to mapping config.", NLog.LogLevel.Debug))
			//{
			//	if (Items != null)
			//		AppLogger.Debug("Existing items: " + Items.Length);
			//	else
			//		AppLogger.Debug("Items collection is null. This is first item.");
			
			bool exists = false;
			
			List<MappingItem> list = new List<MappingItem>();
			
			if (Items != null && Items.Length > 0)
			{
				foreach (object t in Items)
					list.Add((MappingItem)t);

				foreach (MappingItem t in list)
				{
					//if (t is I)
					//{
					if (t.TypeName == item.TypeName)
					{
						exists = true;
						//					AppLogger.Debug("Item already exists.");
					}
					else
					{
						exists = false;
					}
					//}
				}
			}
			if (exists)
			{
				//		AppLogger.Debug("Adding item to the list.");
				list.Remove(item);
			}
			
			Items = list.ToArray();
			//}
		}
		
		/*void IXmlSerializable.ReadXml ( XmlReader reader )
		{
			// Name
			Name = reader.ReadElementString("Name");
			
			// PathVariation
			PathVariation = reader.ReadElementString("PathVariation");
			
			// Items
			reader.ReadStartElement("Items");
			//string strType = reader.GetAttribute("type");
			List<IMappingItem> list = new List<IMappingItem>();
			while (reader.Read())
			{
				XmlSerializer serial = new XmlSerializer(typeof(MappingItem));
				list.Add((MappingItem)serial.Deserialize(reader));
			}
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml ( XmlWriter writer )
		{
			// Name
			writer.WriteElementString("Name", Name);
			
			// PathVariation
			writer.WriteElementString("PathVariation", PathVariation);
			
			// Items
			writer.WriteStartElement("Items");
			//writer.WriteAttributeString("type", strType);
			foreach (IMappingItem item in Items)
			{
				XmlSerializer serial = new XmlSerializer(item.GetType());
				serial.Serialize(writer, item);
			}
			writer.WriteEndElement();
		}
		
		public XmlSchema GetSchema()
		{
			return(null);
		}*/
		
	}
}

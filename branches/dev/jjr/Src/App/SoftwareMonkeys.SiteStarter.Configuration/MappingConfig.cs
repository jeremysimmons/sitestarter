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
	public class MappingConfig : IMappingConfig, IXmlSerializable
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
		
		private IMappingItem[] items;
		/// <summary>
		/// Gets/sets the mapping settings.
		/// </summary>
		public virtual IMappingItem[] Items
		{
			get { return items; }
			set { items = value; }
		}
		
		public MappingConfig()
		{}
		
		public void AddItem<I>(I item)
			where I : IMappingItem
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
			
			List<IMappingItem> list = new List<IMappingItem>();
			
			if (Items != null && Items.Length > 0)
			{
				foreach (object t in Items)
					list.Add((IMappingItem)t);

				foreach (object t in list)
				{
					if (t is I)
					{
						if (((I)t).TypeName == item.TypeName)
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
			}
			if (!exists)
			{
				//		AppLogger.Debug("Adding item to the list.");
				list.Add(item);
			}
			
			Items = list.ToArray();
			//}
		}
		
		public I GetItem<I>(Type type, bool includeInterfaces)
			where I : IMappingItem
		{
			I returnValue = default(I);
			// NOTE: Logging is commented out simply to reduce size of log.
			// Should be kept in case it's needed for debugging at some point.
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving item for the type: " + type.ToString(), NLog.LogLevel.Debug))
			//{
			if (type == null)
				throw new ArgumentNullException("type");
			
			if (Items == null || Items.Length == 0)
				returnValue = default(I);
			else
			{
				foreach (object item in Items)
				{
					if (item is I)
					{
						// Match either type name or full name
						if (MatchesType<I>((I)item, type, includeInterfaces))
						{
							returnValue = (I)item;
						}
					}
				}
			}
			//}
			return returnValue;
		}
		
		public I GetItem<I>(string name, bool includeInterfaces)
			where I : IMappingItem
		{
			I returnValue = default(I);
			
			// NOTE: Logging is commented out simply to reduce size of log.
			// Should be kept in case it's needed for debugging at some point.
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving item for the name: " + name, NLog.LogLevel.Debug))
			//{
			if (name == null || name == String.Empty || items == null)
				returnValue = default(I);
			
			foreach (object item in Items)
			{
				if (item is I)
				{
					// Match either type name or full name
					if (MatchesType<I>((I)item, name, includeInterfaces))
					{
						//				AppLogger.Debug("Found item with type name: " + ((I)item).TypeName);
						returnValue = (I)item;
					}
				}
			}
			//	AppLogger.Debug("No item found.");
			//}
			return returnValue;
		}
		
		private bool MatchesType<I>(I item, Type type, bool includeInterfaces)
			where I : IMappingItem
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
		
		
		private bool MatchesType<I>(I item, string name, bool includeInterfaces)
			where I : IMappingItem
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
		
		void IXmlSerializable.ReadXml ( XmlReader reader )
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
		}
	}
}

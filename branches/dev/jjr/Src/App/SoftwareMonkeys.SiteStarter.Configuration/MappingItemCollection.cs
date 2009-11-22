using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of MappingItemCollection.
	/// </summary>
	[Serializable]
	public class MappingItemCollection : SerializableDictionary<string, IMappingItem>
	{
		public MappingItemCollection()
		{
		}
		
		/*void IXmlSerializable.ReadXml ( XmlReader reader )
		{
			// Name
			Name = reader.ReadElementString("Name");
			
			// PathVariation
			PathVariation = reader.ReadElementString("PathVariation");
			
			// Items
			reader.ReadStartElement("Items");
			string strType = reader.GetAttribute("type");
			XmlSerializer serial = new XmlSerializer(Type.GetType(strType));
			Items = (MappingItemCollection)serial.Deserialize(reader);
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
			string strType = Items.GetType().FullName;
			writer.WriteAttributeString("type", strType);
			XmlSerializer serial = new XmlSerializer(Type.GetType(strType));
			serial.Serialize(writer, Items);
			writer.WriteEndElement();
		}
		
		public XmlSchema GetSchema()
		{
			return(null);
		}*/
	}
}

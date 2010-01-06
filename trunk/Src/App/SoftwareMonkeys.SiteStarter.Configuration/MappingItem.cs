using System;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Configuration;


namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of MappingItem.
	/// </summary>
	//[XmlType("SiteStarter.MappingItem")]
	public class MappingItem : IMappingItem//, IXmlSerializable
	{
		private string typeName;
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		/// <summary>
        /// Gets/sets the flexible settings collection.
        /// </summary>
        [XmlIgnore]
        IConfigurationDictionary IMappingItem.Settings
        {
            get {
            	return this.Settings; }
        	set { this.Settings = new ConfigurationDictionary(value); }
        }
        
        private ConfigurationDictionary settings = new ConfigurationDictionary();
        /// <summary>
        /// Gets/sets the flexible settings collection.
        /// </summary>
        public ConfigurationDictionary Settings
        {
            get {
            	if (settings == null)
            		settings = new ConfigurationDictionary();
            	return settings; }
            set { settings = value; }
        }
        
		/*private IConfigurationDictionary settings = new ConfigurationDictionary();
		/// <summary>
		/// The mapping settings applied to this item.
		/// </summary>
		public IConfigurationDictionary Settings
		{
			get { return settings; }
			set { settings = value; }
		}*/
		
		public MappingItem()
		{
		
		}
		
		public MappingItem(Type entityType)
		{
			typeName = entityType.ToString();
			
		}
		
		public MappingItem(string entityType)
		{
			typeName = entityType;
			
		}
		
		/*void IXmlSerializable.ReadXml ( XmlReader reader )
		{
			// Name
			TypeName = reader.ReadElementString("TypeName");
			
			
			// Items
			reader.ReadStartElement("Settings");
			string strType = reader.GetAttribute("type");
			XmlSerializer serial = new XmlSerializer(Type.GetType(strType));
			Settings = (ConfigurationDictionary)serial.Deserialize(reader);
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml ( XmlWriter writer )
		{
			// Name
			writer.WriteElementString("TypeName", TypeName);
			
			// Items
			writer.WriteStartElement("Settings");
			string strType = Settings.GetType().FullName;
			writer.WriteAttributeString("type", strType);
			XmlSerializer serial = new XmlSerializer(Type.GetType(strType));
			serial.Serialize(writer, Settings);
			writer.WriteEndElement();
		}
		
		public XmlSchema GetSchema()
		{
			return(null);
		}
		*/
	}
}

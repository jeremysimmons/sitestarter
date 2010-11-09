using System;
using System.Collections;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// A collection that can be serialized.
	/// </summary>
	[Serializable]
	[XmlRoot("Xml")]
	[XmlType("Xml")]
	public class SerializableCollection
	{
		private ArrayList entities = new ArrayList();
		[XmlArrayItem("Entity")]
		public ArrayList Entities
		{
			get { return entities; }
			set { entities = value; }
		}
		
		public SerializableCollection(IEntity[] entities)
		{
			foreach (IEntity entity in entities)
			{
				Entities.Add(entity);
			}
		}
		
		public SerializableCollection()
		{}
	}
}

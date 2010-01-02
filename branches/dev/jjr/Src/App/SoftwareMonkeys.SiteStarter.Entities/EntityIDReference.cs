using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of IDReference.
	/// </summary>
	[Serializable]
	public class EntityIDReference : BaseEntity//, IXmlSerializable
	{
		/*private Guid[] entityIDs = new Guid[2];
		/// <summary>
		/// Gets/sets the IDs of the entities that the reference is associated with.
		/// </summary>
		public Guid[] EntityIDs
		{
			get { return entityIDs; }
			set { entityIDs = value; }
		}*/
		
		private Guid entity1ID;
		public Guid Entity1ID
		{
			get { return entity1ID; }
			set { entity1ID = value; }
		}
		
		private Guid entity2ID;
		public Guid Entity2ID
		{
			get { return entity2ID; }
			set { entity2ID = value; }
		}
		
		private string property1Name = String.Empty;
		public string Property1Name
		{
			get { return property1Name; }
			set { property1Name = value; }
		}
		
		private string property2Name = String.Empty;
		public string Property2Name
		{
			get { return property2Name; }
			set { property2Name = value; }
		}
		
		/*private string[] typeNames = new String[2];
		/// <summary>
		/// Gets/sets the short names of the types involved in the reference.
		/// </summary>
		public string[] TypeNames
		{
			get { return typeNames; }
			set { typeNames = value; }
		}*/
		
		private string typeName1;
		public string TypeName1
		{
			get { return typeName1; }
			set { typeName1 = value; }
		}
		
		private string typeName2;
		public string TypeName2
		{
			get { return typeName2; }
			set { typeName2 = value; }
		}
		
		public EntityIDReference()
		{
		}
		
		/*/// <summary>
		/// Adds the provided entity ID to the reference.
		/// </summary>
		/// <param name="id">The entity ID to add.</param>
		/// <param name="typeName">The short type name.</param>
		public void Add(Guid id, string typeName)
		{
			if (id == null)
				throw new ArgumentNullException("id");
			
			List<Guid> ids = (EntityIDs == null ? new List<Guid>() : new List<Guid>(EntityIDs));
			List<string> names = (TypeNames == null ? new List<string>() : new List<string>(TypeNames));
			if (!ids.Contains(id) && !names.Contains(typeName))
			{
				ids.Add(id);
				names.Add(typeName);
			}
			EntityIDs = ids.ToArray();
			TypeNames = names.ToArray();
		}
		
		public void Add(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!Includes(entity))
				Add(entity.ID, entity.GetType().Name);
		}*/
			
			/*public virtual bool Includes(IEntity entity)
		{
			bool flag = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provided entity is included in the reference.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				AppLogger.Debug("Provided entity name: " + entity.ShortTypeName);
				AppLogger.Debug("Provided entity ID: " + entity.ID.ToString());
				AppLogger.Debug("Reference entity 1 ID: " + Entity1ID.ToString());
				AppLogger.Debug("Reference entity 2 ID: " + Entity2ID.ToString());
				AppLogger.Debug("Reference entity type name 1: " + TypeName1);
				AppLogger.Debug("Reference entity type name 2: " + TypeName2);
				
				
				flag = (entity.ID.Equals(Entity1ID) && entity.ShortTypeName.Equals(TypeName1))
					|| (entity.ID.Equals(Entity2ID) && entity.ShortTypeName.Equals(TypeName2));
				
				AppLogger.Debug("Entity is included in reference: " + flag.ToString());
			}
			
			return flag;
		}*/
		
		/// <summary>
		/// Checks whether the reference includes an entity with the specified ID and a property with the specified name.
		/// Note: The ID and property belong to the same entity. The property does not contain the provided ID.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public virtual bool Includes(Guid id, string propertyName)
		{
			bool flag = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provided entity is included in the reference.", NLog.LogLevel.Debug))
			{
				if (id == Guid.Empty)
					throw new ArgumentException("entity");
				
				AppLogger.Debug("Provided entity ID: " + id.ToString());
				AppLogger.Debug("Reference entity 1 ID: " + Entity1ID.ToString());
				AppLogger.Debug("Reference entity 2 ID: " + Entity2ID.ToString());
				AppLogger.Debug("Reference property 1: " + Property1Name.ToString());
				AppLogger.Debug("Reference property 2: " + Property2Name.ToString());
				AppLogger.Debug("Reference entity type name 1: " + TypeName1);
				AppLogger.Debug("Reference entity type name 2: " + TypeName2);
				
				
				flag = (id.Equals(Entity1ID) && propertyName.Equals(Property1Name))
					|| (id.Equals(Entity2ID) && propertyName.Equals(Property2Name));
				
				AppLogger.Debug("Entity is included in reference: " + flag.ToString());
			}
			
			return flag;
		}
		
		/*/// <summary>
		/// Removes the specified entity from the reference.
		/// </summary>
		/// <param name="entity">The entity to remove.</param>
		public void Remove(IEntity entity)
		{
			Remove(entity.ID);
		}
		
		/// <summary>
		/// Removes the specified entity from the reference.
		/// </summary>
		/// <param name="id">The entity ID to remove.</param>
		/// <param name="typeName">The short type name.</param>
		public void Remove(Guid id)
		{
			List<Guid> ids = (EntityIDs == null ? new List<Guid>() : new List<Guid>(EntityIDs));
			List<string> names = (TypeNames == null ? new List<string>() : new List<string>(TypeNames));
			if (ids.Contains(id))
			{
				int pos = ids.IndexOf(id);
				
				ids.Remove(id);
				names.Remove(names[pos]);
			}
			EntityIDs = ids.ToArray();
			TypeNames = names.ToArray();
		}*/
			
			/*	public void ReadXml ( XmlReader reader )
		{
			reader.ReadStartElement("EntityIDs");
			//string strType = reader.GetAttribute("type");
			XmlSerializer serial = new XmlSerializer(typeof(Guid[]));
			EntityIDs = (Guid[])serial.Deserialize(reader);
			reader.ReadEndElement();
		}

		public void WriteXml ( XmlWriter writer )
		{
			writer.WriteStartElement("EntityIDs");
			//writer.WriteAttributeString("type", strType);
			XmlSerializer serial = new XmlSerializer(typeof(Guid[]));
			serial.Serialize(writer, EntityIDs);
			writer.WriteEndElement();
		}
		
		public XmlSchema GetSchema()
		{
			return(null);
		}*/
			
			public virtual EntityIDReference ToData()
		{
			return this;
		}
		
		public EntityIDReference SwitchFor(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Switching reference data to the perspective of a specific entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				AppLogger.Debug("Existing target Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Existing source entity type: " + TypeName1);
				AppLogger.Debug("Existing reference entity type: " + TypeName2);
				AppLogger.Debug("Existing source entity ID: " + Entity1ID.ToString());
				AppLogger.Debug("Existing reference entity ID: " + Entity2ID.ToString());
				AppLogger.Debug("Existing source property name: " + Property1Name.ToString());
				AppLogger.Debug("Existing reference property name: " + Property2Name.ToString());
				
				if (EntitiesUtilities.MatchAlias(entity.GetType().Name, TypeName1))
				{
					AppLogger.Debug("The reference is already suited for the specified entity. No need to switch.");
					
					return this;
				}
				else
				{
					AppLogger.Debug("Switching to the perspective of entity type: " + entity.GetType());
					
					Guid entity1ID = Entity1ID;
					Guid entity2ID = Entity2ID;
					
					string typeName1 = TypeName1;
					string typeName2 = TypeName2;
					
					string property1Name = Property1Name;
					string property2Name = Property2Name;
					
					this.Entity1ID = entity2ID;
					this.Entity2ID = entity1ID;
					
					this.TypeName1 = typeName2;
					this.TypeName2 = typeName1;
					
					this.Property1Name = property2Name;
					this.Property2Name = property1Name;
					
					return this;
				}
			}
		}
	}
}

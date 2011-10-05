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
	public class EntityIDReference : BaseEntity, IEntity//, IXmlSerializable
	{
		private Guid entity1ID = Guid.Empty;
		public Guid Entity1ID
		{
			get { return entity1ID; }
			set { entity1ID = value; }
		}
		
		private Guid entity2ID = Guid.Empty;
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
		
		private string type1Name = String.Empty;
		public string Type1Name
		{
			get { return type1Name; }
			set { type1Name = value; }
		}
		
		private string type2Name = String.Empty;
		public string Type2Name
		{
			get { return type2Name; }
			set { type2Name = value; }
		}
		
		public EntityIDReference()
		{
		}
		
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
			
			// Logging disabled to improve performance
			//using (LogGroup logGroup = LogGroup.Start("Checking whether the provided entity is included in the reference.", NLog.LogLevel.Debug))
			//{
			if (id == Guid.Empty)
				throw new ArgumentException("The provided ID is Guid.Empty","id");
			
			//LogWriter.Debug("Provided entity ID: " + id.ToString());
			//LogWriter.Debug("Provided property name: " + propertyName);
			//LogWriter.Debug("Reference entity 1 ID: " + Entity1ID.ToString());
			//LogWriter.Debug("Reference entity 2 ID: " + Entity2ID.ToString());
			//LogWriter.Debug("Reference property 1: " + Property1Name.ToString());
			//LogWriter.Debug("Reference property 2: " + Property2Name.ToString());
			//LogWriter.Debug("Reference entity type name 1: " + Type1Name);
			//LogWriter.Debug("Reference entity type name 2: " + Type2Name);
			
			
			flag = (id.Equals(Entity1ID) && propertyName.Equals(Property1Name))
				|| (id.Equals(Entity2ID) && propertyName.Equals(Property2Name));
			
			//LogWriter.Debug("Entity is included in reference: " + flag.ToString());
			//}
			
			return flag;
		}
		
		// TODO: Check if needed. Remove if not n
		/*public virtual EntityIDReference ToData()
		{
			return (EntityIDReference)this;
		}*/
				
		public EntityIDReference SwitchFor(IEntity entity)
		{
			return SwitchFor(entity.GetType().Name, entity.ID);
		}
		
		public EntityIDReference SwitchFor(Type type, Guid id)
		{
			return SwitchFor(type.Name, id);
		}
		
		public EntityIDReference SwitchFor(string typeName, Guid id)
		{
			
			//using (LogGroup logGroup = LogGroup.Start("Switching reference data to the perspective of a specific entity.", NLog.LogLevel.Debug))
			//{
			if (typeName == null)
				throw new ArgumentNullException("typeName");
			
			//	LogWriter.Debug("Existing target Entity type: " + typeName);
			//	LogWriter.Debug("Existing source entity type: " + Type1Name);
			//	LogWriter.Debug("Existing reference entity type: " + Type2Name);
			//	LogWriter.Debug("Existing source entity ID: " + Entity1ID.ToString());
			//	LogWriter.Debug("Existing reference entity ID: " + Entity2ID.ToString());
			//	LogWriter.Debug("Existing source property name: " + Property1Name.ToString());
			//	LogWriter.Debug("Existing reference property name: " + Property2Name.ToString());
			
			if (EntitiesUtilities.MatchAlias(typeName, Type1Name))
			{
				//		LogWriter.Debug("The reference is already suited for the specified entity. No need to switch.");
				
			}
			else
			{
				//		LogWriter.Debug("Switching to the perspective of entity type: " + typeName);
				
				Guid entity1ID = Entity1ID;
				Guid entity2ID = Entity2ID;
				
				string type1Name = Type1Name;
				string type2Name = Type2Name;
				
				string property1Name = Property1Name;
				string property2Name = Property2Name;
				
				this.Entity1ID = entity2ID;
				this.Entity2ID = entity1ID;
				
				this.Type1Name = type2Name;
				this.Type2Name = type1Name;
				
				this.Property1Name = property2Name;
				this.Property2Name = property1Name;
			}
			//}
			
			return this;
		}
		
	}
}

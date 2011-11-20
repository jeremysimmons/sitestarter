using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityReference.
	/// </summary>
	[Serializable]
	[XmlRootAttribute("EntityReference")]
	[XmlTypeAttribute("EntityReference")]
	public class EntityReference : BaseEntity, IEntity
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
		
		private IEntity sourceEntity;
		/// <summary>
		/// Gets/sets the source entity.
		/// </summary>
		[XmlIgnore]
		public IEntity SourceEntity
		{
			get
			{
				// If the source entity doesn't match Type1Name property then return null
				if (sourceEntity != null && Type1Name != sourceEntity.ShortTypeName)
					return null;
				
				return sourceEntity;
			}
			set
			{
				//using (LogGroup logGroup = LogGroup.Start("Setting EntityReference.SourceEntity value", NLog.LogLevel.Debug))
				//{
				sourceEntity = value;
				if (sourceEntity != null)
				{
					//LogWriter.Debug("sourceEntity != null");
					
					//LogWriter.Debug("Short type name: " + sourceEntity.ShortTypeName);
					//LogWriter.Debug("Entity ID: " + sourceEntity.ID);
					
					Type1Name = sourceEntity.ShortTypeName;
					Entity1ID = sourceEntity.ID;
				}
				else
				{
					//LogWriter.Debug("sourceEntity == null");
					
					Entity1ID = Guid.Empty;
				}
				//}
			}
		}
		
		private IEntity referenceEntity;
		/// <summary>
		/// Gets/sets the entity being referenced.
		/// </summary>
		[XmlIgnore]
		public IEntity ReferenceEntity
		{
			get
			{
				// If the reference entity doesn't match Type2Name property then return null
				if (referenceEntity != null && Type2Name != referenceEntity.ShortTypeName)
					return null;
				
				return referenceEntity;
			}
			set
			{
				//using (LogGroup logGroup = LogGroup.Start("Setting EntityReference.ReferenceEntity value", NLog.LogLevel.Debug))
				//{
				referenceEntity = value;
				if (referenceEntity != null)
				{
					//LogWriter.Debug("referenceEntity != null");
					
					//LogWriter.Debug("Short type name: " + referenceEntity.ShortTypeName);
					//LogWriter.Debug("Type ID: " + referenceEntity.ID);
					
					Type2Name = referenceEntity.ShortTypeName;
					Entity2ID = referenceEntity.ID;
				}
				else
				{
					//LogWriter.Debug("referenceEntity == null");
					
					//LogWriter.Debug("ID: Guid.Empty");

					Entity2ID = Guid.Empty;
				}
				//}
			}
		}
		
		public EntityReference()
		{
		}
		
		/// <summary>
		/// Gets the entity in the reference that wasn't provided. Hence the "other" entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public IEntity GetOtherEntity(IEntity entity)
		{
			IEntity otherEntity = null;
			
			//using (LogGroup logGroup = LogGroup.Start("Retrieving the entity from the reference that is not the one provided (ie. the other entity).", NLog.LogLevel.Debug))
			//{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (referenceEntity == null)
				throw new InvalidOperationException("The referenced entity is null.");
			
			if (sourceEntity == null)
				throw new InvalidOperationException("The source entity is null.");
			
			if (referenceEntity.ID == entity.ID)
				otherEntity = sourceEntity;
			else if (sourceEntity.ID == entity.ID)
				otherEntity = referenceEntity;
			else
				throw new InvalidOperationException("Can't get the other entity. Neither entity matches.\nParameter entity type: " + entity.ToString() + "\nParameter entity ID: " + entity.ID.ToString() + "\nEntity #1 type: " + Type1Name + "\nEntity #2 type: " + Type2Name + "\nProperty #1 type: " + Property1Name + "\nProperty #2 type: " + Property2Name + "\nEntity #1 ID: " + Entity1ID.ToString() + "\nEntity #2 ID: " + Entity2ID.ToString());
			
			//LogWriter.Debug("Other entity type: " + otherEntity.GetType().ToString());
			//LogWriter.Debug("Other entity ID: " + otherEntity.ID.ToString());
			//}
			
			return otherEntity;
		}
		
		public override void Deactivate()
		{
			base.Deactivate();
			
			referenceEntity = null;
			sourceEntity = null;
		}
		
		public EntityReference SwitchFor(IEntity entity)
		{
			EntityReference reference = this;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Switching the reference to the perspective of '" + entity.ShortTypeName + "' entity."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Existing source entity type: " + Type1Name);
				LogWriter.Debug("Existing reference entity type: " + Type2Name);
				LogWriter.Debug("Existing source entity ID: " + Entity1ID.ToString());
				LogWriter.Debug("Existing reference entity ID: " + Entity2ID.ToString());
				LogWriter.Debug("Existing source property name: " + Property1Name.ToString());
				LogWriter.Debug("Existing reference property name: " + Property2Name.ToString());
				
				SwitchFor(entity.ShortTypeName, entity.ID);
			}
			
			return reference;
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
		
		public EntityReference SwitchFor(Type type, Guid id)
		{
			return SwitchFor(type.Name, id);
		}
		
		public virtual EntityReference SwitchFor(string typeName, Guid id)
		{
			//EntityReference reference = (EntityReference)Clone();
			EntityReference reference = this;
			
			// TODO: Comment out logging to boost performance
			using (LogGroup logGroup = LogGroup.StartDebug("Switching reference data to the perspective of a '" + typeName + "' entity."))
			{
				if (typeName == null)
					throw new ArgumentNullException("typeName");
				
				LogWriter.Debug("Existing source entity type: " + Type1Name);
				LogWriter.Debug("Existing reference entity type: " + Type2Name);
				LogWriter.Debug("Existing source entity ID: " + Entity1ID.ToString());
				LogWriter.Debug("Existing reference entity ID: " + Entity2ID.ToString());
				LogWriter.Debug("Existing source property name: " + Property1Name.ToString());
				LogWriter.Debug("Existing reference property name: " + Property2Name.ToString());
				
				if (EntitiesUtilities.MatchAlias(typeName, Type1Name)
				    && Entity1ID == id)
				{
					LogWriter.Debug("The reference is already in the perspective of the specified entity. No need to switch.");
					
				}
				else
				{
					LogWriter.Debug("Switching to the perspective of entity type: " + typeName);
					
					Guid entity1ID = Entity1ID;
					Guid entity2ID = Entity2ID;
					
					string type1Name = Type1Name;
					string type2Name = Type2Name;
					
					string property1Name = Property1Name;
					string property2Name = Property2Name;
					
					IEntity originalSourceEntity = SourceEntity;
					IEntity originalReferenceEntity = ReferenceEntity;
					
					reference.SourceEntity = originalReferenceEntity;
					reference.ReferenceEntity = originalSourceEntity;
					
					reference.Entity1ID = entity2ID;
					reference.Entity2ID = entity1ID;
					
					reference.Type1Name = type2Name;
					reference.Type2Name = type1Name;
					
					reference.Property1Name = property2Name;
					reference.Property2Name = property1Name;
					
					
					LogWriter.Debug("New source entity type: " + reference.Type1Name);
					LogWriter.Debug("New reference entity type: " + reference.Type2Name);
					LogWriter.Debug("New source entity ID: " + reference.Entity1ID.ToString());
					LogWriter.Debug("New reference entity ID: " + reference.Entity2ID.ToString());
					LogWriter.Debug("New source property name: " + reference.Property1Name.ToString());
					LogWriter.Debug("New reference property name: " + reference.Property2Name.ToString());
				}
			}
			
			return reference;
		}
		
		public override IEntity Clone()
		{
			EntityReference reference = new EntityReference();
			
			CopyTo(reference);
			
			return reference;
		}
		
		public override void CopyTo(IEntity entity)
		{
			EntityReference reference = (EntityReference)entity;
			
			reference.ID = ID;
			reference.SourceEntity = SourceEntity;
			reference.ReferenceEntity = ReferenceEntity;
			reference.Entity1ID = Entity1ID;
			reference.Entity2ID = Entity2ID;
			reference.Property1Name = Property1Name;
			reference.Property2Name = Property2Name;
			reference.Type1Name = Type1Name;
			reference.Type2Name = Type2Name;
		}
		
	}
}
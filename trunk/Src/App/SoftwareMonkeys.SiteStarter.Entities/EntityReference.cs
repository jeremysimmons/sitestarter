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
	public class EntityReference : EntityIDReference, IEntity
	{
		private IEntity sourceEntity;
		/// <summary>
		/// Gets/sets the source entity
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
					
					base.Type1Name = sourceEntity.ShortTypeName;
					base.Entity1ID = sourceEntity.ID;
				}
				else
				{
					//LogWriter.Debug("sourceEntity == null");
					
					//LogWriter.Debug("ID: Guid.Empty");
					//if (base.TypeNames.Length > 0)
					//	LogWriter.Debug("Short type name remains: " + base.TypeNames[0]);
					
					//base.TypeNames[0] = String.Empty;
					base.Entity1ID = Guid.Empty;
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
					
					base.Type2Name = referenceEntity.ShortTypeName;
					base.Entity2ID = referenceEntity.ID;
				}
				else
				{
					//LogWriter.Debug("referenceEntity == null");
					
					//LogWriter.Debug("ID: Guid.Empty");

					base.Entity2ID = Guid.Empty;
				}
				//}
			}
		}
		
		public EntityReference()
		{
		}
		
		public EntityReference(EntityIDReference reference)
		{
			Property1Name = reference.Property1Name;
			Property2Name = reference.Property2Name;
			
			Type1Name = reference.Type1Name;
			Type2Name = reference.Type2Name;
			
			Entity1ID = reference.Entity1ID;
			Entity2ID = reference.Entity2ID;
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
		
		public new EntityReference SwitchFor(IEntity entity)
		{
			//EntityReference reference = (EntityReference)this.Clone();
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
				
				// Switch the SourceEntity and ReferencedEntity properties if needed
				if (reference.SourceEntity == null || reference.SourceEntity.ID != entity.ID)
				{
					LogWriter.Debug("Switching.");
					
					IEntity originalSourceEntity = reference.SourceEntity;
					IEntity originalReferenceEntity = reference.ReferenceEntity;
					
					string originalProperty1 = reference.Property1Name;
					string originalProperty2 = reference.Property2Name;
					
					reference.SourceEntity = originalReferenceEntity;
					reference.ReferenceEntity = originalSourceEntity;
					
					reference.Property1Name = originalProperty2;
					reference.Property2Name = originalProperty1;
					
					LogWriter.Debug("New source entity type: " + reference.Type1Name);
					LogWriter.Debug("New reference entity type: " + reference.Type2Name);
					LogWriter.Debug("New source entity ID: " + reference.Entity1ID.ToString());
					LogWriter.Debug("New reference entity ID: " + reference.Entity2ID.ToString());
					LogWriter.Debug("New source property name: " + reference.Property1Name.ToString());
					LogWriter.Debug("New reference property name: " + reference.Property2Name.ToString());
				}
				else
					LogWriter.Debug("No need to switch. Skipping.");
			}
			
			return reference;
		}
	}
}
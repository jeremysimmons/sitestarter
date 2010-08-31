using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityReference.
	/// </summary>
	[Serializable]
	[XmlRootAttribute("EntityReference")]
	[XmlTypeAttribute("EntityReference")]
	public class EntityReference : EntityIDReference//, IXmlSerializable
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
				return sourceEntity;
			}
			set
			{
				//using (LogGroup logGroup = AppLogger.StartGroup("Setting EntityReference.SourceEntity value", NLog.LogLevel.Debug))
				//{
					sourceEntity = value;
					if (sourceEntity != null)
					{
						//AppLogger.Debug("sourceEntity != null");
						
						//AppLogger.Debug("Short type name: " + sourceEntity.ShortTypeName);
						//AppLogger.Debug("Entity ID: " + sourceEntity.ID);
						
						base.Type1Name = sourceEntity.ShortTypeName;
						base.Entity1ID = sourceEntity.ID;
					}
					else
					{
						//AppLogger.Debug("sourceEntity == null");
						
						//AppLogger.Debug("ID: Guid.Empty");
						//if (base.TypeNames.Length > 0)
						//	AppLogger.Debug("Short type name remains: " + base.TypeNames[0]);
						
						//base.TypeNames[0] = String.Empty;
						base.Entity1ID = Guid.Empty;
					}
				//}
			}
		}
		
//		private Guid referenceEntityID;
//		/// <summary>
//		/// Gets/sets the ID of the entity being referenced.
//		/// </summary>
//		public Guid ReferenceEntityID
//		{
//			get { return referenceEntityID; }
		/*set {
//				// Remove the old ID
				base.Remove(referenceEntityID);
				referenceEntityID = value;
				// Add the new ID
				base.Add(value);
			}*/
		/*get {
				if (ReferenceEntity != null)
					return ReferenceEntity.ID;
				else
					return referenceEntityID; }
			set { referenceEntityID = value;
				if (ReferenceEntity != null && ReferenceEntity.ID != value)
					ReferenceEntity = default(E2);
			}*/
//		}
		
		private IEntity referenceEntity;
		/// <summary>
		/// Gets/sets the entity being referenced.
		/// </summary>
		[XmlIgnore]
		public IEntity ReferenceEntity
		{
			get
			{
				return referenceEntity;
			}
			set
			{
				//using (LogGroup logGroup = AppLogger.StartGroup("Setting EntityReference.ReferenceEntity value", NLog.LogLevel.Debug))
				//{
					referenceEntity = value;
					if (referenceEntity != null)
					{
						//AppLogger.Debug("referenceEntity != null");
						
						//AppLogger.Debug("Short type name: " + referenceEntity.ShortTypeName);
						//AppLogger.Debug("Type ID: " + referenceEntity.ID);
						
						base.Type2Name = referenceEntity.ShortTypeName;
						base.Entity2ID = referenceEntity.ID;
					}
					else
					{
						//AppLogger.Debug("referenceEntity == null");
						
						//AppLogger.Debug("ID: Guid.Empty");
						//if (base.TypeNames.Length > 0)
						//		AppLogger.Debug("Short type name remains: " + base.TypeNames[1]);
						
						//base.TypeNames[1] = String.Empty;
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
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entity from the reference that is not the one provided (ie. the other entity).", NLog.LogLevel.Debug))
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
					throw new InvalidOperationException("Can't get the other entity. Both entities match.");
				
				//AppLogger.Debug("Other entity type: " + otherEntity.GetType().ToString());
				//AppLogger.Debug("Other entity ID: " + otherEntity.ID.ToString());
			//}
			
			return otherEntity;
		}
		
		public void Deactivate()
		{
			referenceEntity = null;
			sourceEntity = null;
		}
	}
}
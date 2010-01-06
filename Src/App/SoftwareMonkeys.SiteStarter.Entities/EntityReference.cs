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
				using (LogGroup logGroup = AppLogger.StartGroup("Setting EntityReference.SourceEntity value", NLog.LogLevel.Debug))
				{
					sourceEntity = value;
					if (sourceEntity != null)
					{
						AppLogger.Debug("sourceEntity != null");
						
						AppLogger.Debug("Short type name: " + sourceEntity.ShortTypeName);
						AppLogger.Debug("Entity ID: " + sourceEntity.ID);
						
						base.TypeName1 = sourceEntity.ShortTypeName;
						base.Entity1ID = sourceEntity.ID;
					}
					else
					{
						AppLogger.Debug("sourceEntity == null");
						
						AppLogger.Debug("ID: Guid.Empty");
						//if (base.TypeNames.Length > 0)
						//	AppLogger.Debug("Short type name remains: " + base.TypeNames[0]);
						
						//base.TypeNames[0] = String.Empty;
						base.Entity1ID = Guid.Empty;
					}
				}
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
				using (LogGroup logGroup = AppLogger.StartGroup("Setting EntityReference.ReferenceEntity value", NLog.LogLevel.Debug))
				{
					referenceEntity = value;
					if (referenceEntity != null)
					{
						AppLogger.Debug("referenceEntity != null");
						
						AppLogger.Debug("Short type name: " + referenceEntity.ShortTypeName);
						AppLogger.Debug("Type ID: " + referenceEntity.ID);
						
						base.TypeName2 = referenceEntity.ShortTypeName;
						base.Entity2ID = referenceEntity.ID;
					}
					else
					{
						AppLogger.Debug("referenceEntity == null");
						
						AppLogger.Debug("ID: Guid.Empty");
						//if (base.TypeNames.Length > 0)
						//		AppLogger.Debug("Short type name remains: " + base.TypeNames[1]);
						
						//base.TypeNames[1] = String.Empty;
						base.Entity2ID = Guid.Empty;
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the entity in the reference that wasn't provided. Hence the "other" entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public IEntity GetOtherEntity(IEntity entity)
		{
			IEntity otherEntity = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entity from the reference that is not the one provided (ie. the other entity).", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (referenceEntity == null)
					throw new InvalidOperationException("The referenced entity is null.");
				
				if (sourceEntity == null)
					throw new InvalidOperationException("The source entity is null.");
				
				if (referenceEntity.ID != entity.ID)
					otherEntity = referenceEntity;
				else if (sourceEntity.ID != entity.ID)
					otherEntity = sourceEntity;
				else
					throw new InvalidOperationException("Can't get the other entity. Both entities match.");
				
				AppLogger.Debug("Other entity type: " + otherEntity.GetType().ToString());
				AppLogger.Debug("Other entity ID: " + otherEntity.ID.ToString());
			}
			
			return otherEntity;
		}
		
		public void Deactivate()
		{
			referenceEntity = null;
			sourceEntity = null;
		}
	}
}

/*/// <summary>
/// Description of EntityReference.
/// </summary>
[Serializable]
[XmlRootAttribute("EntityReference")]
[XmlTypeAttribute("EntityReference")]
public class EntityReference<E1, E2> : EntityReference, IXmlSerializable
	where E1 : IEntity
	where E2 : IEntity
{
	private Guid id = Guid.NewGuid();
	public new Guid ID
	{
		get { return id; }
		set { id = value; }
	}
	
	///		/// <summary>
//		/// Gets/sets the source entity
//		/// </summary>
//		[XmlIgnore]
//		public new E1 SourceEntity
//		{
//			get
//			{
//				return (E1)base.SourceEntity;
//			}
//			set
//			{
//				//if (base.SourceEntity != null)
//				//	base.Remove(base.SourceEntity);
//				base.SourceEntity = value;
//
//				//if (base.SourceEntity != null)
//				//	base.Add(base.SourceEntity);
//			}
//		}
	
//		/// <summary>
//		/// Gets/sets the entity being referenced.
//		/// </summary>
//		[XmlIgnore]
//		public new E2 ReferenceEntity
//		{
//			get
//			{
//				return (E2)base.ReferenceEntity;
//			}
//			set
//			{
//				//if (base.ReferenceEntity != null)
//				//	base.Remove(base.ReferenceEntity);
//				base.ReferenceEntity = value;
	
//				//if (base.ReferenceEntity != null)
//				//	base.Add(base.ReferenceEntity);
//			}
//		}
	
	/// <summary>
	/// Empty constructor.
	/// </summary>
	public EntityReference()
	{
		TypeNames[0] = typeof(E1).Name;
		TypeNames[1] = typeof(E2).Name;
	}
	
//		/// <summary>
//		/// Sets the source entity of the reference.
//		/// </summary>
//		/// <param name="reader">The source entity.</param>
//		public EntityReference(E1 source)
//		{
//			if (source == null)
//				throw new ArgumentNullException("source");
//
//			this.sourceEntity = source;
//			Add(source);
//		}
	
	/// <summary>
	/// Sets the source and reference entities.
	/// </summary>
	/// <param name="reader">The source entity.</param>
	/// <param name="reference">The reference entity.</param>
	public EntityReference(E1 source, E2 reference)
	{
		//if (source == null)
		//	throw new ArgumentNullException("source");
		
		using (LogGroup logGroup = AppLogger.StartGroup("Constructing entity reference.", NLog.LogLevel.Debug))
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			
			AppLogger.Debug("Source entity type: " + source != null ? source.GetType().ToString() : "[null]");
			AppLogger.Debug("Reference entity type: " + reference != null ? reference.GetType().ToString() : "[null]");
			AppLogger.Debug("Source entity ID: " + source != null ? source.ID.ToString() : "[null]");
			AppLogger.Debug("Reference entity ID: " + reference != null ? reference.ID.ToString() : "[null]");
			
			
			SourceEntity = source;
			//Add(source);
			ReferenceEntity = reference;
			//Add(reference);
			
		}
	}
	
//		public E GetEntity<E>()
//			where E : IEntity
//		{
//			foreach (IEntity entity in Entities)
//			{
//				if (entity is E)
//					return (E)entity;
//			}
//			return default(E);
//		}
//
//		public void SetEntity<E>(E entity)
//			where E : IEntity
//		{
//			bool found = false;
//			for (int i = 0; i < Entities.Length; i++)
//			{
//				if (entity is E)
//				{
//					Entities[i] = entity;
//					found = true;
//				}
//			}
//			if (!found)
//			{
//				Add(entity);
//			}
//		}
	
//		public bool Includes(IEntity entity)
//		{
//			if (entity == null)
//				throw new ArgumentNullException("entity");
//
//			return (SourceEntity != null && SourceEntity.ID == entity.ID)
//				|| (ReferenceEntity != null && ReferenceEntity.ID == entity.ID);
//		}
	
	public new void ReadXml ( XmlReader reader )
	{
		
		base.ReadXml(reader);
	}

	public new void WriteXml ( XmlWriter writer )
	{
		base.WriteXml(writer);
	}
	
	public XmlSchema GetSchema()
	{
		return(null);
	}
	
	public override EntityIDReference ToData()
	{
		return this;
		
//			/*EntityReference reference = new EntityReference();
//
//			using (LogGroup logGroup = AppLogger.StartGroup("Stripping this reference down to bare ID reference.", NLog.LogLevel.Debug))
//			{
//
//
//				reference.ID = this.ID;
//				AppLogger.Debug("ID: " + reference.ID);
//				reference.TypeNames = this.TypeNames;
//				AppLogger.Debug("# of type names: " + reference.TypeNames.Length);
//				if (this.TypeNames.Length > 0)
//				{
//					AppLogger.Debug("Type name 0: " + this.TypeNames[0]);
//					AppLogger.Debug("Type name 1: " + this.TypeNames[1]);
//				}
//				reference.EntityIDs = this.EntityIDs;
//				AppLogger.Debug("# of entity IDs: " + reference.EntityIDs.Length);
//				if (this.TypeNames.Length > 0)
//				{
//					AppLogger.Debug("Entity ID 0: " + this.EntityIDs[0]);
//					AppLogger.Debug("Entity ID 1: " + this.EntityIDs[1]);
//				}
//			}
		
//			return reference;
	}
	
}
}*/
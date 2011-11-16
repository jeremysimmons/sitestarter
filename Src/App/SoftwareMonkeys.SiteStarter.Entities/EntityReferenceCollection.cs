using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityReferenceCollection.
	/// </summary>
	[Serializable]
	public class EntityReferenceCollection : List<EntityReference>
	{
		public new EntityReference this[int i]
		{
			get { return (EntityReference)base[i]; }
			set { base[i] = value; }
		}
		
		private IEntity sourceEntity;
		/// <summary>
		/// Gets/sets the source entity for this reference collection.
		/// </summary>
		public IEntity SourceEntity
		{
			get { return sourceEntity; }
			set { sourceEntity = value; }
		}
		
		
		public EntityReferenceCollection()
		{
			
		}
		
		public EntityReferenceCollection(EntityReference[] references) : base(Collection<EntityReference>.ConvertAll(references))
		{
		}
		
		public EntityReferenceCollection(IEntity sourceEntity)
		{
			SourceEntity = sourceEntity;
		}
		
		public EntityReferenceCollection(IEntity sourceEntity, string sourcePropertyName, IEntity[] referencedEntities, string referencedPropertyName)
		{
			if (sourceEntity != null && referencedEntities != null)
			{
				SourceEntity = sourceEntity;
				
				foreach (IEntity referencedEntity in referencedEntities)
				{
					if (referencedEntity != null)
					{
						EntityReference reference = new EntityReference();
						reference.ID = Guid.NewGuid();
						
						reference.Entity1ID = sourceEntity.ID;
						reference.Entity2ID = referencedEntity.ID;
						
						reference.Property1Name = sourcePropertyName;
						reference.Property2Name = referencedPropertyName;
						
						reference.Type1Name = sourceEntity.ShortTypeName;
						reference.Type2Name = referencedEntity.ShortTypeName;
						
						reference.SourceEntity = sourceEntity;
						reference.ReferenceEntity = referencedEntity;
						
						Add(reference);
					}
				}
			}
		}
		
		public IEntity[] GetReferencedEntities(IEntity sourceReference)
		{
			List<IEntity> list = new List<IEntity>();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving entities that are referenced by the one provided."))
			{
				LogWriter.Debug("Total references: " + Count.ToString());
				
				foreach (EntityReference reference in this)
				{
					list.Add(reference.GetOtherEntity(sourceReference));
				}
				
				LogWriter.Debug("Total matching: " + list.Count.ToString());
			}
			
			return list.ToArray();
		}
		
		public new EntityReference[] ToArray()
		{
			List<EntityReference> list = new List<EntityReference>();
			foreach (EntityReference r in this)
				list.Add(r);
			return list.ToArray();
		}
		
		/// <summary>
		/// Retrieves all the IDs of the entities from the collection that are associated with the specified entity.
		/// </summary>
		/// <param name="sourceEntityID">The entity that the referenced IDs are being retrieved for. If this is Guid.Empty then all IDs from both sides of the reference are returned.</param>
		/// <returns>The IDs of the referenced entities.</returns>
		public Guid[] GetEntityIDs(Guid sourceEntityID)
		{
			List<Guid> list = new List<Guid>();
			foreach (EntityReference reference in this)
			{
				if (sourceEntityID == Guid.Empty || reference.Entity1ID == sourceEntityID)
				{
					if (!list.Contains(reference.Entity2ID))
						list.Add(reference.Entity2ID);
				}
				
				if (sourceEntityID == Guid.Empty || reference.Entity2ID == sourceEntityID)
				{
					if (!list.Contains(reference.Entity1ID))
						list.Add(reference.Entity1ID);
				}
			}
			
			return list.ToArray();
		}
	}
}
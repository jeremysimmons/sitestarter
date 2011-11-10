using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityReferenceCollection.
	/// </summary>
	[Serializable]
	public class EntityReferenceCollection : EntityIDReferenceCollection
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
		
		public EntityReferenceCollection(EntityReference[] references) : base(Collection<EntityIDReference>.ConvertAll(references))
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
			
			foreach (EntityReference reference in this)
			{
				list.Add(reference.GetOtherEntity(sourceReference));
			}
			
			return list.ToArray();
		}
	}
}
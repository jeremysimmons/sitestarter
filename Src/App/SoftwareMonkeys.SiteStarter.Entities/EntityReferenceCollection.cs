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
		
		public EntityReferenceCollection(IEntity sourceEntity)
		{
			SourceEntity = sourceEntity;
		}
		
		// TODO: Remove
		/*public void Add(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Adding entity to the reference collection: " + entity.GetType().ToString()))
			{
				if (sourceEntity == null)
					throw new InvalidOperationException("The SourceEntity property needs to be set before entities can be added.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				
				
				EntityReference reference = Reflector.CreateGenericObject(typeof(EntityReference<,>),
				                                                          new Type[] {},
				                                                          new Object[] {});
				
				//	new EntityReference((E1)sourceEntity, (E2)entity);
				
				Add(reference);
			}
		}*/
		
		public EntityReferenceCollection(IEntity sourceEntity, string sourcePropertyName, IEntity[] referencedEntities, string referencedPropertyName)
		{
			if (sourceEntity != null && referencedEntities != null)
			{
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
						
						Add(reference);
					}
				}
			}
		}
	}
}

/*	/// <summary>
	/// Description of EntityReferenceCollection.
	/// </summary>
	[Serializable]
	public class EntityReferenceCollection<E1, E2> : EntityReferenceCollection
		where E1 : IEntity
		where E2 : IEntity
	{
		
		private Guid[] referenceIDs;
		/// <summary>
		/// Gets/sets the IDs of the references.
		/// </summary>
		public Guid[] ReferenceIDs
		{
			get { return GetReferenceIDs(); }
			//	set { SetReferenceIDs(value); }
		}
		
		/// <summary>
		/// Gets/sets the IDs of the referenced entities.
		/// </summary>
		public Guid[] IDs
		{
			get { return GetIDs(); }
			set { SetIDs(value); }
		}
		
		public EntityReferenceCollection()
		{
			
		}
		
		public EntityReferenceCollection(E1 sourceEntity)
		{
			SourceEntity = sourceEntity;
		}
		
//		public EntityReferenceCollection()
//		{
//
//		}
		
		protected new void Add(EntityReference<E1, E2> reference)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Adding a reference to the collection.", NLog.LogLevel.Debug))
			{
				if (reference == null)
					throw new ArgumentNullException("reference");
				
				//	if (reference.SourceEntity == null)
				//		reference.SourceEntity = SourceEntity;
				
				base.Add(reference);
			}
			
		}
		
//		public new void Add(EntityIDReference reference)
//		{
//			if (reference == null)
//				throw new ArgumentNullException("reference");
//
//			Add((EntityReference<E1, E2>)reference);
//		}
		
		public void Add(E2 entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Adding entity to the reference collection: " + entity.GetType().ToString()))
			{
				if (SourceEntity == null)
					throw new InvalidOperationException("The SourceEntity property needs to be set before entities can be added.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				EntityReference<E1, E2> reference = new EntityReference<E1, E2>((E1)SourceEntity, (E2)entity);
				
				Add(reference);
			}

			
		}
		
		public EntityReference<E1, E2> GetReference(params IEntity[] entities)
		{
			bool match = false;
			foreach (EntityReference<E1, E2> reference in this)
			{
				foreach (IEntity entity in entities)
				{
					match = reference.Includes(entity);
				}
				if (match)
					return reference;
			}
			return null;
		}
		
		
		public EntityReference<E1, E2> GetReference(params Guid[] ids)
		{
			bool match = false;
			foreach (EntityReference<E1, E2> reference in this)
			{
				foreach (Guid id in ids)
				{
					match = (reference.EntityIDs[0] == id
					         || reference.EntityIDs[1] == id);
				}
				if (match)
					return reference;
			}
			return null;
		}
		
		public Guid[] GetIDs()
		{
			List<Guid> list = new List<Guid>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving IDs of referenced entities.", NLog.LogLevel.Debug))
			{
				foreach (object reference in this)
				{
					AppLogger.Debug("ID: " + ((EntityReference)reference).GetOtherEntity(SourceEntity).ID);
					list.Add(((EntityReference)reference).GetOtherEntity(SourceEntity).ID);
				}
			}
			return list.ToArray();
		}
		
		
		public void SetIDs(Guid[] ids)
		{
			List<Guid> list = new List<Guid>();
			
			EntityIDReferenceCollection obsoleteReferences = new EntityIDReferenceCollection();
			
			
			
			using (LogGroup logGroup = AppLogger.StartGroup("Setting IDs of referenced entities.", NLog.LogLevel.Debug))
			{
				
				// Add all references to the obsolete list. They'll be removed one by one if they're still in use.
				foreach (EntityIDReference reference in this)
				{
					obsoleteReferences.Add(reference);
				}

				foreach (Guid id in ids)
				{
					AppLogger.Debug("ID: " + id.ToString());
					
					EntityReference reference = this.GetReference(id);
					
					if (reference != null)
					{
						AppLogger.Debug("Reference already exists.");
						
//						// Change the reference entity ID
//						reference.EntityIDs[1] = id;
//						if (reference.ReferenceEntity != null && !reference.ReferenceEntity.ID.Equals(id))
//							reference.ReferenceEntity = null;
							
							// Remove from the 'obsolete' list because it's still in use
							obsoleteReferences.Remove(reference);
					}
					else
					{
						AppLogger.Debug("Creating new reference.");
						
						// Add a new reference with the specified ID
						reference = new EntityReference<E1, E2>();
						reference.SourceEntity = SourceEntity;
						reference.EntityIDs[1] = id;
						
						Add(reference);
					}
				}
				
				//  Remove any obsolete references from the current list.
				foreach (EntityReference reference in obsoleteReferences)
				{
					AppLogger.Debug("Removing obsolete reference: " + reference.ID.ToString());
					
					this.Remove(reference);
				}
				
				AppLogger.Debug("# of obsolete references: " +
				                (obsoleteReferences == null
				                 ? "0"
				                 : obsoleteReferences.Count.ToString()));
				
				// Set the obsolete list
				this.RemovedReferences = (obsoleteReferences == null ? null : obsoleteReferences.ToArray());
			}
		}
		
		public Guid[] GetReferenceIDs()
		{
			List<Guid> list = new List<Guid>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving IDs of referenced entities.", NLog.LogLevel.Debug))
			{
				foreach (object reference in this)
				{
					AppLogger.Debug("ID: " + ((EntityReference)reference).ID);
					list.Add(((EntityReference)reference).ID);
				}
			}
			return list.ToArray();
		}
		
		public void Remove(E2 entity)
		{
			EntityReference<E1, E2> reference = GetReference(entity);
			Remove(reference);
		}
		
		// TODO: Remove
//		public static implicit operator EntityReferenceCollection<E1, E2>(EntityIDReferenceCollection c)
//		//	where E1 : IEntity
//		//	where E2 : IEntity
//		{
//			EntityReferenceCollection<E1, E2> newCollection = new EntityReferenceCollection<E1, E2>(c.SourceEntity);
			
//			foreach (EntityReference reference in c)
//			{
//
//			}
//		}
//	}
//}
 */
using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oDataUpdater.
	/// </summary>
	public class Db4oDataUpdater : DataUpdater
	{
		
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataUpdater(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		/// <summary>
		/// Prepares the provided entity to be updated and handles references.
		/// </summary>
		/// <param name="entity">The entity to be prepared for update.</param>
		public override void PreUpdate(IEntity entity)
		{
			EntityReferenceCollection toUpdate = new EntityReferenceCollection();
			EntityReferenceCollection toDelete = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing entity to be updated.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Entity ID : " + entity.ID);
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				EntityReferenceCollection latestReferences = EntitiesUtilities.GetReferences(entity);
				
				using (LogGroup logGroup2 = AppLogger.StartGroup("Creating list of deletable obsolete references.", NLog.LogLevel.Debug))
				{
					// Delete the old references
					//foreach (EntityIDReference reference in DataAccess.Data.GetObsoleteReferences(entity, new Collection<IEntity>(DataUtilities.GetReferencedEntities(latestReferences, entity)).IDs))
					foreach (EntityIDReference reference in DataAccess.Data.Referencer.GetObsoleteReferences(entity, new Guid[]{}))
					{
						DataAccess.Data.Activator.ActivateReference((EntityReference)reference);
						
						AppLogger.Debug("Reference type #1: " + reference.Type1Name);
						AppLogger.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
						AppLogger.Debug("Reference type #2: " + reference.Type2Name);
						AppLogger.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
						
						toDelete.Add(reference);
					}
				}
				
				Provider.Referencer.DeleteObsoleteReferences(toDelete);
				
				AppLogger.Debug("# to delete: " + toDelete.Count);
				
				using (LogGroup logGroup2 = AppLogger.StartGroup("Creating list of references to be updated.", NLog.LogLevel.Debug))
				{
					foreach (EntityIDReference reference in latestReferences)
					{
						AppLogger.Debug("Reference type #1: " + reference.Type1Name);
						AppLogger.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
						AppLogger.Debug("Reference type #2: " + reference.Type2Name);
						AppLogger.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
						
						/*EntityReference existingReference = DataAccess.Data.Referencer.GetReference(
							EntitiesUtilites.GetType(reference.Type1Name),
							reference.Entity1ID,
							reference.Property1Name,
							EntitiesUtilities.GetType(reference.Type2Name),
							reference.Entity2ID,
							reference.Property2Name,
							false);
						
						if (existingReference != null)
							reference = existingReference;*/
						
						//if (!DataAccess.Data.IsStored(reference))
						//{
						
						Data.DataAccess.Data.Activator.ActivateReference((EntityReference)reference);
						toUpdate.Add(reference.ToData());
						//}
					}
				}
				
				AppLogger.Debug("# to update: " + toUpdate.Count);
				
				Provider.Referencer.PersistReferences(toUpdate);
			}
		}
		
		/// <summary>
		/// Updates the provided entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		public override void Update(IEntity entity)
		{			
			using (LogGroup logGroup = AppLogger.StartGroup("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				Db4oDataStore store = (Db4oDataStore)GetDataStore(entity);
				
				if (!store.IsStored(entity))
					throw new ArgumentException("entity", "The provided entity of type '" + entity.GetType() + "' is not found in the store with name '" + store.Name + "'.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity.ID == Guid.Empty)
					throw new ArgumentException("entity.ID must be set.");
				
				
				//ReferenceValidator validator = new ReferenceValidator();
				//validator.CheckForCircularReference(entity);
				
				using (Batch batch = Batch.StartBatch())
				{
					if (entity == null)
						throw new ArgumentNullException("entity");
					
					//DataAccess.Data.Activator.Activate(entity);
					
					AppLogger.Debug("Entity type: " + entity.GetType().ToString());
					AppLogger.Debug("Entity ID: " + entity.ID);
					
					// Clone the entity, but do it in reverse so the data store is dealing with the bound instance/
					// The entity needs to be cloned so that the one currently in memory doesn't reflect the preparations applied before saving/updating.
					IEntity clonedEntity = entity;
					// TODO: See if entity needs to be cloned
					//entity = clonedEntity.Clone();
					
					AppLogger.Debug("Entity cloned");
					
					if (clonedEntity == entity)
						AppLogger.Debug("Cloned entity == original entity.");
					else
						AppLogger.Debug("Cloned entity == separate instance.");
					
					// Preupdate must be called to ensure all references are correctly stored
					PreUpdate(clonedEntity);
					
					if (clonedEntity != null)
					{
						
						DataUtilities.StripReferences(clonedEntity);
						
						store.ObjectContainer.Store(clonedEntity);
						AppLogger.Debug("Entity updated.");
						
						store.Commit();
						AppLogger.Debug("ObjectContainer committed.");
					}
				}
				
				
			}
		}
		
	}
}

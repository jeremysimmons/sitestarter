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
				
				EntityReferenceCollection latestReferences = Provider.Referencer.GetActiveReferences(entity);
				
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
				
				using (Batch batch = BatchState.StartBatch())
				{
					if (entity == null)
						throw new ArgumentNullException("entity");
					
					AppLogger.Debug("Entity type: " + entity.GetType().ToString());
					AppLogger.Debug("Entity ID: " + entity.ID);
					
					if (entity == null)
						throw new ArgumentException("The provided entity hasn't been saved so it cannot be updated.");
					
					// Preupdate must be called to ensure all references are correctly stored
					PreUpdate(entity);
					
					// Get a bound copy of the entity
					IDataReader reader = DataAccess.Data.InitializeDataReader();
					reader.AutoRelease = false; // Tell the reader not to unbind the entity from the store because it's still within the data access system
					
					IEntity existingEntity = reader.GetEntity(entity.GetType(), "ID", entity.ID);
					
					
					if (existingEntity != null)
					{
						// Activate the found entity
						Provider.Activator.Activate(existingEntity);
						
						// Copy the provided data to the bound entity
						entity.CopyTo(existingEntity);
						
						DataUtilities.StripReferences(existingEntity);
						
						// TODO: Check if needed. The entity is already bound when it's retrieved
						// so the Store call shouldn't be necessary
						// The entity in the store should already reflect the changes
						store.ObjectContainer.Store(existingEntity);
						AppLogger.Debug("Entity updated.");
						
						store.Commit();
						AppLogger.Debug("ObjectContainer committed.");
						
					}
				}
				
				
			}
		}
		
	}
}
